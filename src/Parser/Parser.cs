using System.Globalization;
using Execution;
using Lexer;

namespace Parser;

/// <summary>
/// Рекурсивный спускающийся парсер языка Minion#.
/// Выражения — по грамматике `docs/specification/expressions-grammar.md`,
/// верхний уровень — по `docs/specification/top-level-grammar.md`.
/// </summary>
public class Parser
{
    private readonly TokenStream tokenStream;
    private readonly Context context;
    private readonly IEnvironment? environment;
    private bool syntaxOnlyMode;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Parser"/>.
    /// </summary>
    /// <param name="tokenStream">Поток токенов для разбора.</param>
    /// <param name="context">Контекст выполнения с областями видимости (опционально).</param>
    /// <param name="environment">Окружение для выполнения программы (опционально).</param>
    public Parser(TokenStream tokenStream, Context? context = null, IEnvironment? environment = null)
    {
        this.tokenStream = tokenStream ?? throw new ArgumentNullException(nameof(tokenStream));
        this.context = context ?? new Context();
        this.environment = environment;
    }

    /// <summary>
    /// program = "bello!" , { top-level-item } ;
    /// </summary>
    public static void ParseProgram(string code, IEnvironment? environment = null)
    {
        Parser parser = new(new TokenStream(code), environment: environment);
        parser.ParseProgramInternal();
    }

