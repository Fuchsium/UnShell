namespace UnShell.Runtime;

public abstract class Instruction
{
    public abstract object? Execute(ExecutionContext context);
}
