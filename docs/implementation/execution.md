# Модуль Execution - Подробное описание

## Содержание
1. [Обзор архитектуры](#обзор-архитектуры)
2. [Компоненты модуля](#компоненты-модуля)
3. [Принципы работы](#принципы-работы)
4. [Взаимодействие компонентов](#взаимодействие-компонентов)
5. [Используемые конструкции C#](#используемые-конструкции-c)
6. [Примеры выполнения](#примеры-выполнения)

---

## Обзор архитектуры

Модуль `Execution` реализует **интерпретатор** для выполнения программ на языке Minion#. Он принимает на вход абстрактное синтаксическое дерево (AST) и выполняет его, используя паттерн **Visitor** для обхода узлов дерева.

### Основные принципы проектирования:

1. **Разделение ответственности** - каждый класс отвечает за свою область
2. **Инверсия зависимостей** - использование интерфейсов для абстракции
3. **Стековый метод вычисления** - выражения вычисляются через стек операндов
4. **Управление областями видимости** - через стек областей (scope stack)

---

## Компоненты модуля

### 1. AstEvaluator - Вычислитель AST

**Файл:** `AstEvaluator.cs`

**Назначение:** Центральный компонент, который выполняет обход AST и вычисляет выражения, выполняя инструкции.

#### Архитектура

```csharp
public class AstEvaluator : IAstVisitor
{
    private readonly Context context;              // Управление областями видимости
    private readonly IEnvironment environment;      // Ввод/вывод
    private readonly Dictionary<string, FunctionDefinition> functions;  // Хранилище функций
    private Stack<decimal>? evaluationStack;       // Стек для вычисления выражений
    private bool isInsideFunction;                 // Флаг выполнения функции
}
```

#### Ключевые методы

##### EvaluateProgram
Выполняет программу в два этапа:
1. **Фаза объявлений** - регистрирует все константы и функции
2. **Фаза выполнения** - выполняет все инструкции

```csharp
public void EvaluateProgram(IReadOnlyList<AstNode> topLevelItems)
{
    // Сначала обрабатываем все объявления (константы и функции)
    foreach (AstNode item in topLevelItems)
    {
        if (item is Declaration declaration)
        {
            declaration.Accept(this);
        }
    }

    // Затем выполняем инструкции
    foreach (AstNode item in topLevelItems)
    {
        if (item is Statement statement)
        {
            statement.Accept(this);
        }
    }
}
```

**Почему два этапа?**
- Функции и константы должны быть доступны до их использования
- Это соответствует семантике языка, где объявления идут перед использованием

##### EvaluateExpression
Вычисляет выражение стековым методом:

```csharp
public decimal EvaluateExpression(Expression expression)
{
    Stack<decimal> stack = new Stack<decimal>();
    Stack<decimal>? savedStack = evaluationStack;
    evaluationStack = stack;

    try
    {
        expression.Accept(this);  // Рекурсивный обход дерева

        if (stack.Count != 1)
        {
            throw new InvalidOperationException(
                $"Ошибка вычисления выражения: стек должен содержать одно значение, получено {stack.Count}");
        }

        return stack.Pop();
    }
    finally
    {
        evaluationStack = savedStack;  // Восстановление предыдущего стека
    }
}
```

**Стековый метод вычисления:**
- Литералы и идентификаторы **пушат** значения в стек
- Операторы **извлекают** операнды из стека и **пушат** результат
- В конце в стеке остается одно значение - результат выражения

**Пример:** Выражение `2 + 3 * 4`
```
1. VisitNumberLiteral(2) → stack: [2]
2. VisitNumberLiteral(3) → stack: [2, 3]
3. VisitNumberLiteral(4) → stack: [2, 3, 4]
4. VisitBinaryExpression(*) → pop 4, pop 3, push 12 → stack: [2, 12]
5. VisitBinaryExpression(+) → pop 12, pop 2, push 14 → stack: [14]
6. Результат: 14
```

#### Обработка выражений

##### VisitNumberLiteral
Просто пушит значение в стек:
```csharp
public void VisitNumberLiteral(NumberLiteral number)
{
    evaluationStack.Push(number.Value);
}
```

##### VisitIdentifier
Ищет переменную в контексте и пушит её значение:
```csharp
public void VisitIdentifier(Identifier identifier)
{
    if (!context.TryGetVariable(identifier.Name, out decimal value))
    {
        throw new InvalidOperationException($"Неизвестная переменная: {identifier.Name}");
    }
    evaluationStack.Push(value);
}
```

##### VisitBinaryExpression
Вычисляет оба операнда, затем применяет оператор:
```csharp
public void VisitBinaryExpression(BinaryExpression binary)
{
    binary.Left.Accept(this);   // Вычисляем левый операнд
    binary.Right.Accept(this);   // Вычисляем правый операнд
    
    decimal right = evaluationStack.Pop();
    decimal left = evaluationStack.Pop();
    
    decimal result = binary.Operator switch
    {
        BinaryExpression.BinaryOperator.Add => left + right,
        BinaryExpression.BinaryOperator.Subtract => left - right,
        // ... другие операторы
    };
    
    evaluationStack.Push(result);
}
```

**Важно:** Операторы извлекаются в обратном порядке (right, left), так как стек работает по принципу LIFO.

##### VisitFunctionCall
Самый сложный метод - обрабатывает вызовы функций:

```csharp
public void VisitFunctionCall(FunctionCall call)
{
    // 1. Вычисляем все аргументы
    foreach (Expression arg in call.Arguments)
    {
        arg.Accept(this);
    }

    // 2. Собираем аргументы из стека (в обратном порядке)
    List<decimal> arguments = new List<decimal>();
    for (int i = call.Arguments.Count - 1; i >= 0; i--)
    {
        arguments.Add(evaluationStack.Pop());
    }
    arguments.Reverse();

    // 3. Проверяем встроенные функции
    if (IsBuiltinFunction(call.FunctionName))
    {
        result = BuiltinFunctions.Invoke(call.FunctionName, arguments);
    }
    else
    {
        // 4. Вызываем пользовательскую функцию
        // Создаем новую область видимости
        context.PushScope();
        
        // Присваиваем параметры
        for (int i = 0; i < function.Parameters.Count; i++)
        {
            context.TryDefineVariable(function.Parameters[i], arguments[i]);
        }
        
        // Выполняем тело функции
        try
        {
            function.Body.Accept(this);
        }
        catch (FunctionReturnException returnException)
        {
            result = returnException.ReturnValue;
        }
        finally
        {
            context.PopScope();
        }
    }
    
    evaluationStack.Push(result);
}
```

**Особенности:**
- Сохранение и восстановление стека вычислений (для вложенных вызовов)
- Создание новой области видимости для параметров функции
- Обработка возврата через исключение

#### Обработка инструкций

##### VisitVariableDeclaration
Объявляет переменную со значением 0:
```csharp
public void VisitVariableDeclaration(VariableDeclaration declaration)
{
    if (!context.TryDefineVariable(declaration.Name, 0m))
    {
        throw new InvalidOperationException(
            $"Переменная '{declaration.Name}' уже объявлена в этой области видимости");
    }
}
```

##### VisitAssignmentStatement
Вычисляет выражение и присваивает переменной:
```csharp
public void VisitAssignmentStatement(AssignmentStatement assignment)
{
    decimal value = EvaluateExpression(assignment.Expression);
    if (!context.TryAssignVariable(assignment.VariableName, value))
    {
        throw new InvalidOperationException($"Неизвестная переменная: {assignment.VariableName}");
    }
}
```

##### VisitInputStatement
Читает значение из окружения и создает/обновляет переменную:
```csharp
public void VisitInputStatement(InputStatement input)
{
    decimal value = environment.ReadNumber();
    
    // Пытаемся присвоить существующей переменной
    if (!context.TryAssignVariable(input.VariableName, value))
    {
        // Если переменная не существует, создаем её
        if (!context.TryDefineVariable(input.VariableName, value))
        {
            throw new InvalidOperationException($"Не удалось создать переменную: {input.VariableName}");
        }
    }
}
```

##### VisitOutputStatement
Вычисляет выражение и выводит результат:
```csharp
public void VisitOutputStatement(OutputStatement output)
{
    decimal value = EvaluateExpression(output.Expression);
    environment.WriteNumber(value);
}
```

##### VisitIfStatement
Вычисляет условие и выполняет соответствующий блок:
```csharp
public void VisitIfStatement(IfStatement ifStatement)
{
    decimal condition = EvaluateExpression(ifStatement.Condition);
    
    if (condition != 0)  // В Minion# 0 = false, остальное = true
    {
        ifStatement.ThenBlock.Accept(this);
    }
    else if (ifStatement.ElseBlock != null)
    {
        ifStatement.ElseBlock.Accept(this);
    }
}
```

**Обработка исключений:** `FunctionReturnException` пробрасывается наверх, чтобы не прерывать возврат из функции.

##### VisitWhileStatement
Цикл с проверкой условия:
```csharp
public void VisitWhileStatement(WhileStatement whileStatement)
{
    while (true)
    {
        decimal condition = EvaluateExpression(whileStatement.Condition);
        
        if (condition == 0)
        {
            break;
        }
        
        try
        {
            whileStatement.Body.Accept(this);
        }
        catch (FunctionReturnException)
        {
            throw;  // Пробрасываем исключение возврата
        }
    }
}
```

##### VisitReturnStatement
Выбрасывает специальное исключение для возврата:
```csharp
public void VisitReturnStatement(ReturnStatement returnStatement)
{
    if (!isInsideFunction)
    {
        throw new InvalidOperationException("'tank yu' может использоваться только внутри функции");
    }
    
    decimal returnValue = EvaluateExpression(returnStatement.Expression);
    throw new FunctionReturnException { ReturnValue = returnValue };
}
```

**Почему исключение?**
- Упрощает проброс возврата через вложенные блоки и циклы
- Не требует передачи флагов через все уровни вызовов
- Гарантирует правильную очистку областей видимости через `finally`

##### VisitBlock
Создает новую область видимости для блока:
```csharp
public void VisitBlock(Block block)
{
    context.PushScope();
    
    try
    {
        foreach (Statement statement in block.Statements)
        {
            statement.Accept(this);
        }
    }
    catch (FunctionReturnException)
    {
        throw;  // Пробрасываем исключение возврата
    }
    finally
    {
        context.PopScope();  // Гарантированная очистка области видимости
    }
}
```

**Важно:** `finally` гарантирует удаление области видимости даже при исключениях.

---

### 2. Context - Управление областями видимости

**Файл:** `Context.cs`

**Назначение:** Управляет стеком областей видимости (scopes) для переменных.

#### Архитектура

```csharp
public class Context
{
    private readonly Stack<Scope> scopes = new();
    
    public Context()
    {
        PushScope();  // Создаем глобальную область видимости
    }
}
```

#### Принцип работы

**Стек областей видимости:**
- Каждая область видимости (блок, функция) создает новый `Scope`
- При выходе из области видимости она удаляется из стека
- Поиск переменных идет от текущей области к глобальной (цепочка областей)

**Пример:**
```
Глобальная область: { x = 10 }
  ↓ PushScope()
Блок 1: { y = 20 }
  ↓ PushScope()
Блок 2: { z = 30 }
  ↓ Попытка прочитать x
  → Ищет в Блок 2: нет
  → Ищет в Блок 1: нет
  → Ищет в Глобальной: есть! x = 10
```

#### Ключевые методы

##### PushScope / PopScope
```csharp
public void PushScope()
{
    scopes.Push(new Scope());
}

public void PopScope()
{
    if (scopes.Count <= 1)
    {
        throw new InvalidOperationException("Нельзя удалить глобальную область видимости");
    }
    scopes.Pop();
}
```

**Защита:** Глобальная область видимости не может быть удалена.

##### TryGetVariable
Ищет переменную в цепочке областей видимости (от текущей к глобальной):
```csharp
public bool TryGetVariable(string name, out decimal value)
{
    foreach (Scope scope in scopes)  // Итерация от вершины стека
    {
        if (scope.TryGetVariable(name, out value))
        {
            return true;
        }
    }
    
    value = 0.0m;
    return false;
}
```

**Почему foreach по Stack?**
- `Stack<T>` реализует `IEnumerable<T>` в порядке LIFO
- Итерация идет от текущей области к глобальной (нужный порядок)

##### TryAssignVariable
Присваивает значение переменной в первой области видимости, где она найдена:
```csharp
public bool TryAssignVariable(string name, decimal value)
{
    foreach (Scope scope in scopes)
    {
        if (scope.TryAssignVariable(name, value))
        {
            return true;  // Переменная найдена и обновлена
        }
    }
    
    return false;  // Переменная не найдена ни в одной области
}
```

**Важно:** Присваивание ищет переменную в цепочке, но обновляет её в той области, где она была объявлена.

##### TryDefineVariable
Объявляет переменную в текущей (верхней) области видимости:
```csharp
public bool TryDefineVariable(string name, decimal value)
{
    if (scopes.Count == 0)
    {
        return false;
    }
    
    return scopes.Peek().TryDefineVariable(name, value);
}
```

**Почему только в текущей области?**
- Это соответствует семантике языков программирования
- Переменная должна быть объявлена в той области, где она используется
- Предотвращает случайное переопределение переменных из внешних областей

---

### 3. Scope - Область видимости

**Файл:** `Scope.cs`

**Назначение:** Хранит переменные одной области видимости.

#### Архитектура

```csharp
public class Scope
{
    private readonly Dictionary<string, decimal> variables = [];
}
```

**Почему Dictionary?**
- O(1) доступ по имени переменной
- Простая проверка существования через `ContainsKey`
- Эффективное хранение пар имя-значение

#### Ключевые методы

##### TryGetVariable
Читает значение переменной:
```csharp
public bool TryGetVariable(string name, out decimal value)
{
    if (variables.TryGetValue(name, out decimal v))
    {
        value = v;
        return true;
    }
    
    value = 0.0m;
    return false;
}
```

##### TryAssignVariable
Присваивает значение существующей переменной:
```csharp
public bool TryAssignVariable(string name, decimal value)
{
    if (variables.ContainsKey(name))
    {
        variables[name] = value;
        return true;
    }
    
    return false;  // Переменная не объявлена в этой области
}
```

**Важно:** Присваивание работает только для уже объявленных переменных.

##### TryDefineVariable
Объявляет новую переменную:
```csharp
public bool TryDefineVariable(string name, decimal value)
{
    if (variables.ContainsKey(name))
    {
        return false;  // Переменная уже объявлена
    }
    
    variables[name] = value;
    return true;
}
```

**Проверка дубликатов:** Предотвращает повторное объявление в одной области видимости.

---

### 4. IEnvironment - Абстракция ввода/вывода

**Файл:** `IEnvironment.cs`

**Назначение:** Интерфейс для изоляции операций ввода/вывода от логики выполнения.

#### Архитектура

```csharp
public interface IEnvironment
{
    void WriteNumber(decimal result);
    decimal ReadNumber();
}
```

**Почему интерфейс?**
- **Разделение ответственности:** логика выполнения не зависит от конкретной реализации I/O
- **Тестируемость:** можно подменить реализацию для тестов
- **Гибкость:** легко добавить новые реализации (файловый ввод/вывод, сетевой и т.д.)

#### Реализации

##### ConsoleEnvironment
**Файл:** `ConsoleEnvironment.cs`

Реальная реализация для консольного ввода/вывода:

```csharp
public class ConsoleEnvironment : IEnvironment
{
    public void WriteNumber(decimal value)
    {
        Console.WriteLine(value.ToString("0.#####", CultureInfo.InvariantCulture));
    }
    
    public decimal ReadNumber()
    {
        string? input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new InvalidOperationException("Input is empty");
        }
        
        if (!decimal.TryParse(input, NumberStyles.Number, 
            CultureInfo.InvariantCulture, out decimal number))
        {
            throw new FormatException($"Invalid number format: '{input}'");
        }
        
        return number;
    }
}
```

**Особенности:**
- Использует `CultureInfo.InvariantCulture` для консистентного форматирования
- Обрабатывает ошибки парсинга
- Форматирование с ограничением знаков после запятой

##### FakeEnvironment
**Файл:** `FakeEnvironment.cs`

Реализация для тестирования:

```csharp
public class FakeEnvironment : IEnvironment
{
    private readonly List<decimal> results = [];
    private readonly Queue<decimal> inputs = new();
    
    public IReadOnlyList<decimal> Results => results;
    
    public void AddInput(decimal value)
    {
        inputs.Enqueue(value);
    }
    
    public void WriteNumber(decimal result)
    {
        results.Add(result);
    }
    
    public decimal ReadNumber()
    {
        if (inputs.Count == 0)
        {
            throw new InvalidOperationException("Нет входных значений для чтения");
        }
        
        return inputs.Dequeue();
    }
}
```

**Особенности:**
- Хранит результаты вывода в списке (для проверки в тестах)
- Использует очередь для входных данных (FIFO)
- Не выполняет реальный I/O

---

### 5. BuiltinFunctions - Встроенные функции

**Файл:** `BuiltinFunctions.cs`

**Назначение:** Реализация встроенных функций языка Minion#.

#### Архитектура

```csharp
public static class BuiltinFunctions
{
    public static decimal Invoke(string name, List<decimal> arguments)
    {
        return name switch
        {
            "muak" => Muak(arguments),
            "miniboss" => Miniboss(arguments),
            "bigboss" => Bigboss(arguments),
            _ => throw new ArgumentException($"Неизвестная функция: {name}", nameof(name))
        };
    }
}
```

**Почему статический класс?**
- Встроенные функции не требуют состояния
- Удобный способ группировки связанных функций
- Простой вызов без создания экземпляра

#### Реализации функций

##### Muak (абсолютное значение)
```csharp
private static decimal Muak(List<decimal> arguments)
{
    if (arguments.Count != 1)
    {
        throw new ArgumentException("Функция 'muak' принимает 1 аргумент", nameof(arguments));
    }
    
    return Math.Abs(arguments[0]);
}
```

##### Miniboss (минимум)
```csharp
private static decimal Miniboss(List<decimal> arguments)
{
    if (arguments.Count == 0)
    {
        throw new ArgumentException("Функция 'miniboss' требует хотя бы один аргумент", nameof(arguments));
    }
    
    return arguments.Min();  // LINQ метод
}
```

##### Bigboss (максимум)
```csharp
private static decimal Bigboss(List<decimal> arguments)
{
    if (arguments.Count == 0)
    {
        throw new ArgumentException("Функция 'bigboss' требует хотя бы один аргумент", nameof(arguments));
    }
    
    return arguments.Max();  // LINQ метод
}
```

**Особенности:**
- Валидация количества аргументов
- Использование LINQ для удобных операций
- Понятные сообщения об ошибках

---

### 6. FunctionReturnException - Исключение для возврата

**Файл:** `AstEvaluator.cs` (внутренний класс)

**Назначение:** Специальное исключение для реализации возврата из функции.

#### Архитектура

```csharp
internal class FunctionReturnException : Exception
{
    public decimal ReturnValue { get; init; }
}
```

**Почему исключение для возврата?**
- Упрощает проброс возврата через вложенные блоки и циклы
- Не требует передачи флагов через все уровни вызовов
- Гарантирует правильную очистку областей видимости через `finally`

**Пример использования:**
```csharp
// В VisitReturnStatement
throw new FunctionReturnException { ReturnValue = returnValue };

// В VisitFunctionCall
try
{
    function.Body.Accept(this);
}
catch (FunctionReturnException returnException)
{
    result = returnException.ReturnValue;
    hasReturned = true;
}
```

**Особенности:**
- `init`-only property - значение устанавливается только при создании
- `internal` класс - используется только внутри модуля Execution
- Наследуется от `Exception` для совместимости с обработкой исключений

---

## Принципы работы

### 1. Стековый метод вычисления выражений

**Алгоритм:**
1. Создается стек для хранения промежуточных значений
2. Рекурсивный обход дерева выражения:
   - Литералы и идентификаторы → push в стек
   - Операторы → pop операнды, вычислить, push результат
3. В конце в стеке остается одно значение - результат

**Преимущества:**
- Естественно для постфиксной записи
- Упрощает обработку вложенных выражений
- Не требует явного управления приоритетами операторов

**Пример:**
```
Выражение: (2 + 3) * 4

1. VisitNumberLiteral(2) → stack: [2]
2. VisitNumberLiteral(3) → stack: [2, 3]
3. VisitBinaryExpression(+) → pop 3, pop 2, push 5 → stack: [5]
4. VisitNumberLiteral(4) → stack: [5, 4]
5. VisitBinaryExpression(*) → pop 4, pop 5, push 20 → stack: [20]
6. Результат: 20
```

### 2. Управление областями видимости

**Алгоритм:**
1. При входе в блок/функцию → `PushScope()` (создается новая область)
2. При выходе → `PopScope()` (удаляется область)
3. Поиск переменных → от текущей области к глобальной
4. Присваивание → в первой области, где найдена переменная
5. Объявление → только в текущей области

**Пример:**
```
Глобальная: { x = 10 }
  ↓
Функция f(y):
  Параметры: { y = 5 }
    ↓
  Блок:
    Локальные: { z = 3 }
    Чтение x → ищет в Блок → нет
              → ищет в Функция → нет
              → ищет в Глобальная → есть! x = 10
    Присваивание x = 20 → обновляет в Глобальная
    Объявление x = 30 → создает в Блок (затеняет глобальную)
```

### 3. Вызов функций

**Алгоритм:**
1. Вычисление аргументов (каждый пушит значение в стек)
2. Сбор аргументов из стека (в обратном порядке)
3. Проверка типа функции (встроенная/пользовательская)
4. Для пользовательских функций:
   - Сохранение текущего стека вычислений
   - Создание новой области видимости
   - Привязка параметров к аргументам
   - Выполнение тела функции
   - Обработка возврата через исключение
   - Восстановление стека и области видимости

**Особенности:**
- Сохранение стека необходимо для вложенных вызовов функций
- Новая область видимости изолирует параметры функции
- Исключение для возврата упрощает проброс через вложенные блоки

### 4. Обработка ошибок

**Типы исключений:**
- `InvalidOperationException` - логические ошибки (неизвестная переменная, функция и т.д.)
- `DivideByZeroException` - деление на ноль
- `FunctionReturnException` - возврат из функции (не ошибка, механизм control flow)
- `ArgumentException` - неверные аргументы функций

**Стратегия:**
- Проверка на этапе выполнения (runtime checks)
- Понятные сообщения об ошибках с контекстом
- Гарантированная очистка ресурсов через `finally`

---

## Взаимодействие компонентов

### Диаграмма взаимодействия

```
┌─────────────┐
│ Interpreter │
└──────┬──────┘
       │ создает
       ├─→ Context (управление областями видимости)
       └─→ IEnvironment (ввод/вывод)
              │
              │ передает в
              ↓
       ┌──────────────┐
       │ AstEvaluator │
       └──────┬───────┘
              │ использует
              ├─→ Context
              │   ├─→ Stack<Scope> (стек областей)
              │   └─→ Scope (область видимости)
              │       └─→ Dictionary<string, decimal> (переменные)
              │
              ├─→ IEnvironment
              │   ├─→ ConsoleEnvironment (реальный I/O)
              │   └─→ FakeEnvironment (тестовый I/O)
              │
              ├─→ BuiltinFunctions (встроенные функции)
              │
              └─→ Stack<decimal> (стек вычислений)
```

### Последовательность выполнения программы

```
1. Interpreter.Execute(code)
   │
   ├─→ Parser.ParseProgram(code) → AST
   │
   └─→ AstEvaluator.EvaluateProgram(AST)
       │
       ├─→ Фаза 1: Объявления
       │   ├─→ VisitConstDeclaration → Context.TryDefineVariable
       │   └─→ VisitFunctionDefinition → сохранение в Dictionary
       │
       └─→ Фаза 2: Выполнение
           ├─→ VisitVariableDeclaration → Context.TryDefineVariable
           ├─→ VisitAssignmentStatement → EvaluateExpression → Context.TryAssignVariable
           ├─→ VisitInputStatement → IEnvironment.ReadNumber → Context.TryAssignVariable
           ├─→ VisitOutputStatement → EvaluateExpression → IEnvironment.WriteNumber
           ├─→ VisitIfStatement → EvaluateExpression → VisitBlock
           ├─→ VisitWhileStatement → EvaluateExpression → VisitBlock (цикл)
           └─→ VisitFunctionCall → EvaluateExpression (аргументы) → VisitBlock (тело)
```

### Пример полного выполнения

**Код:**
```minion
bello!
poop x naidu!
x lumai 10 naidu!
tulalilloo ti amo x naidu!
```

**Выполнение:**
```
1. EvaluateProgram
   ├─ Фаза объявлений: нет объявлений
   └─ Фаза выполнения:
       ├─ VisitVariableDeclaration("x")
       │   └─ Context.TryDefineVariable("x", 0) → Scope: {x = 0}
       │
       ├─ VisitAssignmentStatement("x", Expression(10))
       │   ├─ EvaluateExpression(10)
       │   │   └─ VisitNumberLiteral(10) → stack: [10]
       │   └─ Context.TryAssignVariable("x", 10) → Scope: {x = 10}
       │
       └─ VisitOutputStatement(Expression("x"))
           ├─ EvaluateExpression("x")
           │   └─ VisitIdentifier("x")
           │       └─ Context.TryGetVariable("x") → 10 → stack: [10]
           └─ IEnvironment.WriteNumber(10) → Console: "10"
```

---

## Используемые конструкции C#

### 1. Nullable Reference Types

```csharp
private Stack<decimal>? evaluationStack;
```

**Назначение:** Указывает, что поле может быть `null`. Используется для опционального стека вычислений.

**Почему:** Стек создается только при вычислении выражений, в остальное время может быть `null`.

### 2. Pattern Matching

#### Switch Expressions
```csharp
decimal result = binary.Operator switch
{
    BinaryExpression.BinaryOperator.Add => left + right,
    BinaryExpression.BinaryOperator.Subtract => left - right,
    _ => throw new InvalidOperationException($"Неизвестный оператор: {binary.Operator}")
};
```

**Преимущества:**
- Компактный синтаксис
- Проверка полноты на этапе компиляции
- Выражение возвращает значение

#### Type Pattern
```csharp
if (item is Declaration declaration)
{
    declaration.Accept(this);
}
```

**Преимущества:**
- Одновременная проверка типа и приведение
- Безопасное приведение без исключений

### 3. Collection Expressions (C# 12)

```csharp
private readonly Dictionary<string, decimal> variables = [];
private readonly List<decimal> results = [];
```

**Преимущества:**
- Компактный синтаксис инициализации
- Автоматический вывод типа коллекции

### 4. Init-Only Properties

```csharp
public decimal ReturnValue { get; init; }
```

**Назначение:** Свойство можно установить только при инициализации объекта.

**Использование:**
```csharp
throw new FunctionReturnException { ReturnValue = returnValue };
```

### 5. LINQ методы

```csharp
return arguments.Min();
return arguments.Max();
```

**Преимущества:**
- Удобные операции над коллекциями
- Читаемый код

### 6. Out-параметры

```csharp
public bool TryGetVariable(string name, out decimal value)
{
    if (variables.TryGetValue(name, out decimal v))
    {
        value = v;
        return true;
    }
    value = 0.0m;
    return false;
}
```

**Преимущества:**
- Возврат нескольких значений (успех + значение)
- Безопасный способ возврата значений без исключений

### 7. Generic Collections

```csharp
Dictionary<string, FunctionDefinition> functions
Stack<Scope> scopes
Stack<decimal> evaluationStack
List<decimal> arguments
Queue<decimal> inputs
```

**Преимущества:**
- Типобезопасность
- Эффективные структуры данных
- Богатый API

### 8. Интерфейсы и полиморфизм

```csharp
public interface IEnvironment
{
    void WriteNumber(decimal result);
    decimal ReadNumber();
}
```

**Преимущества:**
- Абстракция от конкретной реализации
- Легкая замена реализации (Dependency Injection)
- Упрощение тестирования

### 9. Dependency Injection

```csharp
public AstEvaluator(Context context, IEnvironment environment)
{
    this.context = context ?? throw new ArgumentNullException(nameof(context));
    this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
}
```

**Преимущества:**
- Зависимости передаются извне
- Легкая замена зависимостей
- Упрощение тестирования

### 10. Exception Handling для Control Flow

```csharp
try
{
    function.Body.Accept(this);
}
catch (FunctionReturnException returnException)
{
    result = returnException.ReturnValue;
}
```

**Использование:** Исключения для возврата из функции упрощают проброс через вложенные блоки.

**Альтернативы:**
- Передача флагов через все уровни (сложно)
- Использование специального типа результата (избыточно для простого случая)

### 11. Try-Finally для гарантированной очистки

```csharp
try
{
    context.PushScope();
    // ... выполнение ...
}
finally
{
    context.PopScope();  // Гарантированная очистка
}
```

**Преимущества:**
- Гарантированная очистка ресурсов даже при исключениях
- Предотвращение утечек памяти
- Корректное управление областями видимости

### 12. Null-Coalescing Operator

```csharp
_environment = environment ?? new ConsoleEnvironment();
```

**Преимущества:**
- Компактный синтаксис для значений по умолчанию
- Удобная обработка null

### 13. ArgumentNullException

```csharp
this.context = context ?? throw new ArgumentNullException(nameof(context));
```

**Преимущества:**
- Явная проверка null-аргументов
- Понятные сообщения об ошибках
- Раннее обнаружение ошибок

### 14. IReadOnlyList / IReadOnlyCollection

```csharp
public void EvaluateProgram(IReadOnlyList<AstNode> topLevelItems)
public IReadOnlyList<decimal> Results => results;
```

**Преимущества:**
- Защита от модификации коллекции
- Явное указание намерения (только чтение)
- Безопасность при передаче коллекций

---

## Примеры выполнения

### Пример 1: Простое выражение

**Код:**
```minion
bello!
tulalilloo ti amo 2 + 3 * 4 naidu!
```

**AST:**
```
OutputStatement
  └─ BinaryExpression(+)
      ├─ NumberLiteral(2)
      └─ BinaryExpression(*)
          ├─ NumberLiteral(3)
          └─ NumberLiteral(4)
```

**Выполнение:**
```
1. VisitOutputStatement
   └─ EvaluateExpression(BinaryExpression(+))
       ├─ VisitBinaryExpression(+)
       │   ├─ VisitNumberLiteral(2) → stack: [2]
       │   └─ VisitBinaryExpression(*)
       │       ├─ VisitNumberLiteral(3) → stack: [2, 3]
       │       └─ VisitNumberLiteral(4) → stack: [2, 3, 4]
       │       └─ Pop 4, Pop 3, Push 12 → stack: [2, 12]
       │   └─ Pop 12, Pop 2, Push 14 → stack: [14]
       └─ WriteNumber(14) → Console: "14"
```

### Пример 2: Условное выполнение

**Код:**
```minion
bello!
poop x naidu!
x lumai 10 naidu!
bi do x > 5 stopa
    tulalilloo ti amo x naidu!
stopa
```

**Выполнение:**
```
1. VisitVariableDeclaration("x") → Scope: {x = 0}
2. VisitAssignmentStatement("x", 10) → Scope: {x = 10}
3. VisitIfStatement
   ├─ EvaluateExpression(x > 5)
   │   └─ VisitBinaryExpression(>)
   │       ├─ VisitIdentifier("x") → 10 → stack: [10]
   │       └─ VisitNumberLiteral(5) → stack: [10, 5]
   │       └─ Pop 5, Pop 10, 10 > 5 = true → stack: [1]
   │   └─ Результат: 1 (true)
   └─ VisitBlock (ThenBlock)
       └─ VisitOutputStatement("x")
           └─ WriteNumber(10) → Console: "10"
```

### Пример 3: Цикл

**Код:**
```minion
bello!
poop i naidu!
i lumai 0 naidu!
kemari i < 5 stopa
    tulalilloo ti amo i naidu!
    i lumai i + 1 naidu!
stopa
```

**Выполнение:**
```
1. VisitVariableDeclaration("i") → Scope: {i = 0}
2. VisitAssignmentStatement("i", 0) → Scope: {i = 0}
3. VisitWhileStatement
   ├─ Итерация 1: i < 5 → true → Output(0) → i = 1
   ├─ Итерация 2: i < 5 → true → Output(1) → i = 2
   ├─ Итерация 3: i < 5 → true → Output(2) → i = 3
   ├─ Итерация 4: i < 5 → true → Output(3) → i = 4
   ├─ Итерация 5: i < 5 → true → Output(4) → i = 5
   └─ Итерация 6: i < 5 → false → выход
```

### Пример 4: Функция

**Код:**
```minion
bello!
function add(a, b) stopa
    tank yu a + b naidu!
stopa
poop result naidu!
result lumai add(5, 3) naidu!
tulalilloo ti amo result naidu!
```

**Выполнение:**
```
1. Фаза объявлений:
   └─ VisitFunctionDefinition("add")
       └─ functions["add"] = FunctionDefinition

2. Фаза выполнения:
   ├─ VisitVariableDeclaration("result") → Scope: {result = 0}
   ├─ VisitAssignmentStatement("result", FunctionCall("add", [5, 3]))
   │   └─ VisitFunctionCall("add")
   │       ├─ EvaluateExpression(5) → stack: [5]
   │       ├─ EvaluateExpression(3) → stack: [5, 3]
   │       ├─ Сбор аргументов: [5, 3]
   │       ├─ Context.PushScope() → новая область
   │       ├─ TryDefineVariable("a", 5) → Scope: {a = 5}
   │       ├─ TryDefineVariable("b", 3) → Scope: {a = 5, b = 3}
   │       ├─ VisitBlock (тело функции)
   │       │   └─ VisitReturnStatement(a + b)
   │       │       └─ EvaluateExpression(a + b) → 8
   │       │       └─ throw FunctionReturnException { ReturnValue = 8 }
   │       ├─ catch FunctionReturnException → result = 8
   │       └─ Context.PopScope() → удаление области функции
   │   └─ Scope: {result = 8}
   └─ VisitOutputStatement("result") → Console: "8"
```

### Пример 5: Вложенные области видимости

**Код:**
```minion
bello!
poop x naidu!
x lumai 10 naidu!
stopa
    poop x naidu!
    x lumai 20 naidu!
    tulalilloo ti amo x naidu!
stopa
tulalilloo ti amo x naidu!
```

**Выполнение:**
```
1. VisitVariableDeclaration("x") → Глобальная: {x = 0}
2. VisitAssignmentStatement("x", 10) → Глобальная: {x = 10}
3. VisitBlock
   ├─ Context.PushScope() → новая область
   ├─ VisitVariableDeclaration("x") → Блок: {x = 0} (затеняет глобальную)
   ├─ VisitAssignmentStatement("x", 20) → Блок: {x = 20}
   ├─ VisitOutputStatement("x")
   │   └─ TryGetVariable("x") → ищет в Блок → есть! x = 20
   │   └─ Console: "20"
   └─ Context.PopScope() → удаление области блока
4. VisitOutputStatement("x")
   └─ TryGetVariable("x") → ищет в Глобальная → есть! x = 10
   └─ Console: "10"
```

---

## Заключение

Модуль `Execution` представляет собой хорошо структурированный интерпретатор, использующий современные возможности C# и проверенные паттерны проектирования. Ключевые особенности:

1. **Стековый метод вычисления** - эффективный и понятный способ вычисления выражений
2. **Управление областями видимости** - корректная реализация scoping через стек областей
3. **Разделение ответственности** - каждый компонент отвечает за свою область
4. **Тестируемость** - использование интерфейсов позволяет легко тестировать компоненты
5. **Обработка ошибок** - понятные сообщения и гарантированная очистка ресурсов

Модуль демонстрирует профессиональный подход к реализации интерпретатора с учетом всех аспектов выполнения программ.

