using System.Text;

namespace UnShell.Parser.Grammar;

public static class Grammars
{
    public static EmptyGrammarElement Empty() => EmptyGrammarElement.Instance;

    public static GroupGrammarElement Group(params GrammarElement[] elements)
    {
        return new GroupGrammarElement(elements);
    }
    public static LiteralGrammarElement Literal()
    {
        var element = new LiteralGrammarElement();
        return element;
    }
    public static WideGrammarElement Wide()
    {
        return WideGrammarElement.Instance;
    }
    public static KeywordGrammarElement Keyword(string keyword)
    {
        return new KeywordGrammarElement(Encoding.UTF8.GetBytes(keyword));
    }

    public static TElement AsInt32<TElement>(this TElement element) where TElement : GrammarElement
    {
        return element.As<int, TElement>();
    }
    public static TElement AsSingle<TElement>(this TElement element) where TElement : GrammarElement
    {
        return element.As<float, TElement>();
    }
    public static TElement As<T, TElement>(this TElement element) where TElement : GrammarElement
    {
        element.Decorators.Add(ConvertDecorator<T>.Instance);
        return element;
    }

    public static TElement Optional<TElement>(this TElement element) where TElement : GrammarElement
    {
        element.Optional = true;
        return element;
    }
}
