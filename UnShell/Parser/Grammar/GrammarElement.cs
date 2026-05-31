using System.Diagnostics;
using System.Text;
using UnShell.Runtime;

namespace UnShell.Parser.Grammar;

public abstract class GrammarElement
{
    public List<IElementDecorator> Decorators {get;} = [];
    public bool Optional {get; set;}
    public abstract List<Expression>? Parse(ref Utf8Lexer lexer, ParsingContext context, bool inOptionalGroup);

    protected Expression RunDecorators(Expression expr, ParsingContext context)
    {
        foreach(var decorator in Decorators)
        {
            expr = decorator.Decorate(expr, context);
        }
        return expr;
    }
}

public sealed class EmptyGrammarElement : GrammarElement
{
    internal static readonly EmptyGrammarElement Instance = new();

    private EmptyGrammarElement() {}

    public override List<Expression>? Parse(ref Utf8Lexer lexer, ParsingContext context, bool inOptionalGroup)
    {
        return [];
    }
}

public sealed class GroupGrammarElement : GrammarElement
{
    private readonly GrammarElement[] _elements;

    internal GroupGrammarElement(GrammarElement[] elements)
    {
        _elements = elements;
    }

    public override List<Expression>? Parse(ref Utf8Lexer lexer, ParsingContext context, bool inOptionalGroup)
    {
        var result = new List<Expression>();
        foreach(var element in _elements)
        { 
            var exprs = element.Parse(ref lexer, context, inOptionalGroup || Optional);
            if(exprs == null)
            {
                return Optional ? [] : null;
            }

            result.AddRange(exprs);
        }
        return result;
    }
}

public sealed class KeywordGrammarElement : GrammarElement
{
    private readonly byte[] _keywordUtf8 = [];

    internal KeywordGrammarElement(byte[] keywordUtf8)
    {
        _keywordUtf8 = keywordUtf8;
    }

    public override List<Expression>? Parse(ref Utf8Lexer lexer, ParsingContext context, bool inOptionalGroup)
    {
        var didRead = lexer.ReadToken();
        if(!didRead || lexer.Token != TokenType.Literal)
        {
            if(!Optional && !inOptionalGroup)
            {
                context.Errors.Add(new ParseError(lexer.TokenLine, lexer.TokenColumn, $"Expected '{Encoding.UTF8.GetString(_keywordUtf8)}'"));
            } else if(didRead)
            {
                lexer.RepeatToken = true;
            }
            return Optional ? [] : null;
        }

        if(_keywordUtf8.Length != lexer.TokenSpan.Length)
        {
            if(!Optional && !inOptionalGroup)
            {
                context.Errors.Add(new ParseError(lexer.TokenLine, lexer.TokenColumn, $"Expected '{Encoding.UTF8.GetString(_keywordUtf8)}'"));
            } else if(didRead)
            {
                lexer.RepeatToken = true;
            }
            return Optional ? [] : null;
        }

        for(var i=0; i<_keywordUtf8.Length; i++)
        {
            if(_keywordUtf8[i] == lexer.TokenSpan[i])
            {
                continue;
            }
            if(!Optional && !inOptionalGroup)
            {
                context.Errors.Add(new ParseError(lexer.TokenLine, lexer.TokenColumn, $"Expected '{Encoding.UTF8.GetString(_keywordUtf8)}'"));
            } else if(didRead)
            {
                lexer.RepeatToken = true;
            }
            return Optional ? [] : null;
        }

        return [];
    }
}

public sealed class LiteralGrammarElement : GrammarElement
{
    internal LiteralGrammarElement() {}

    public override List<Expression>? Parse(ref Utf8Lexer lexer, ParsingContext context, bool inOptionalGroup)
    {
        if(!lexer.ReadToken() || lexer.Token != TokenType.Literal)
        {
            if(!Optional && !inOptionalGroup)
            {
                context.Errors.Add(new ParseError(lexer.TokenLine, lexer.TokenColumn, "Unexpected end of instruction"));
            }
            return null;
        }

        var expr = Expression.FromToken(ref lexer, context);
        expr = RunDecorators(expr, context);
        return [expr];
    }
}

public sealed class WideGrammarElement : GrammarElement
{
    internal static readonly WideGrammarElement Instance = new();

    private WideGrammarElement() {}

    public override List<Expression>? Parse(ref Utf8Lexer lexer, ParsingContext context, bool inOptionalGroup)
    {
        var exprs = new List<Expression>();
        while(lexer.ReadWideToken() && lexer.Token != TokenType.EndOfLine)
        {
            exprs.Add(Expression.FromToken(ref lexer, context));
        }
        if(exprs.Count == 1)
        {
            return [RunDecorators(exprs[0], context)];
        }
        return [RunDecorators(new ConcatExpression(exprs), context)];
    }
}
