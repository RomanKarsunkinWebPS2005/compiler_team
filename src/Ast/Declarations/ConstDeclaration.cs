using Ast.Expressions;

namespace Ast.Declarations;

/// <summary>
/// Узел для объявления константы (trusela).
/// </summary>
public class ConstDeclaration : Declaration
{
    /// <summary>
    /// Имя константы.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Значение константы (литерал или константа belloPi/belloE).
    /// </summary>
    public Expression Value { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ConstDeclaration"/>.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="value">Значение константы.</param>
    public ConstDeclaration(string name, Expression value)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc/>
    public override void Accept(IAstVisitor visitor)
    {
        visitor.VisitConstDeclaration(this);
    }
}

