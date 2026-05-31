using UnShell.Parser.Grammar;

namespace UnShell.Runtime;

internal sealed class EchoInstruction : Instruction
{
    public static readonly InstructionParser Parser = new("echo",
        Grammars.Wide(),
        (exprs, ctx) => new EchoInstruction(exprs[0]));

    private readonly Expression _message;

    public EchoInstruction(Expression message)
    {
        _message = message;
    }

    public override object Execute(ExecutionContext context)
    {
        var message = _message.Evaluate(context)?.ToString() ?? "";
        context.Settings.EchoCallback?.Invoke(message);
        return message;
    }
}
