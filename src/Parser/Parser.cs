using System.Globalization;
using Lexer;

namespace Parser;

/// <summary>
/// Выполняет синтаксический разбор выражений языка Minion#.
/// Грамматика языка описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
    private readonly TokenStream _tokenStream;
    private readonly Dictionary<string, decimal> _variables;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Parser"/>.
    /// </summary>
    /// <param name="tokenStream">Поток токенов для разбора.</param>
    /// <param name="variables">Словарь значений переменных (опционально).</param>
    public Parser(TokenStream tokenStream, Dictionary<string, decimal>? variables = null)
    {
        _tokenStream = tokenStream ?? throw new ArgumentNullException(nameof(tokenStream));
        _variables = variables ?? new Dictionary<string, decimal>();
    }

    /// <summary>
    /// Выполняет синтаксический разбор и вычисление выражения.
    /// </summary>
    /// <param name="code">Исходный код выражения.</param>
    /// <param name="variables">Словарь значений переменных (опционально).</param>
    /// <returns>Результат вычисления выражения.</returns>
    public int EvaluateExpression(string code, Dictionary<string, decimal>? variables = null)
    {
        TokenStream stream = new TokenStream(code);
        Parser parser = new Parser(stream, variables);
        decimal result = parser.ParseExpression();

        // Проверяем, что все токены обработаны
        Token remainingToken = stream.Peek();
        if (remainingToken.Type != TokenType.EndOfFile)
        {
            throw new InvalidOperationException($"Неожиданный токен после выражения: {remainingToken}");
        }

        return (int)result;
    }

    /// <summary>
    /// Разбирает выражение верхнего уровня.
    /// Правила:
    ///     expression = logical-or-expression ;
    /// </summary>
    private decimal ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Разбирает логическое ИЛИ выражение.
    /// Правила:
    ///     logical-or-expression = logical-and-expression , { "bo-ca" , logical-and-expression } ;
    /// </summary>
    private decimal ParseLogicalOrExpression()
    {
        decimal left = ParseLogicalAndExpression();

        while (IsOperator("bo-ca"))
        {
            _tokenStream.Advance();
            decimal right = ParseLogicalAndExpression();
            left = (left != 0 || right != 0) ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// Разбирает логическое И выражение.
    /// Правила:
    ///     logical-and-expression = logical-not-expression , { "tropa" , logical-not-expression } ;
    /// </summary>
    private decimal ParseLogicalAndExpression()
    {
        decimal left = ParseLogicalNotExpression();

        while (IsOperator("tropa"))
        {
            _tokenStream.Advance();
            decimal right = ParseLogicalNotExpression();
            left = (left != 0 && right != 0) ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// Разбирает логическое НЕ выражение.
    /// Правила:
    ///     logical-not-expression = equality-expression
    ///                             | "makoroni" , logical-not-expression ;
    /// </summary>
    private decimal ParseLogicalNotExpression()
    {
        if (IsOperator("makoroni"))
        {
            _tokenStream.Advance();
            decimal value = ParseLogicalNotExpression();
            return value == 0 ? 1 : 0;
        }

        return ParseEqualityExpression();
    }

    /// <summary>
    /// Разбирает выражение равенства и неравенства.
    /// Правила:
    ///     equality-expression = relational-expression , { ("con" | "nocon") , relational-expression } ;
    /// </summary>
    private decimal ParseEqualityExpression()
    {
        decimal left = ParseRelationalExpression();

        while (IsOperator("con") || IsOperator("nocon"))
        {
            string op = _tokenStream.Peek().Lexeme;
            _tokenStream.Advance();
            decimal right = ParseRelationalExpression();

            bool result = op == "con" ? left == right : left != right;
            left = result ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// Разбирает выражение сравнения.
    /// Правила:
    ///     relational-expression = additive-expression ,
    ///                             { ("looka too" , [ "con" ] | "la" , [ "con" ]) , additive-expression } ;
    /// </summary>
    private decimal ParseRelationalExpression()
    {
        decimal left = ParseAdditiveExpression();

        while (IsOperator("la") || IsOperator("looka"))
        {
            string op = _tokenStream.Peek().Lexeme;
            _tokenStream.Advance();

            bool hasCon = false;

            // Обработка "la" с опциональным "con"
            if (op == "la")
            {
                if (IsOperator("con"))
                {
                    _tokenStream.Advance();
                    hasCon = true;
                }
            }

            // Обработка "looka too" с опциональным "con"
            else if (op == "looka")
            {
                if (!IsOperator("too"))
                {
                    throw new InvalidOperationException("После 'looka' ожидалось 'too'");
                }

                _tokenStream.Advance();
                op = "looka too";

                if (IsOperator("con"))
                {
                    _tokenStream.Advance();
                    hasCon = true;
                }
            }

            decimal right = ParseAdditiveExpression();

            bool result = op switch
            {
                "la" when !hasCon => left < right,
                "la" when hasCon => left <= right,
                "looka too" when !hasCon => left > right,
                "looka too" when hasCon => left >= right,
                _ => throw new InvalidOperationException($"Неизвестный оператор сравнения: {op}")
            };

            left = result ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// Разбирает выражение сложения и вычитания.
    /// Правила:
    ///     additive-expression = multiplicative-expression ,
    ///                           { ("melomo" | "flavuk") , multiplicative-expression } ;
    /// </summary>
    private decimal ParseAdditiveExpression()
    {
        decimal left = ParseMultiplicativeExpression();

        while (IsOperator("melomo") || IsOperator("flavuk"))
        {
            string op = _tokenStream.Peek().Lexeme;
            _tokenStream.Advance();

            // Проверка на ошибку: два бинарных оператора подряд
            if (IsOperator("melomo") || IsOperator("flavuk"))
            {
                throw new InvalidOperationException($"Неожиданный оператор после '{op}'");
            }

            decimal right = ParseMultiplicativeExpression();

            left = op == "melomo" ? left + right : left - right;
        }

        return left;
    }

    /// <summary>
    /// Разбирает выражение умножения, деления и остатка.
    /// Правила:
    ///     multiplicative-expression = unary-expression ,
    ///                                 { ("dibotada" | "poopaye" | "pado") , unary-expression } ;
    /// </summary>
    private decimal ParseMultiplicativeExpression()
    {
        decimal left = ParseUnaryExpression();

        while (IsOperator("dibotada") || IsOperator("poopaye") || IsOperator("pado"))
        {
            string op = _tokenStream.Peek().Lexeme;
            _tokenStream.Advance();
            decimal right = ParseUnaryExpression();

            left = op switch
            {
                "dibotada" => left * right,
                "poopaye" => right != 0 ? left / right : throw new DivideByZeroException("Деление на ноль"),
                "pado" => right != 0 ? left % right : throw new DivideByZeroException("Остаток от деления на ноль"),
                _ => throw new InvalidOperationException($"Неизвестный оператор: {op}")
            };
        }

        return left;
    }

    /// <summary>
    /// Разбирает унарное выражение.
    /// Правила:
    ///     unary-expression = power-expression
    ///                      | "melomo" , unary-expression
    ///                      | "flavuk" , unary-expression;
    /// </summary>
    private decimal ParseUnaryExpression()
    {
        if (IsOperator("melomo"))
        {
            _tokenStream.Advance();
            return ParseUnaryExpression();
        }

        if (IsOperator("flavuk"))
        {
            _tokenStream.Advance();
            return -ParseUnaryExpression();
        }

        return ParsePowerExpression();
    }

    /// <summary>
    /// Разбирает выражение возведения в степень.
    /// Правила:
    ///     power-expression = primary-expression , [ "beedo" , power-expression ] ;
    /// </summary>
    private decimal ParsePowerExpression()
    {
        decimal left = ParsePrimaryExpression();

        if (IsOperator("beedo"))
        {
            _tokenStream.Advance();
            decimal right = ParsePowerExpression();
            return (decimal)Math.Pow((double)left, (double)right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает первичное выражение.
    /// Правила:
    ///     primary-expression = number-literal
    ///                        | identifier
    ///                        | constant
    ///                        | "(" , expression , ")"
    ///                        | function-call ;
    /// </summary>
    private decimal ParsePrimaryExpression()
    {
        Token token = _tokenStream.Peek();

        // Числовой литерал
        if (token.Type == TokenType.NumberLiteral)
        {
            _tokenStream.Advance();
            return decimal.Parse(token.Lexeme, CultureInfo.InvariantCulture);
        }

        // Логические литералы
        if (token.Type == TokenType.Da)
        {
            _tokenStream.Advance();
            return 1;
        }

        if (token.Type == TokenType.No)
        {
            _tokenStream.Advance();
            return 0;
        }

        // Строковые литералы (пока не поддерживаем вычисление)
        if (token.Type == TokenType.StringLiteral)
        {
            _tokenStream.Advance();
            throw new NotImplementedException("Вычисление строковых литералов не реализовано");
        }

        // Идентификатор или константа
        if (token.Type == TokenType.Identifier)
        {
            _tokenStream.Advance();
            string name = token.Lexeme;

            // Проверяем, является ли это константой
            if (name == "belloPi")
            {
                return 3.141592653589793m;
            }

            if (name == "belloE")
            {
                return 2.718281828459045m;
            }

            // Проверяем, является ли это вызовом функции
            if (_tokenStream.Peek().Type == TokenType.Delimiter && _tokenStream.Peek().Lexeme == "(")
            {
                return ParseFunctionCall(name);
            }

            // Иначе это переменная
            if (_variables.TryGetValue(name, out decimal value))
            {
                return value;
            }

            throw new InvalidOperationException($"Неизвестная переменная: {name}");
        }

        // Скобки
        if (token.Type == TokenType.Delimiter && token.Lexeme == "(")
        {
            _tokenStream.Advance();
            decimal result = ParseExpression();

            if (_tokenStream.Peek().Type != TokenType.Delimiter || _tokenStream.Peek().Lexeme != ")")
            {
                throw new InvalidOperationException("Ожидалась закрывающая скобка ')'");
            }

            _tokenStream.Advance();
            return result;
        }

        throw new InvalidOperationException($"Неожиданный токен: {token}");
    }

    /// <summary>
    /// Разбирает вызов функции.
    /// Правила:
    ///     function-call = identifier , "(" , [ argument-list ] , ")" ;
    ///     argument-list = expression , { "," , expression } ;
    /// </summary>
    private decimal ParseFunctionCall(string functionName)
    {
        // Уже прочитали идентификатор и открывающую скобку
        if (_tokenStream.Peek().Type != TokenType.Delimiter || _tokenStream.Peek().Lexeme != "(")
        {
            throw new InvalidOperationException("Ожидалась открывающая скобка '('");
        }

        _tokenStream.Advance();

        List<decimal> arguments = new List<decimal>();

        // Если сразу закрывающая скобка, то аргументов нет
        if (_tokenStream.Peek().Type == TokenType.Delimiter && _tokenStream.Peek().Lexeme == ")")
        {
            _tokenStream.Advance();
            return BuiltinFunctions.Invoke(functionName, arguments);
        }

        // Читаем первый аргумент
        arguments.Add(ParseExpression());

        // Читаем остальные аргументы
        while (_tokenStream.Peek().Type == TokenType.Delimiter && _tokenStream.Peek().Lexeme == ",")
        {
            _tokenStream.Advance();
            arguments.Add(ParseExpression());
        }

        // Закрывающая скобка
        if (_tokenStream.Peek().Type != TokenType.Delimiter || _tokenStream.Peek().Lexeme != ")")
        {
            throw new InvalidOperationException("Ожидалась закрывающая скобка ')' или запятая ','");
        }

        _tokenStream.Advance();
        return BuiltinFunctions.Invoke(functionName, arguments);
    }

    /// <summary>
    /// Проверяет, является ли текущий токен оператором с заданным именем.
    /// </summary>
    private bool IsOperator(string operatorName)
    {
        Token token = _tokenStream.Peek();
        return token.Type == TokenType.Operator && token.Lexeme == operatorName;
    }
}
