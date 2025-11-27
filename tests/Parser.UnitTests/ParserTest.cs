using Parser;
using Xunit;

namespace Parser.UnitTests;

/// <summary>
/// Тесты для парсера выражений языка Minion#.
/// </summary>
public class ParserTest
{
    [Theory]
    [MemberData(nameof(GetPrimaryExpressionData))]
    public void ParsePrimaryExpressionTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetIdentifierData))]
    public void ParseIdentifierTest(string code, Dictionary<string, decimal> variables, int expected)
    {
        Parser parser = new Parser(new TokenStream(code), variables);
        int actual = parser.EvaluateExpression(code, variables);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetUnaryOperatorData))]
    public void ParseUnaryOperatorTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetUnaryOperatorWithVariablesData))]
    public void ParseUnaryOperatorWithVariablesTest(string code, Dictionary<string, decimal> variables, int expected)
    {
        Parser parser = new Parser(new TokenStream(code), variables);
        int actual = parser.EvaluateExpression(code, variables);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetPowerExpressionData))]
    public void ParsePowerExpressionTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetMultiplicativeExpressionData))]
    public void ParseMultiplicativeExpressionTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetAdditiveExpressionData))]
    public void ParseAdditiveExpressionTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetRelationalExpressionData))]
    public void ParseRelationalExpressionTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetEqualityExpressionData))]
    public void ParseEqualityExpressionTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetLogicalAndExpressionData))]
    public void ParseLogicalAndExpressionTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetLogicalOrExpressionData))]
    public void ParseLogicalOrExpressionTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetLogicalNotExpressionData))]
    public void ParseLogicalNotExpressionTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetOperatorPrecedenceData))]
    public void ParseOperatorPrecedenceTest(string code, int expected)
    {
        Parser parser = new Parser(new TokenStream(code));
        int actual = parser.EvaluateExpression(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetErrorHandlingData))]
    public void ParseErrorHandlingTest(string code, Type expectedExceptionType)
    {
        Parser parser = new Parser(new TokenStream(code));
        Assert.Throws(expectedExceptionType, () => parser.EvaluateExpression(code));
    }

    /// <summary>
    /// Тестовые данные для идентификаторов (переменных).
    /// </summary>
    public static TheoryData<string, Dictionary<string, decimal>, int> GetIdentifierData()
    {
        return new TheoryData<string, Dictionary<string, decimal>, int>
        {
            // Простой идентификатор
            { "x", new Dictionary<string, decimal> { { "x", 42 } }, 42 },
            { "x", new Dictionary<string, decimal> { { "x", 10 } }, 10 },

            // Идентификатор с подчёркиванием
            { "_temp", new Dictionary<string, decimal> { { "_temp", 5 } }, 5 },

            // Идентификатор с цифрами
            { "level2", new Dictionary<string, decimal> { { "level2", 2 } }, 2 },

            // Идентификатор с несколькими символами
            { "myVariable", new Dictionary<string, decimal> { { "myVariable", 100 } }, 100 },

            // Идентификатор в выражении
            { "x melomo y", new Dictionary<string, decimal> { { "x", 5 }, { "y", 3 } }, 8 },
            { "a dibotada b", new Dictionary<string, decimal> { { "a", 2 }, { "b", 3 } }, 6 },
            { "x beedo 2", new Dictionary<string, decimal> { { "x", 5 } }, 25 },
        };
    }

    /// <summary>
    /// Тестовые данные для первичных выражений.
    /// </summary>
    public static TheoryData<string, int> GetPrimaryExpressionData()
    {
        return new TheoryData<string, int>
        {
            // Числовые литералы
            { "42", 42 },
            { "-7", -7 },
            { "3.14", 3 }, // 3.14 -> 3 при приведении к int
            { "-0.5", 0 }, // -0.5 -> 0 при приведении к int
            { "0", 0 },
            { "-0", 0 },
            { "42.0", 42 }, // Число без точки интерпретируется как 42.0

            // Логические литералы
            { "da", 1 },
            { "no", 0 },
            { "Da", 1 },
            { "No", 0 },

            // Константы
            { "belloPi", 3 }, // 3.14159... -> 3 при приведении к int
            { "belloE", 2 }, // 2.71828... -> 2 при приведении к int

            // Скобки
            { "(42)", 42 },
            { "(2 melomo 3)", 5 }, // Выражение с операцией в скобках
            { "((2 melomo 3))", 5 }, // Вложенные скобки
            { "(((42)))", 42 }, // Множественные уровни вложенности

            // Вызов функции
            { "muak(5)", 5 },
            { "muak(-5)", 5 },
            { "miniboss(3, 5)", 3 },
            { "miniboss(5, 3)", 3 },
            { "miniboss(3, 5, 1)", 1 },
            { "bigboss(3, 5)", 5 },
            { "bigboss(5, 3)", 5 },
            { "bigboss(3, 5, 7)", 7 },
            { "muak(2 melomo 3)", 5 }, // Вызов функции с выражением в аргументе
            { "miniboss(muak(5), 3)", 3 }, // Вызов функции с вложенным вызовом
        };
    }

    /// <summary>
    /// Тестовые данные для унарных операторов.
    /// </summary>
    public static TheoryData<string, int> GetUnaryOperatorData()
    {
        return new TheoryData<string, int>
        {
            // Унарный плюс
            { "melomo 5", 5 },
            { "melomo 10", 10 },
            { "melomo (2 melomo 3)", 5 }, // Унарный плюс перед выражением в скобках
            { "melomo muak(5)", 5 }, // Унарный плюс перед вызовом функции

            // Унарный минус
            { "flavuk 5", -5 },
            { "flavuk 10", -10 },
            { "flavuk (2 melomo 3)", -5 }, // Унарный минус перед выражением в скобках
            { "flavuk muak(5)", -5 }, // Унарный минус перед вызовом функции
            { "flavuk flavuk 5", 5 }, // Двойной минус
        };
    }

    /// <summary>
    /// Тестовые данные для унарных операторов с переменными.
    /// </summary>
    public static TheoryData<string, Dictionary<string, decimal>, int> GetUnaryOperatorWithVariablesData()
    {
        return new TheoryData<string, Dictionary<string, decimal>, int>
        {
            // Унарный плюс перед переменной
            { "melomo x", new Dictionary<string, decimal> { { "x", 5 } }, 5 },
            { "melomo x", new Dictionary<string, decimal> { { "x", 10 } }, 10 },

            // Унарный минус перед переменной
            { "flavuk x", new Dictionary<string, decimal> { { "x", 5 } }, -5 },
            { "flavuk x", new Dictionary<string, decimal> { { "x", 10 } }, -10 },
            { "flavuk flavuk x", new Dictionary<string, decimal> { { "x", 5 } }, 5 }, // Двойной унарный минус
        };
    }

    /// <summary>
    /// Тестовые данные для возведения в степень.
    /// </summary>
    public static TheoryData<string, int> GetPowerExpressionData()
    {
        return new TheoryData<string, int>
        {
            // Базовые случаи
            { "2 beedo 3", 8 },
            { "3 beedo 2", 9 },
            { "2 beedo 0", 1 },
            { "5 beedo 1", 5 },
            { "2 beedo 3 beedo 2", 512 }, // Правая ассоциативность: 2^(3^2) = 2^9 = 512
            { "2 beedo 3 beedo 2 beedo 1", 512 }, // Три уровня возведения
            { "flavuk 2 beedo 3", -8 }, // Возведение в степень с унарным минусом: (-2)^3 = -8
            { "muak(5) beedo 2", 25 }, // Возведение в степень с вызовом функции
            { "(2 melomo 3) beedo 2", 25 }, // Возведение в степень с выражением в скобках: (2+3)^2 = 25
            { "2 beedo (3 beedo 2)", 512 }, // Возведение в степень в скобках: 2^(3^2) = 512
        };
    }

    /// <summary>
    /// Тестовые данные для умножения, деления и остатка.
    /// </summary>
    public static TheoryData<string, int> GetMultiplicativeExpressionData()
    {
        return new TheoryData<string, int>
        {
            // Умножение
            { "2 dibotada 3", 6 },
            { "4 dibotada 5", 20 },
            { "flavuk 2 dibotada 3", -6 }, // Умножение с унарным минусом
            { "2 dibotada 3 dibotada 4", 24 }, // Левая ассоциативность: (2*3)*4

            // Деление
            { "10 poopaye 2", 5 },
            { "15 poopaye 3", 5 },
            { "flavuk 10 poopaye 2", -5 }, // Деление с унарным минусом
            { "20 poopaye 4 poopaye 2", 2 }, // Левая ассоциативность: (20/4)/2

            // Остаток
            { "10 pado 3", 1 },
            { "15 pado 4", 3 },
            { "flavuk 10 pado 3", -1 }, // Остаток с унарным минусом
            { "20 pado 6 pado 3", 2 }, // Левая ассоциативность: (20%6)%3

            // Смешанные операции
            { "2 dibotada 3 poopaye 2", 3 }, // 2*3/2 = 3
            { "10 poopaye 2 dibotada 3", 15 }, // 10/2*3 = 15
            { "2 dibotada 3 pado 2", 0 }, // 2*3%2 = 6%2 = 0
            { "10 dibotada 2 poopaye 4 pado 3", 2 }, // 10*2/4%3 = 20/4%3 = 5%3 = 2
        };
    }

    /// <summary>
    /// Тестовые данные для сложения и вычитания.
    /// </summary>
    public static TheoryData<string, int> GetAdditiveExpressionData()
    {
        return new TheoryData<string, int>
        {
            // Сложение
            { "2 melomo 3", 5 },
            { "10 melomo 20", 30 },
            { "melomo 2 melomo 3", 5 }, // Сложение с унарным плюсом
            { "1 melomo 2 melomo 3", 6 }, // Левая ассоциативность: (1+2)+3

            // Вычитание
            { "10 flavuk 3", 7 },
            { "20 flavuk 5", 15 },
            { "flavuk 10 flavuk 3", -13 }, // Вычитание с унарным минусом: (-10)-3 = -13
            { "10 flavuk 3 flavuk 2", 5 }, // Левая ассоциативность: (10-3)-2

            // Смешанные операции
            { "10 melomo 5 flavuk 3", 12 }, // 10+5-3 = 12
            { "20 flavuk 5 melomo 3", 18 }, // 20-5+3 = 18
            { "1 melomo 2 flavuk 3 melomo 4", 4 }, // 1+2-3+4 = 4
        };
    }

    /// <summary>
    /// Тестовые данные для операторов сравнения.
    /// </summary>
    public static TheoryData<string, int> GetRelationalExpressionData()
    {
        return new TheoryData<string, int>
        {
            // Меньше
            { "5 la 10", 1 }, // true
            { "10 la 5", 0 }, // false
            { "2 melomo 3 la 10", 1 }, // Сравнение с выражениями: 5 < 10 = true

            // Меньше или равно
            { "5 la con 5", 1 }, // true
            { "5 la con 10", 1 }, // true
            { "10 la con 5", 0 }, // false
            { "2 melomo 3 la con 10", 1 }, // Сравнение с выражениями: 5 <= 10 = true

            // Больше
            { "10 looka too 5", 1 }, // true
            { "5 looka too 10", 0 }, // false
            { "10 looka too 2 melomo 3", 1 }, // Сравнение с выражениями: 10 > 5 = true

            // Больше или равно
            { "10 looka too con 10", 1 }, // true
            { "10 looka too con 5", 1 }, // true
            { "5 looka too con 10", 0 }, // false
            { "10 looka too con 2 melomo 3", 1 }, // Сравнение с выражениями: 10 >= 5 = true

            // Цепочки сравнений (левая ассоциативность)
            { "5 la 10 la 15", 1 }, // (5<10)<15 = (1)<15 = 1<15 = true = 1
        };
    }

    /// <summary>
    /// Тестовые данные для равенства и неравенства.
    /// </summary>
    public static TheoryData<string, int> GetEqualityExpressionData()
    {
        return new TheoryData<string, int>
        {
            // Равенство
            { "5 con 5", 1 }, // true
            { "5 con 10", 0 }, // false
            { "da con da", 1 }, // true (оба 1)
            { "da con no", 0 }, // false
            { "2 melomo 3 con 5", 1 }, // Равенство с выражениями: 5 == 5 = true

            // Неравенство
            { "5 nocon 10", 1 }, // true
            { "5 nocon 5", 0 }, // false
            { "da nocon no", 1 }, // true
            { "da nocon da", 0 }, // false
            { "2 melomo 3 nocon 10", 1 }, // Неравенство с выражениями: 5 != 10 = true

            // Цепочки равенств (левая ассоциативность)
            { "5 con 5 con 5", 0 }, // (5==5)==5 = (1)==5 = false = 0
            { "5 con 5 nocon 0", 1 }, // (5==5)!=0 = (1)!=0 = true = 1
        };
    }

    /// <summary>
    /// Тестовые данные для логического И.
    /// </summary>
    public static TheoryData<string, int> GetLogicalAndExpressionData()
    {
        return new TheoryData<string, int>
        {
            { "da tropa da", 1 }, // true && true = true
            { "da tropa no", 0 }, // true && false = false
            { "no tropa da", 0 }, // false && true = false
            { "no tropa no", 0 }, // false && false = false
            { "da tropa da tropa da", 1 }, // Левая ассоциативность
        };
    }

    /// <summary>
    /// Тестовые данные для логического ИЛИ.
    /// </summary>
    public static TheoryData<string, int> GetLogicalOrExpressionData()
    {
        return new TheoryData<string, int>
        {
            { "da bo-ca da", 1 }, // true || true = true
            { "da bo-ca no", 1 }, // true || false = true
            { "no bo-ca da", 1 }, // false || true = true
            { "no bo-ca no", 0 }, // false || false = false
            { "no bo-ca no bo-ca da", 1 }, // Левая ассоциативность
        };
    }

    /// <summary>
    /// Тестовые данные для логического НЕ.
    /// </summary>
    public static TheoryData<string, int> GetLogicalNotExpressionData()
    {
        return new TheoryData<string, int>
        {
            { "makoroni da", 0 }, // !true = false
            { "makoroni no", 1 }, // !false = true
            { "makoroni (5 con 5)", 0 }, // Логическое НЕ от выражения: !(5==5) = !true = false
            { "makoroni da tropa no", 0 }, // Логическое НЕ с логическим И: !(true&&false) = !false = true
            { "makoroni da bo-ca no", 0 }, // Логическое НЕ с логическим ИЛИ: !(true||false) = !true = false
            { "makoroni makoroni da", 1 }, // Двойное логическое НЕ: !!true = true (правая ассоциативность)
        };
    }

    /// <summary>
    /// Тестовые данные для проверки приоритета операторов.
    /// </summary>
    public static TheoryData<string, int> GetOperatorPrecedenceData()
    {
        return new TheoryData<string, int>
        {
            // Скобки имеют наивысший приоритет
            { "(2 melomo 3) dibotada 4", 20 }, // (2+3)*4 = 20

            // Унарные операторы выше бинарных
            { "melomo 5 melomo 3", 8 }, // (+5)+3 = 8
            { "flavuk 5 dibotada 3", -15 }, // (-5)*3 = -15

            // Возведение в степень выше умножения
            { "2 dibotada 3 beedo 2", 18 }, // 2*(3^2) = 18
            { "2 melomo 3 beedo 2", 11 }, // 2+(3^2) = 11

            // Умножение выше сложения
            { "2 melomo 3 dibotada 4", 14 }, // 2+(3*4) = 14
            { "10 melomo 5 poopaye 2", 12 }, // 10+(5/2) = 12

            // Сложение выше сравнения
            { "5 la 3 melomo 2", 0 }, // 5<(3+2) = 5<5 = false = 0

            // Логическое И выше логического ИЛИ
            { "da tropa no bo-ca da", 1 }, // (da&&no)||da = false||true = true
            { "no tropa da bo-ca no", 0 }, // (no&&da)||no = false||false = false
            { "da bo-ca no melomo 2", 1 },

            // Комплексные выражения
            { "muak(2 melomo 3) dibotada miniboss(4, 5)", 20 }, // abs(2+3)*min(4,5) = 5*4 = 20
            { "belloPi dibotada 2 melomo belloE", 9 }, // 3.14*2+2.71 = 6.28+2.71 = 8.99 -> 8 при int (округление)
        };
    }

    /// <summary>
    /// Тестовые данные для обработки ошибок.
    /// </summary>
    public static TheoryData<string, Type> GetErrorHandlingData()
    {
        return new TheoryData<string, Type>
        {
            // Незакрытая скобка
            { "(2 melomo 3", typeof(InvalidOperationException) },

            // Лишняя закрывающая скобка
            { "2 melomo 3)", typeof(InvalidOperationException) },

            // Неправильный порядок скобок
            { ")2 melomo 3(", typeof(InvalidOperationException) },

            // Отсутствие операнда после оператора
            { "2 melomo", typeof(InvalidOperationException) },

            // Пустое выражение в скобках
            { "()", typeof(InvalidOperationException) },

            // Отсутствие закрывающей скобки в вызове функции
            { "muak(5", typeof(InvalidOperationException) },

            // Отсутствие открывающей скобки в вызове функции
            { "muak 5)", typeof(InvalidOperationException) },

            // Деление на ноль
            { "10 poopaye 0", typeof(DivideByZeroException) },

            // Остаток от деления на ноль
            { "10 pado 0", typeof(DivideByZeroException) },

            // Неизвестная переменная
            { "x", typeof(InvalidOperationException) },
            { "x melomo 5", typeof(InvalidOperationException) },
        };
    }
}
