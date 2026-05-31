namespace UnShell.Runtime;

public sealed class ExecutionContext
{
    private readonly List<object?> _variables = [];

    public ExecutionSettings Settings {get;}

    internal ExecutionContext(ExecutionSettings settings)
    {
        Settings = settings;
    }

    public object? GetGlobalVariable(int id)
    {
        if(_variables.Count <= id)
        {
            return null;
        }
        return _variables[id];
    }
    public void SetGlobalVariable(int id, object? value)
    {
        if(_variables.Count > id)
        {
            _variables[id] = value;
        } else
        {
            while(_variables.Count <= id)
            {
                _variables.Add(null);
            }
            _variables[id] = value;
        }
    }
}
