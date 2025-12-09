namespace Ast;

/// <summary>
/// Узел для представления всей программы.
/// </summary>
public class Program : AstNode
{
    /// <summary>
    /// Список элементов верхнего уровня (объявления и инструкции).
    /// </summary>
    public IReadOnlyList<AstNode> TopLevelItems { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Program"/>.
    /// </summary>
    /// <param name="topLevelItems">Список элементов верхнего уровня.</param>
    public Program(IReadOnlyList<AstNode> topLevelItems)
    {
        TopLevelItems = topLevelItems ?? throw new ArgumentNullException(nameof(topLevelItems));
    }

    /// <inheritdoc/>
    public override void Accept(IAstVisitor visitor)
    {
        visitor.VisitProgram(this);
    }
}

