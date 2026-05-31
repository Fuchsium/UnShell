namespace UnShell.Parser;

public readonly struct ParseError
{
    public int Line {get;}
    public int Column {get;}
    public string Message {get;}

    public ParseError(int line, int column, string message)
    {
        Line = line;
        Column = column;
        Message = message;
    }

    public override string ToString()
    {
        return $"{Line}:{Column} {Message}";
    }
}