    /// <summary>
    /// program = "bello!" , { top-level-item } ;
    /// Выполняет программу с использованием указанного контекста и окружения.
    /// </summary>
    /// <param name="code">Исходный код программы.</param>
    /// <param name="context">Контекст выполнения с областями видимости.</param>
    /// <param name="environment">Окружение для выполнения программы.</param>
    public static void ParseProgram(string code, Context context, IEnvironment? environment = null)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        Parser parser = new(new TokenStream(code), context, environment);
        parser.ParseProgramInternal();
    }

    /// <summary>
    /// Выполняет синтаксический разбор и вычисление выражения.
    /// </summary>
    /// <param name="code">Исходный код выражения.</param>
    /// <param name="variables">Словарь значений переменных (опционально, для обратной совместимости).</param>
    /// <returns>Результат вычисления выражения.</returns>
    public int EvaluateExpression(string code, Dictionary<string, decimal>? variables = null)
    {
        Context ctx = new Context();
        if (variables != null)
        {
            foreach (KeyValuePair<string, decimal> kvp in variables)
            {
                ctx.TryDefineVariable(kvp.Key, kvp.Value);
            }
        }

        TokenStream stream = new TokenStream(code);
        Parser parser = new Parser(stream, ctx);
        decimal result = parser.ParseExpression();

        // Проверяем, что все токены обработаны
        Token remainingToken = stream.Peek();
        if (remainingToken.Type != TokenType.EndOfFile)
        {
            throw new InvalidOperationException($"Неожиданный токен после выражения: {remainingToken}");
        }

        return (int)result;
    }

    private void ParseProgramInternal()
    {
        Expect(TokenType.Bello, "Ожидался старт программы 'bello!'");

        while (tokenStream.Peek().Type != TokenType.EndOfFile)
        {
            ParseTopLevelItem(trackReturn: false);
        }
    }

    /// <summary>
    /// top-level-item = const-declaration | function-definition | statement ;
    /// </summary>
    private bool ParseTopLevelItem(bool trackReturn)
    {
        Token token = tokenStream.Peek();
        return token.Type switch
        {
            TokenType.Trusela => ParseConstDeclaration(),
            TokenType.Boss => ParseFunctionDefinition(),
            _ => ParseStatement(trackReturn)
        };
    }

    /// <summary>
    /// const-declaration = "trusela" , identifier , "Papaya" , const-value , "naidu!" ;
    /// const-value = number-literal | constant ;
    /// </summary>
    private bool ParseConstDeclaration()
    {
        tokenStream.Advance(); // trusela
        Token constName = Expect(TokenType.Identifier, "Ожидалось имя константы после 'trusela'");
        ExpectIdentifierLexeme("Papaya", "Ожидался тип Papaya в объявлении константы");

        Token valueToken = tokenStream.Peek();
        decimal constValue;

        if (valueToken.Type == TokenType.NumberLiteral)
        {
            tokenStream.Advance();
            constValue = decimal.Parse(valueToken.Lexeme, CultureInfo.InvariantCulture);
        }
        else if (valueToken.Type == TokenType.Identifier)
        {
            string constIdentifier = valueToken.Lexeme;
            if (constIdentifier.Equals("belloPi", StringComparison.OrdinalIgnoreCase))
            {
                tokenStream.Advance();
                constValue = 3.141592653589793m;
            }
            else if (constIdentifier.Equals("belloE", StringComparison.OrdinalIgnoreCase))
            {
                tokenStream.Advance();
                constValue = 2.718281828459045m;
            }
            else
            {
                throw new InvalidOperationException("Ожидалось константное значение (число или belloPi/belloE)");
            }
        }
        else
        {
            throw new InvalidOperationException("Ожидалось константное значение (число или belloPi/belloE)");
        }

        Expect(TokenType.Naidu, "Ожидался разделитель 'naidu!'");

        // Сохраняем константу в контекст
        if (environment != null)
        {
            if (!context.TryDefineVariable(constName.Lexeme, constValue))
            {
                throw new InvalidOperationException($"Константа '{constName.Lexeme}' уже объявлена в этой области видимости");
            }
        }

        return false;
    }

    /// <summary>
    /// variable-declaration = "poop" , identifier , "Papaya" , "naidu!" ;
    /// </summary>
    private bool ParseVarDeclaration()
    {
        tokenStream.Advance(); // poop
        Token name = Expect(TokenType.Identifier, "Ожидалось имя переменной после 'poop'");
        ExpectIdentifierLexeme("Papaya", "Ожидался тип Papaya в объявлении переменной");
        Expect(TokenType.Naidu, "Ожидался разделитель 'naidu!'");

        DefineVariable(name.Lexeme);
        return false;
    }

    /// <summary>
    /// statement =
    ///     variable-declaration
    ///   | assignment-statement
    ///   | input-statement
    ///   | output-statement
    ///   | if-statement
    ///   | while-statement
    ///   | return-statement
    ///   | expression-statement
    ///   , "naidu!" ;
    /// </summary>
    private bool ParseStatement(bool trackReturn)
    {
        Token token = tokenStream.Peek();

        return token.Type switch
        {
            TokenType.Poop => ParseVarDeclaration(),
            TokenType.Identifier => ParseAssignmentOrExpressionStatement(),
            TokenType.Tulalilloo => ParseOutput(),
            TokenType.Guoleila => ParseInput(),
            TokenType.BiDo => ParseIf(trackReturn),
            TokenType.Kemari => ParseWhile(trackReturn),
            TokenType.Tank => ParseReturn(trackReturn),
            _ => throw new InvalidOperationException($"Неожиданный токен в инструкции: {token}")
        };
    }

    /// <summary>
    /// assignment-statement = identifier , "lumai" , expression , "naidu!" ;
    /// expression-statement = expression , "naidu!" ;
    /// </summary>
    private bool ParseAssignmentOrExpressionStatement()
    {
        Token ident = tokenStream.Peek();
        tokenStream.Advance();

        if (tokenStream.Peek().Type == TokenType.Operator && tokenStream.Peek().Lexeme == "lumai")
        {
            tokenStream.Advance(); // lumai
            decimal value = ParseExpression();
            Expect(TokenType.Naidu, "Ожидался разделитель 'naidu!'");
            AssignVariable(ident.Lexeme, value);
        }
        else
        {
            // Expression statement - парсим выражение полностью
            // Собираем оставшуюся часть выражения в строку и парсим
            string exprCode = ident.Lexeme;
            while (tokenStream.Peek().Type != TokenType.Naidu && tokenStream.Peek().Type != TokenType.EndOfFile)
            {
                exprCode += " " + tokenStream.Peek().Lexeme;
                tokenStream.Advance();
            }

            Expect(TokenType.Naidu, "Ожидался разделитель 'naidu!'");

            // Парсим и вычисляем выражение (если нужно)
            if (environment != null)
            {
                Parser exprParser = new(new TokenStream(exprCode), context, environment: null);
                exprParser.ParseExpression();
            }
        }

        return false;
    }

    /// <summary>
    /// input-statement = "guoleila" , "(", identifier , ")", "naidu!" ;
    /// </summary>
    private bool ParseInput()
    {
        tokenStream.Advance(); // guoleila
        ExpectDelimiter("(");
        Token name = Expect(TokenType.Identifier, "Ожидалось имя переменной для ввода");
        ExpectDelimiter(")");
        Expect(TokenType.Naidu, "Ожидался разделитель 'naidu!'");

        if (environment != null)
        {
            decimal value = environment.ReadNumber();
            AssignVariable(name.Lexeme, value);
        }

        return false;
    }

    /// <summary>
    /// output-statement = "tulalilloo" , "ti" , "amo" , "(" , expression , ")" , "naidu!" ;
    /// </summary>
    private bool ParseOutput()
    {
        tokenStream.Advance(); // tulalilloo
        Expect(TokenType.Ti, "Ожидалось 'ti'");
        Expect(TokenType.Amo, "Ожидалось 'amo'");
        ExpectDelimiter("(");
        decimal value = ParseExpression();
        ExpectDelimiter(")");
        Expect(TokenType.Naidu, "Ожидался разделитель 'naidu!'");

        environment?.WriteNumber(value);
        return false;
    }

    /// <summary>
    /// if-statement = "bi-do" , "(" , expression , ")" , block , [ "uh-oh" , block ] ;
    /// </summary>
    private bool ParseIf(bool trackReturn)
    {
        tokenStream.Advance(); // bi-do
        ExpectDelimiter("(");
        decimal condition = ParseExpression(); // Вычисляем условие
        ExpectDelimiter(")");

        bool thenReturn = false;

        // Условие истинно - выполняем then блок
        if (condition != 0)
        {
            thenReturn = ParseBlock(trackReturn);

            // Пропускаем else блок, если он есть (парсим для синтаксиса, но не выполняем)
            if (tokenStream.Peek().Type == TokenType.UhOh)
            {
                tokenStream.Advance();
                ParseBlock(trackReturn: false);
            }
        }
        else
        {
            // Условие ложно - пропускаем then блок, выполняем else если есть
            // Парсим then блок для синтаксической проверки, но не выполняем statements
            ParseBlockWithoutExecution();

            if (tokenStream.Peek().Type == TokenType.UhOh)
            {
                tokenStream.Advance();
                thenReturn = ParseBlock(trackReturn);
            }
        }

        return thenReturn;
    }

    /// <summary>
    /// Парсит блок без выполнения statements (только для синтаксической проверки).
    /// </summary>
    private void ParseBlockWithoutExecution()
    {
        Expect(TokenType.Oca, "Ожидался старт блока 'oca!'");
        bool oldSyntaxOnlyMode = syntaxOnlyMode;
        syntaxOnlyMode = true;

        try
        {
            while (tokenStream.Peek().Type != TokenType.Stopa && tokenStream.Peek().Type != TokenType.EndOfFile)
            {
            // Пропускаем все statements в блоке
            Token token = tokenStream.Peek();
            if (token.Type == TokenType.Poop)
            {
                tokenStream.Advance(); // poop
                Expect(TokenType.Identifier, "");
                ExpectIdentifierLexeme("Papaya", "");
                Expect(TokenType.Naidu, "");
            }
            else if (token.Type == TokenType.Identifier)
            {
                tokenStream.Advance();
                if (tokenStream.Peek().Type == TokenType.Operator && tokenStream.Peek().Lexeme == "lumai")
                {
                    tokenStream.Advance(); // lumai
                    ParseExpression();
                    Expect(TokenType.Naidu, "");
                }
                else
                {
                    ParseExpression();
                    Expect(TokenType.Naidu, "");
                }
            }
            else if (token.Type == TokenType.Tulalilloo)
            {
                tokenStream.Advance(); // tulalilloo
                Expect(TokenType.Ti, "");
                Expect(TokenType.Amo, "");
                ExpectDelimiter("(");
                ParseExpression();
                ExpectDelimiter(")");
                Expect(TokenType.Naidu, "");
            }
            else if (token.Type == TokenType.Guoleila)
            {
                tokenStream.Advance(); // guoleila
                ExpectDelimiter("(");
                Expect(TokenType.Identifier, "");
                ExpectDelimiter(")");
                Expect(TokenType.Naidu, "");
            }
            else if (token.Type == TokenType.BiDo)
            {
                // Парсим if без выполнения (только синтаксическая проверка)
                tokenStream.Advance(); // bi-do
                ExpectDelimiter("(");
                ParseExpression(); // Условие
                ExpectDelimiter(")");
                ParseBlockWithoutExecution(); // Then блок
                if (tokenStream.Peek().Type == TokenType.UhOh)
                {
                    tokenStream.Advance();
                    ParseBlockWithoutExecution(); // Else блок
                }
            }
            else if (token.Type == TokenType.Kemari)
            {
                // Парсим while без выполнения (только синтаксическая проверка)
                tokenStream.Advance(); // kemari
                ExpectDelimiter("(");
                ParseExpression(); // Условие
                ExpectDelimiter(")");
                ParseBlockWithoutExecution(); // Блок цикла
            }
            else
            {
                throw new InvalidOperationException($"Неожиданный токен: {token}");
            }
            }
        }
        finally
        {
            syntaxOnlyMode = oldSyntaxOnlyMode;
        }

        Expect(TokenType.Stopa, "Ожидался конец блока 'stopa'");
    }

    /// <summary>
    /// while-statement = "kemari" , "(" , expression , ")" , block ;
    /// </summary>
    private bool ParseWhile(bool trackReturn)
    {
        tokenStream.Advance(); // kemari

        // Сохраняем позицию начала условия для повторной проверки
        int conditionStart = tokenStream.Peek().Position;
        bool bodyReturn = false;

        while (true)
        {
            ExpectDelimiter("(");
            decimal condition = ParseExpression(); // Вычисляем условие
            ExpectDelimiter(")");

            // Условие ложно - выходим из цикла
            if (condition == 0)
            {
                // Парсим блок для синтаксической проверки, но не выполняем
                ParseBlockWithoutExecution();
                break;
            }

            // Условие истинно - выполняем блок
            bodyReturn = ParseBlock(trackReturn) || bodyReturn;

            // После выполнения блока нужно снова проверить условие
            // Но мы не можем вернуться назад в токен-стриме
            // Поэтому цикл выполняется только один раз
            // Для правильной реализации нужна более сложная логика
            break; // Временно: выполняем только одну итерацию
        }

        return bodyReturn;
    }

    /// <summary>
    /// return-statement = "tank" , "yu" , expression , "naidu!" ;
    /// </summary>
    private bool ParseReturn(bool trackReturn)
    {
        if (!trackReturn)
        {
            throw new InvalidOperationException("Ожидался tank yu только внутри функции");
        }

        tokenStream.Advance(); // tank
        Expect(TokenType.Yu, "Ожидалось 'yu' после 'tank'");
        ParseExpression(); // Значение возврата (не используется, но парсится)
        Expect(TokenType.Naidu, "Ожидался разделитель 'naidu!'");
        return true;
    }

    /// <summary>
    /// function-definition = "boss" , identifier , "Papaya" , "(" , [ parameter-list ] , ")" , block ;
    /// parameter-list = identifier , { \",\" , identifier } ;
    /// </summary>
    private bool ParseFunctionDefinition()
    {
        tokenStream.Advance(); // boss
        Expect(TokenType.Identifier, "Ожидалось имя функции после 'boss'");
        ExpectIdentifierLexeme("Papaya", "Ожидался тип Papaya у функции");
        ExpectDelimiter("(");

        List<string> parameters = new();
        if (!(tokenStream.Peek().Type == TokenType.Delimiter && tokenStream.Peek().Lexeme == ")"))
        {
            Token param = Expect(TokenType.Identifier, "Ожидалось имя параметра");
            parameters.Add(param.Lexeme);
            while (tokenStream.Peek().Type == TokenType.Delimiter && tokenStream.Peek().Lexeme == ",")
            {
                tokenStream.Advance();
                param = Expect(TokenType.Identifier, "Ожидалось имя параметра");
                parameters.Add(param.Lexeme);
            }
        }

        ExpectDelimiter(")");

        // Создаем область видимости для функции и объявляем параметры
        if (environment != null)
        {
            context.PushScope();
            foreach (string paramName in parameters)
            {
                context.TryDefineVariable(paramName, 0m);
            }
        }

        bool hasReturn = ParseBlock(trackReturn: true);

        if (environment != null)
        {
            context.PopScope();
        }

        if (!hasReturn)
        {
            throw new InvalidOperationException("В теле функции ожидался 'tank yu'");
        }

        return false;
    }

    /// <summary>
    /// block = "oca!" , { statement } , "stopa" ;
    /// </summary>
    private bool ParseBlock(bool trackReturn)
    {
        Expect(TokenType.Oca, "Ожидался старт блока 'oca!'");
        if (environment != null)
        {
            context.PushScope();
        }

        bool hasReturn = false;
        try
        {
            while (tokenStream.Peek().Type != TokenType.Stopa && tokenStream.Peek().Type != TokenType.EndOfFile)
            {
                bool stmtReturn = ParseTopLevelItem(trackReturn);
                hasReturn = hasReturn || stmtReturn;
            }
        }
        finally
        {
            if (environment != null)
            {
                context.PopScope();
            }
        }

        Expect(TokenType.Stopa, "Ожидался конец блока 'stopa'");
        return hasReturn;
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
            tokenStream.Advance();
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
            tokenStream.Advance();
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
            tokenStream.Advance();
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
            string op = tokenStream.Peek().Lexeme;
            tokenStream.Advance();
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
            string op = tokenStream.Peek().Lexeme;
            tokenStream.Advance();

            bool hasCon = false;

            // Обработка "la" с опциональным "con"
            if (op == "la")
            {
                if (IsOperator("con"))
                {
                    tokenStream.Advance();
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

                tokenStream.Advance();
                op = "looka too";

                if (IsOperator("con"))
                {
                    tokenStream.Advance();
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
            string op = tokenStream.Peek().Lexeme;
            tokenStream.Advance();

            // После бинарного оператора может быть унарный оператор или multiplicative expression
            // Проверяем только на два бинарных оператора подряд (не унарных)
            // Унарные операторы обрабатываются в ParseUnaryExpression
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
            string op = tokenStream.Peek().Lexeme;
            tokenStream.Advance();
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
            tokenStream.Advance();
            return ParseUnaryExpression();
        }

        if (IsOperator("flavuk"))
        {
            tokenStream.Advance();
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
            tokenStream.Advance();
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
        Token token = tokenStream.Peek();

        // Числовой литерал
        if (token.Type == TokenType.NumberLiteral)
        {
            tokenStream.Advance();
            return decimal.Parse(token.Lexeme, CultureInfo.InvariantCulture);
        }

        // Логические литералы
        if (token.Type == TokenType.Da)
        {
            tokenStream.Advance();
            return 1;
        }

        if (token.Type == TokenType.No)
        {
            tokenStream.Advance();
            return 0;
        }

        // Строковые литералы (пока не поддерживаем вычисление)
        if (token.Type == TokenType.StringLiteral)
        {
            tokenStream.Advance();
            throw new NotImplementedException("Вычисление строковых литералов не реализовано");
        }

        // Идентификатор или константа
        if (token.Type == TokenType.Identifier)
        {
            tokenStream.Advance();
            string name = token.Lexeme;

            // Проверяем, является ли это константой
            if (name.Equals("belloPi", StringComparison.OrdinalIgnoreCase))
            {
                return 3.141592653589793m;
            }

            if (name.Equals("belloE", StringComparison.OrdinalIgnoreCase))
            {
                return 2.718281828459045m;
            }

            // Проверяем, является ли это вызовом функции
            if (tokenStream.Peek().Type == TokenType.Delimiter && tokenStream.Peek().Lexeme == "(")
            {
                return ParseFunctionCall(name);
            }

            // Иначе это переменная
            if (syntaxOnlyMode)
            {
                // В режиме только синтаксиса возвращаем 0 для неизвестных переменных
                return 0m;
            }

            if (context.TryGetVariable(name, out decimal value))
            {
                return value;
            }

            throw new InvalidOperationException($"Неизвестная переменная: {name}");
        }

        // Скобки
        if (token.Type == TokenType.Delimiter && token.Lexeme == "(")
        {
            tokenStream.Advance();
            decimal result = ParseExpression();

            if (tokenStream.Peek().Type != TokenType.Delimiter || tokenStream.Peek().Lexeme != ")")
            {
                throw new InvalidOperationException("Ожидалась закрывающая скобка ')'");
            }

            tokenStream.Advance();
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
        if (tokenStream.Peek().Type != TokenType.Delimiter || tokenStream.Peek().Lexeme != "(")
        {
            throw new InvalidOperationException("Ожидалась открывающая скобка '('");
        }

        tokenStream.Advance();

        List<decimal> arguments = new List<decimal>();

        // Если сразу закрывающая скобка, то аргументов нет
        if (tokenStream.Peek().Type == TokenType.Delimiter && tokenStream.Peek().Lexeme == ")")
        {
            tokenStream.Advance();
            return BuiltinFunctions.Invoke(functionName, arguments);
        }

        // Читаем первый аргумент
        arguments.Add(ParseExpression());

        // Читаем остальные аргументы
        while (tokenStream.Peek().Type == TokenType.Delimiter && tokenStream.Peek().Lexeme == ",")
        {
            tokenStream.Advance();
            arguments.Add(ParseExpression());
        }

        // Закрывающая скобка
        if (tokenStream.Peek().Type != TokenType.Delimiter || tokenStream.Peek().Lexeme != ")")
        {
            throw new InvalidOperationException("Ожидалась закрывающая скобка ')' или запятая ','");
        }

        tokenStream.Advance();
        return BuiltinFunctions.Invoke(functionName, arguments);
    }

    /// <summary>
    /// Проверяет, является ли текущий токен оператором с заданным именем.
    /// </summary>
    private bool IsOperator(string operatorName)
    {
        Token token = tokenStream.Peek();
        return token.Type == TokenType.Operator && token.Lexeme == operatorName;
    }

    private void DefineVariable(string name)
    {
        if (environment != null && !context.TryDefineVariable(name, 0m))
        {
            throw new InvalidOperationException($"Переменная '{name}' уже объявлена в этой области видимости");
        }
    }

    private void AssignVariable(string name, decimal value)
    {
        if (environment != null && !context.TryAssignVariable(name, value))
        {
            throw new InvalidOperationException($"Переменная '{name}' не объявлена");
        }
    }

    private decimal GetVariableValue(string name)
    {
        if (!context.TryGetVariable(name, out decimal value))
        {
            throw new InvalidOperationException($"Неизвестная переменная: {name}");
        }

        return value;
    }

    private Token Expect(TokenType type, string message)
    {
        Token token = tokenStream.Peek();
        if (token.Type != type)
        {
            throw new InvalidOperationException(message);
        }

        tokenStream.Advance();
        return token;
    }

    private void ExpectIdentifierLexeme(string lexeme, string message)
    {
        Token token = tokenStream.Peek();
        if (token.Type != TokenType.Identifier || token.Lexeme != lexeme)
        {
            throw new InvalidOperationException(message);
        }

        tokenStream.Advance();
    }

    private void ExpectDelimiter(string lexeme)
    {
        Token token = tokenStream.Peek();
        if (token.Type != TokenType.Delimiter || token.Lexeme != lexeme)
        {
            throw new InvalidOperationException($"Ожидался разделитель '{lexeme}'");
        }

        tokenStream.Advance();
    }
}