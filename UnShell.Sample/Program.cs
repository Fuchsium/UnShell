using UnShell.Parser;
using UnShell.Runtime;

var engine = new Engine();

var lexer = new Utf8Lexer(" echo hi"u8);
var script = engine.ParseScript(ref lexer);

if(engine.ParsingContext.Errors.Count > 0)
{
    foreach(var error in engine.ParsingContext.Errors)
    {
        Console.WriteLine(error);
    }
} else
{
    script.Execute();
}
