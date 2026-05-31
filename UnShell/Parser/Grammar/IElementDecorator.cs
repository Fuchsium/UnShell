using UnShell.Runtime;

namespace UnShell.Parser.Grammar;

public interface IElementDecorator
{
    public Expression Decorate(Expression expr, ParsingContext context);
}

public sealed class ConvertDecorator<T> : IElementDecorator
{
    internal static readonly ConvertDecorator<T> Instance = new();

    public Expression Decorate(Expression expr, ParsingContext context)
    {
        if(expr is LiteralExpression literalExpression)
        {
            return new LiteralExpression(Convert.ChangeType(literalExpression.Literal, typeof(T)));
        } else
        {
            throw new NotSupportedException();
        }
    }
}
