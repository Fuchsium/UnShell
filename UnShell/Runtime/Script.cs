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

    public void Execute()
    {
        foreach(var instruction in _instructions)
        {
            instruction.Execute(_engine.ExecutionContext);
        }
    }
}
