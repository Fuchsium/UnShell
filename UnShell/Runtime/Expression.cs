using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using UnShell.Parser;

namespace UnShell.Runtime;

public abstract partial class Expression
{
    public abstract object? Evaluate(ExecutionContext context);

    public static Expression FromToken(ref Utf8Lexer lexer, ParsingContext context)
    {
        if(lexer.Token == TokenType.Literal) {
            var text = Encoding.UTF8.GetString(lexer.TokenSpan);
            text = Regex.Replace(text, @"\\(?!\\)", "");
            return new LiteralExpression(text);
        } else if(lexer.Token == TokenType.SpecialCharacter && lexer.TokenSpan[0] == (byte)'<')
        {
            var expr = new InstructionExpression(context.ParseInstruction(ref lexer));
            if(!(lexer.ReadToken() && lexer.Token == TokenType.SpecialCharacter && lexer.TokenSpan[0] == (byte)'>'))
            {
                context.Errors.Add(new ParseError(lexer.TokenLine, lexer.TokenColumn, "Expected '>'"));
            }
            return expr;
        } else
        {
            Debug.WriteLine(Encoding.UTF8.GetString(lexer.TokenSpan));
            throw new NotSupportedException();
        }
    }
}

public sealed class LiteralExpression : Expression
{
    public object Literal {get;}

    public LiteralExpression(object literal)
    {
        Literal = literal;
    }

    public override object Evaluate(ExecutionContext context)
    {
        return Literal;
    }
}

public sealed class ConcatExpression : Expression
{
    private readonly List<Expression> _expressions;

    public ConcatExpression(List<Expression> expressions)
    {
        _expressions = expressions;
    }

    public override object Evaluate(ExecutionContext context)
    {
        return string.Join("", _expressions);
    }
}

public sealed class InstructionExpression : Expression
{
    private readonly Instruction? _instruction;

    public InstructionExpression(Instruction? instruction)
    {
        _instruction = instruction;
    }

    public override object? Evaluate(ExecutionContext context)
    {
        return _instruction?.Execute(context);
    }
}
