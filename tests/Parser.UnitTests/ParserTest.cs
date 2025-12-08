using Parser;
using Xunit;

namespace Parser.UnitTests;

public class ParserTest
{
    [Theory]
    [MemberData(nameof(GetPrimaryExpressionData))]
    public void ParsePrimaryExpressionTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetIdentifierData))]
    public void ParseIdentifierTest(string code, Dictionary<string, decimal> variables, int expected)
    {
        AssertEvaluated(code, expected, variables);
    }

    [Theory]
    [MemberData(nameof(GetUnaryOperatorData))]
    public void ParseUnaryOperatorTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetUnaryOperatorWithVariablesData))]
    public void ParseUnaryOperatorWithVariablesTest(string code, Dictionary<string, decimal> variables, int expected)
    {
        AssertEvaluated(code, expected, variables);
    }

    [Theory]
    [MemberData(nameof(GetPowerExpressionData))]
    public void ParsePowerExpressionTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetMultiplicativeExpressionData))]
    public void ParseMultiplicativeExpressionTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetAdditiveExpressionData))]
    public void ParseAdditiveExpressionTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetRelationalExpressionData))]
    public void ParseRelationalExpressionTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetEqualityExpressionData))]
    public void ParseEqualityExpressionTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetLogicalAndExpressionData))]
    public void ParseLogicalAndExpressionTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetLogicalOrExpressionData))]
    public void ParseLogicalOrExpressionTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetLogicalNotExpressionData))]
    public void ParseLogicalNotExpressionTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetOperatorPrecedenceData))]
    public void ParseOperatorPrecedenceTest(string code, int expected)
    {
        AssertEvaluated(code, expected);
    }

    [Theory]
    [MemberData(nameof(GetErrorHandlingData))]
    public void ParseErrorHandlingTest(string code, Type expectedExceptionType)
    {
        AssertThrows(code, expectedExceptionType);
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

            // Идентификатор с подчёркиванием
            { "_temp", new Dictionary<string, decimal> { { "_temp", 5 } }, 5 },

            // Идентификатор с цифрами
            { "level2", new Dictionary<string, decimal> { { "level2", 2 } }, 2 },

            // Идентификатор в выражении
            { "x melomo y", new Dictionary<string, decimal> { { "x", 5 }, { "y", 3 } }, 8 },
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

            // Логические литералы
            { "da", 1 },
            { "no", 0 },

            // Константы
            { "belloPi", 3 }, // 3.14159... -> 3 при приведении к int

            // Скобки
            { "(2 melomo 3)", 5 }, // Выражение с операцией в скобках
            { "((42))", 42 }, // Вложенные скобки

            // Вызов функции
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
            { "melomo muak(5)", 5 }, // Унарный плюс перед вызовом функции

            // Унарный минус
            { "flavuk 5", -5 },
            { "flavuk (2 melomo 3)", -5 }, // Унарный минус перед выражением в скобках
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

            // Унарный минус перед переменной
            { "flavuk x", new Dictionary<string, decimal> { { "x", 5 } }, -5 },
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
            { "2 beedo 3 beedo 2", 512 }, // Правая ассоциативность: 2^(3^2) = 2^9 = 512
            { "flavuk 2 beedo 3", -8 }, // Возведение в степень с унарным минусом: (-2)^3 = -8
            { "2 beedo (3 beedo 2)", 512 }, // Возведение в степень в скобках: 2^(3^2) = 512
            { "flavuk 2 beedo 10", -1024 }, // -2^10
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
            { "flavuk 2 dibotada 3", -6 }, // Умножение с унарным минусом
            { "2 dibotada 3 dibotada 4", 24 }, // Левая ассоциативность: (2*3)*4

            // Деление
            { "10 poopaye 2", 5 },
            { "20 poopaye 4 poopaye 2", 2 }, // Левая ассоциативность: (20/4)/2

            // Остаток
            { "10 pado 3", 1 },
            { "flavuk 10 pado 3", -1 }, // Остаток с унарным минусом

            // Смешанные операции
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
            { "1 melomo 2 melomo 3", 6 }, // Левая ассоциативность: (1+2)+3

            // Вычитание
            { "10 flavuk 3", 7 },
            { "flavuk 10 flavuk 3", -13 }, // Вычитание с унарным минусом: (-10)-3 = -13
            { "10 flavuk 3 flavuk 2", 5 }, // Левая ассоциативность: (10-3)-2

            // Смешанные операции
            { "10 melomo 5 flavuk 3", 12 }, // 10+5-3 = 12
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

            // Меньше или равно
            { "5 la con 5", 1 }, // true

            // Больше
            { "10 looka too 5", 1 }, // true

            // Больше или равно
            { "10 looka too con 10", 1 }, // true

            // Цепочки сравнений (левая ассоциативность)
            { "5 la 10 la 15", 1 }, // (5<10)<15 = (1)<15 = 1<15 = true = 1

            // Смешанные сравнения
            { "1 la 2 looka too 3", 0 },
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
            { "da con no", 0 }, // false

            // Неравенство
            { "5 nocon 10", 1 }, // true

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
            { "da tropa no", 0 }, // true && false = false
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
            { "no bo-ca no", 0 }, // false || false = false
            { "no bo-ca no bo-ca da", 1 }, // Левая ассоциативность
            { "da bo-ca no", 1 }, // true || false = true
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
            { "flavuk 5 dibotada 3", -15 }, // (-5)*3 = -15

            // Возведение в степень выше умножения
            { "2 dibotada 3 beedo 2", 18 }, // 2*(3^2) = 18

            // Умножение выше сложения
            { "2 melomo 3 dibotada 4", 14 }, // 2+(3*4) = 14

            // Сложение выше сравнения
            { "5 la 3 melomo 2", 0 }, // 5<(3+2) = 5<5 = false = 0

            // Логическое И выше логического ИЛИ
            { "da tropa no bo-ca da", 1 }, // (da&&no)||da = false||true = true

            // Комплексные выражения
            { "muak(2 melomo 3) dibotada miniboss(4, 5)", 20 }, // abs(2+3)*min(4,5) = 5*4 = 20
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

            // Ошибки аргументов функции
            { "muak(,)", typeof(InvalidOperationException) },
            { "miniboss(a, b,)", typeof(InvalidOperationException) },

            // Деление на ноль
            { "10 poopaye 0", typeof(DivideByZeroException) },

            // Остаток от деления на ноль
            { "10 pado 0", typeof(DivideByZeroException) },

            // Неизвестная переменная
            { "x", typeof(InvalidOperationException) },
            { "x melomo 5", typeof(InvalidOperationException) },

            // Некорректные токены/выражения
            { "123abc", typeof(InvalidOperationException) },
            { "a invalid b", typeof(InvalidOperationException) },
            { "   ", typeof(InvalidOperationException) },

            // Строковые литералы пока не поддержаны
            { "!Hello!", typeof(NotImplementedException) },
        };
    }

    private static void AssertEvaluated(string code, int expected, Dictionary<string, decimal>? variables = null)
    {
        Parser parser = new Parser(new TokenStream(code), variables);
        int actual = parser.EvaluateExpression(code, variables);
        Assert.Equal(expected, actual);
    }

    private static void AssertThrows(string code, Type expectedExceptionType, Dictionary<string, decimal>? variables = null)
    {
        Parser parser = new Parser(new TokenStream(code), variables);
        Assert.Throws(expectedExceptionType, () => parser.EvaluateExpression(code, variables));
    }
}
