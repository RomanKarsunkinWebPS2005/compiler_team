namespace Ast.Statements;

/// <summary>
/// Узел для объявления переменной (poop).
/// </summary>
public class VariableDeclaration : Statement
{
    /// <summary>
    /// Имя переменной.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="VariableDeclaration"/>.
    /// </summary>
    /// <param name="name">Имя переменной.</param>
    public VariableDeclaration(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <inheritdoc/>
    public override void Accept(IAstVisitor visitor)
    {
        visitor.VisitVariableDeclaration(this);
    }
}