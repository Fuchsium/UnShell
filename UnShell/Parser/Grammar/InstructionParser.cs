using UnShell.Runtime;

namespace UnShell.Parser.Grammar;

public sealed class InstructionParser
{
    public string Name {get; private set;}

    private readonly GrammarElement _grammar;
    private readonly Func<List<Expression>, ParsingContext, Instruction> _constructor;

    public InstructionParser(string name, GrammarElement grammar, Func<List<Expression>, ParsingContext, Instruction> constructor)
    {
        Name = name;
        _grammar = grammar;
        _constructor = constructor;
    }

    public Instruction ParseInstruction(ref Utf8Lexer lexer, ParsingContext context)
    {
        return _constructor(_grammar.Parse(ref lexer, context, inOptionalGroup: false) ?? [], context);
    }
}
