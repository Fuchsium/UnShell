using System.Text;
using UnShell.Parser.Grammar;
using UnShell.Runtime;

namespace UnShell.Parser;

public sealed class ParsingContext
{
    private readonly Dictionary<string, InstructionParser> _instructions = [];
    private readonly Dictionary<string, int> _variables = [];
    private int _variableCounter = 0;
    
    public IReadOnlyDictionary<string, InstructionParser> Instructions => _instructions;
    public ICollection<ParseError> Errors {get;} = [];

    internal ParsingContext() {}

    public void AddInstruction(InstructionParser instruction)
    {
        _instructions[instruction.Name] = instruction;
    }

    internal void AddStandardInstructions()
    {
        AddInstruction(EchoInstruction.Parser);
    }

    public int GetGlobalVariableId(string name)
    {
        if(_variables.TryGetValue(name, out var result))
        {
            return result;
        }
        _variables[name] = _variableCounter;
        _variableCounter++;
        return _variables[name];
    }

    public Instruction? ParseInstruction(ref Utf8Lexer lexer)
    {
        do
        {
            if(!lexer.ReadToken())
            {
                return null;
            }
        } while(lexer.Token == TokenType.EndOfText);

        if(lexer.TokenSpan.Length > 0 && lexer.TokenSpan[0] == (byte)'$')
        {
            lexer.RepeatToken = true;
            return VariableInstruction.Parser.ParseInstruction(ref lexer, this);
        }

        var name = Encoding.UTF8.GetString(lexer.TokenSpan);
        if(!_instructions.TryGetValue(name, out var parser))
        {
            Errors.Add(new ParseError(lexer.TokenLine, lexer.TokenColumn, "Unknown instruction"));
            return null;
        }
        return parser.ParseInstruction(ref lexer, this);
    }
}
