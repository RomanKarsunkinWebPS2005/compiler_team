using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;
using Parser;

namespace Execution;

/// <summary>
/// Вычислитель AST, реализующий интерфейс IAstVisitor.
/// Вычисляет выражения и выполняет инструкции, используя стековый метод для выражений.
/// </summary>
public class AstEvaluator : IAstVisitor
{
    private readonly Context context;
    private readonly IEnvironment environment;
    private readonly Dictionary<string, FunctionDefinition> functions = new();

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="AstEvaluator"/>.
    /// </summary>
    /// <param name="context">Контекст выполнения с областями видимости.</param>
    /// <param name="environment">Окружение для ввода/вывода.</param>
    public AstEvaluator(Context context, IEnvironment environment)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    /// <summary>
    /// Выполняет программу.
    /// </summary>
    /// <param name="program">Узел программы.</param>
    public void EvaluateProgram(Program program)
    {
        VisitProgram(program);
    }

    // Реализация IAstVisitor

    public void VisitProgram(Program program)
    {
        // Сначала обрабатываем все объявления (константы и функции)
        foreach (AstNode item in program.TopLevelItems)
        {
            if (item is Declaration declaration)
            {
                VisitDeclaration(declaration);
            }
        }

        // Затем выполняем инструкции
        foreach (AstNode item in program.TopLevelItems)
        {
            if (item is Statement statement)
            {
                VisitStatement(statement);
            }
        }
    }

    public void VisitFunctionDefinition(FunctionDefinition function)
    {
        // Сохраняем определение функции для последующего вызова
        functions[function.Name] = function;
    }

    public void VisitConstDeclaration(ConstDeclaration declaration)
    {
        decimal value = EvaluateExpression(declaration.Value);

        if (!context.TryDefineVariable(declaration.Name, value))
        {
            throw new InvalidOperationException($"Константа '{declaration.Name}' уже объявлена в этой области видимости");
        }
    }

    // Выражения возвращают decimal, обернутый в object?

    public object? VisitNumberLiteral(NumberLiteral number)
    {
        // Результат вычисления используется через EvaluateExpression
    }

    public void VisitIdentifier(Identifier identifier)
    {
        // Результат вычисления используется через EvaluateExpression
    }

    public void VisitConstant(Constant constant)
    {
        // Результат вычисления используется через EvaluateExpression
    }

    public void VisitBinaryExpression(BinaryExpression binary)
    {
        // Результат вычисления используется через EvaluateExpression
    }

    public void VisitUnaryExpression(UnaryExpression unary)
    {
        // Результат вычисления используется через EvaluateExpression
    }

    public void VisitFunctionCall(FunctionCall call)
    {
        // Результат вычисления используется через EvaluateExpression
    }

    // Инструкции

    public void VisitVariableDeclaration(VariableDeclaration declaration)
    {
        if (!context.TryDefineVariable(declaration.Name, 0m))
        {
            throw new InvalidOperationException($"Переменная '{declaration.Name}' уже объявлена в этой области видимости");
        }
    }

    public void VisitAssignmentStatement(AssignmentStatement assignment)
    {
        decimal value = EvaluateExpression(assignment.Expression);

        if (!context.TryAssignVariable(assignment.VariableName, value))
        {
            throw new InvalidOperationException($"Неизвестная переменная: {assignment.VariableName}");
        }
    }

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

    public void VisitOutputStatement(OutputStatement output)
    {
        decimal value = EvaluateExpression(output.Expression);
        environment.WriteNumber(value);
    }

    public void VisitIfStatement(IfStatement ifStatement)
    {
        decimal condition = EvaluateExpression(ifStatement.Condition);

        if (condition != 0)
        {
            VisitBlock(ifStatement.ThenBlock);
        }
        else if (ifStatement.ElseBlock != null)
        {
            VisitBlock(ifStatement.ElseBlock);
        }
    }

    public void VisitWhileStatement(WhileStatement whileStatement)
    {
        while (true)
        {
            decimal condition = EvaluateExpression(whileStatement.Condition);

            if (condition == 0)
            {
                break;
            }

            VisitBlock(whileStatement.Body);
        }
    }

    public void VisitReturnStatement(ReturnStatement returnStatement)
    {
        // Возврат из функции обрабатывается специальным образом
        // Пока что просто вычисляем выражение
        EvaluateExpression(returnStatement.Expression);
    }

    public void VisitBlock(Block block)
    {
        context.PushScope();

        try
        {
            foreach (Statement statement in block.Statements)
            {
                VisitStatement(statement);
            }
        }
        finally
        {
            context.PopScope();
        }
    }

    public void VisitExpressionStatement(ExpressionStatement expression)
    {
        // Вычисляем выражение и игнорируем результат
        EvaluateExpression(expression.Expression);
    }

    // Вспомогательные методы

    private decimal EvaluateExpression(Expression expression)
    {
        return expression switch
        {
            NumberLiteral n => n.Value,
            Identifier i => GetVariableValue(i.Name),
            Constant c => GetConstantValue(c),
            BinaryExpression b => EvaluateBinaryExpression(b),
            UnaryExpression u => EvaluateUnaryExpression(u),
            FunctionCall f => EvaluateFunctionCall(f),
            _ => throw new InvalidOperationException($"Неизвестный тип выражения: {expression.GetType()}")
        };
    }

    private decimal GetVariableValue(string name)
    {
        if (!context.TryGetVariable(name, out decimal value))
        {
            throw new InvalidOperationException($"Неизвестная переменная: {name}");
        }

        return value;
    }

