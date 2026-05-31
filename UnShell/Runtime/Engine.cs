using UnShell.Parser;

namespace UnShell.Runtime;

public sealed class Engine
{
    public ExecutionContext ExecutionContext {get;} = new(new ExecutionSettings());
    public ParsingContext ParsingContext {get;} = new ParsingContext();

    public Engine()
    {
        ParsingContext.AddStandardInstructions();
    }

    public Script ParseScript(ref Utf8Lexer lexer)
    {
        var instructions = new List<Instruction>();
        do
        {
            var instruction = ParsingContext.ParseInstruction(ref lexer);
            if(instruction != null)
            {
                instructions.Add(instruction);
            }
        } while(lexer.Token != TokenType.EndOfText);
        return new Script(this, instructions.ToArray());
    }
}
