using UnShell.Parser;
using UnShell.Parser.Grammar;
using UnShell.Runtime;

namespace UnShell.Tests;

public class OptionalParameter
{
    [Test]
    public void Test1()
    {
        var engine = new Engine();
        engine.ParsingContext.AddInstruction(TestInstructionA.Parser);

        var lexer = new Utf8Lexer("test_a"u8);
        var script = engine.ParseScript(ref lexer);
        
        var result = script.Execute();
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test2()
    {
        var engine = new Engine();
        engine.ParsingContext.AddInstruction(TestInstructionA.Parser);

        var lexer = new Utf8Lexer("test_a 123"u8);
        var script = engine.ParseScript(ref lexer);
        
        var result = script.Execute();
        Assert.That(result, Is.EqualTo("123"));
    }

    [Test]
    public void Test3()
    {
        var engine = new Engine();
        engine.ParsingContext.AddInstruction(TestInstructionB.Parser);

        var lexer = new Utf8Lexer("test_b abc"u8);
        var script = engine.ParseScript(ref lexer);
        
        var result = script.Execute();
        Assert.That(result, Is.EqualTo("abc"));
    }

    [Test]
    public void Test4()
    {
        var engine = new Engine();
        engine.ParsingContext.AddInstruction(TestInstructionB.Parser);

        var lexer = new Utf8Lexer("test_b abc 123"u8);
        var script = engine.ParseScript(ref lexer);
        
        var result = script.Execute();
        Assert.That(result, Is.EqualTo("123"));
    }

    sealed class TestInstructionA : Instruction
    {
        public static readonly InstructionParser Parser = new("test_a", Grammars.Literal().Optional(), (expr, ctx) => new TestInstructionA(expr));

        private readonly Expression? _expr;

        public TestInstructionA(List<Expression> exprs)
        {
            _expr = exprs.FirstOrDefault();
        }

        public override object? Execute(Runtime.ExecutionContext context)
        {
            return _expr?.Evaluate(context);
        }
    }

    sealed class TestInstructionB : Instruction
    {
        public static readonly InstructionParser Parser = new("test_b", Grammars.Group(Grammars.Literal(), Grammars.Literal().Optional()), (expr, ctx) => new TestInstructionB(expr));

        private readonly Expression _expr1;
        private readonly Expression? _expr2;

        public TestInstructionB(List<Expression> exprs)
        {
            _expr1 = exprs[0];
            _expr2 = exprs.Count > 1 ? exprs[1] : null;
        }

        public override object? Execute(Runtime.ExecutionContext context)
        {
            return _expr2?.Evaluate(context) ?? _expr1.Evaluate(context);
        }
    }
}