    private decimal GetConstantValue(Constant constant)
    {
        return constant.Type switch
        {
            Constant.ConstantType.Pi => 3.141592653589793m,
            Constant.ConstantType.E => 2.718281828459045m,
            _ => throw new InvalidOperationException($"Неизвестная константа: {constant.Type}")
        };
    }

    private decimal EvaluateBinaryExpression(BinaryExpression binary)
    {
        decimal left = EvaluateExpression(binary.Left);
        decimal right = EvaluateExpression(binary.Right);

        return binary.Operator switch
        {
            BinaryExpression.BinaryOperator.Add => left + right,
            BinaryExpression.BinaryOperator.Subtract => left - right,
            BinaryExpression.BinaryOperator.Multiply => left * right,
            BinaryExpression.BinaryOperator.Divide => right != 0 ? left / right : throw new DivideByZeroException("Деление на ноль"),
            BinaryExpression.BinaryOperator.Modulo => right != 0 ? left % right : throw new DivideByZeroException("Остаток от деления на ноль"),
            BinaryExpression.BinaryOperator.Power => (decimal)Math.Pow((double)left, (double)right),
            BinaryExpression.BinaryOperator.Equal => left == right ? 1m : 0m,
            BinaryExpression.BinaryOperator.NotEqual => left != right ? 1m : 0m,
            BinaryExpression.BinaryOperator.LessThan => left < right ? 1m : 0m,
            BinaryExpression.BinaryOperator.LessThanOrEqual => left <= right ? 1m : 0m,
            BinaryExpression.BinaryOperator.GreaterThan => left > right ? 1m : 0m,
            BinaryExpression.BinaryOperator.GreaterThanOrEqual => left >= right ? 1m : 0m,
            BinaryExpression.BinaryOperator.LogicalAnd => (left != 0 && right != 0) ? 1m : 0m,
            BinaryExpression.BinaryOperator.LogicalOr => (left != 0 || right != 0) ? 1m : 0m,
            _ => throw new InvalidOperationException($"Неизвестный бинарный оператор: {binary.Operator}")
        };
    }

    private decimal EvaluateUnaryExpression(UnaryExpression unary)
    {
        decimal operand = EvaluateExpression(unary.Operand);

        return unary.Operator switch
        {
            UnaryExpression.UnaryOperator.Plus => operand,
            UnaryExpression.UnaryOperator.Minus => -operand,
            UnaryExpression.UnaryOperator.LogicalNot => operand == 0 ? 1m : 0m,
            _ => throw new InvalidOperationException($"Неизвестный унарный оператор: {unary.Operator}")
        };
    }

    private decimal EvaluateFunctionCall(FunctionCall call)
    {
        // Проверяем встроенные функции
        if (IsBuiltinFunction(call.FunctionName))
        {
            List<decimal> arguments = new();
            foreach (Expression arg in call.Arguments)
            {
                arguments.Add(EvaluateExpression(arg));
            }

            return BuiltinFunctions.Invoke(call.FunctionName, arguments);
        }

        // Проверяем пользовательские функции
        if (!functions.TryGetValue(call.FunctionName, out FunctionDefinition? function))
        {
            throw new InvalidOperationException($"Неизвестная функция: {call.FunctionName}");
        }

        // Проверяем количество аргументов
        if (call.Arguments.Count != function.Parameters.Count)
        {
            throw new InvalidOperationException(
                $"Функция '{call.FunctionName}' ожидает {function.Parameters.Count} аргументов, получено {call.Arguments.Count}");
        }

        // Вычисляем аргументы
        List<decimal> argumentValues = new();
        foreach (Expression arg in call.Arguments)
        {
            argumentValues.Add(EvaluateExpression(arg));
        }

        // Создаем новую область видимости для функции
        context.PushScope();

        try
        {
            // Присваиваем значения параметрам
            for (int i = 0; i < function.Parameters.Count; i++)
            {
                context.TryDefineVariable(function.Parameters[i], argumentValues[i]);
            }

            // Выполняем тело функции
            VisitBlock(function.Body);

            // Ищем return statement в результате выполнения
            // Для этого нужно модифицировать VisitBlock, чтобы он возвращал значение
            // Пока что просто выполняем блок
            throw new InvalidOperationException("Функция должна содержать 'tank yu'");
        }
        finally
        {
            context.PopScope();
        }
    }

    private void VisitDeclaration(Declaration declaration)
    {
        switch (declaration)
        {
            case FunctionDefinition f:
                VisitFunctionDefinition(f);
                break;
            case ConstDeclaration c:
                VisitConstDeclaration(c);
                break;
            default:
                throw new InvalidOperationException($"Неизвестный тип объявления: {declaration.GetType()}");
        }
    }

    private void VisitStatement(Statement statement)
    {
        switch (statement)
        {
            case VariableDeclaration v:
                VisitVariableDeclaration(v);
                break;
            case AssignmentStatement a:
                VisitAssignmentStatement(a);
                break;
            case InputStatement i:
                VisitInputStatement(i);
                break;
            case OutputStatement o:
                VisitOutputStatement(o);
                break;
            case IfStatement ifs:
                VisitIfStatement(ifs);
                break;
            case WhileStatement ws:
                VisitWhileStatement(ws);
                break;
            case ReturnStatement r:
                VisitReturnStatement(r);
                break;
            case Block b:
                VisitBlock(b);
                break;
            case ExpressionStatement e:
                VisitExpressionStatement(e);
                break;
            default:
                throw new InvalidOperationException($"Неизвестный тип инструкции: {statement.GetType()}");
        }
    }

    private static bool IsBuiltinFunction(string name)
    {
        return name == "muak" || name == "miniboss" || name == "bigboss";
    }
}

