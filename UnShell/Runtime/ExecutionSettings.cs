namespace UnShell.Runtime;

public sealed class ExecutionSettings
{
    public Action<string> EchoCallback {get; set;} = (msg) => Console.WriteLine(msg);
}
