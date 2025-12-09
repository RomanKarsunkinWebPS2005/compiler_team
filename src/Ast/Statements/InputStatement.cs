namespace Ast.Statements;

/// <summary>
/// Узел для инструкции ввода (guoleila).
/// </summary>
public class InputStatement : Statement
{
    /// <summary>
    /// Имя переменной для ввода.
    /// </summary>
    public string VariableName { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="InputStatement"/>.
    /// </summary>
    /// <param name="variableName">Имя переменной для ввода.</param>
    public InputStatement(string variableName)
    {
        VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
    }

    /// <inheritdoc/>
    public override void Accept(IAstVisitor visitor)
    {
        visitor.VisitInputStatement(this);
    }
}

