using UnShell.Parser.Grammar;
using static UnShell.Parser.Grammar.Grammars;

namespace UnShell.Runtime;

internal sealed class VariableInstruction : Instruction
{
    public static readonly InstructionParser Parser = new("", 
        Group(
            Literal(),
            Group(
                Keyword("="),
                Wide()
            ).Optional()),
        (exprs, ctx) =>
        {
            var name = (LiteralExpression)exprs[0];
            return new VariableInstruction(ctx.GetGlobalVariableId(name.Literal.ToString()), exprs.Count > 1 ? exprs[1] : null);
        });

    private readonly int _variable;
    private readonly Expression? _value;

    public VariableInstruction(int variable, Expression? value)
    {
        _variable = variable;
        _value = value;
    }

    public override object? Execute(ExecutionContext context)
    {
        if(_value == null)
        {
            return context.GetGlobalVariable(_variable);
        }

        var value = _value.Evaluate(context);
        context.SetGlobalVariable(_variable, value);
        return value;
    }
}
