namespace Ast.Expressions;

/// <summary>
/// Узел для идентификатора (переменной).
/// </summary>
public class Identifier : Expression
{
    /// <summary>
    /// Имя переменной.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Identifier"/>.
    /// </summary>
    /// <param name="name">Имя переменной.</param>
    public Identifier(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <inheritdoc/>
    public override void Accept(IAstVisitor visitor)
    {
        visitor.VisitIdentifier(this);
    }
}

