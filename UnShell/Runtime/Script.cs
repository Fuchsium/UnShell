namespace UnShell.Runtime;

public sealed class Script
{
    private readonly Instruction[] _instructions;
    private readonly Engine _engine;
    public IReadOnlyList<Instruction> Instructions => _instructions;

    internal Script(Engine engine, Instruction[] instructions)
    {
        _instructions = instructions;
        _engine = engine;
    }

    public object? Execute()
    {
        object? lastResult = null;
        foreach(var instruction in _instructions)
        {
            lastResult = instruction.Execute(_engine.ExecutionContext);
        }
        return lastResult;
    }
}
