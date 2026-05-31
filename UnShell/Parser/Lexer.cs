namespace UnShell.Parser;

public ref struct Utf8Lexer
{
    private ReadOnlySpan<byte> _buffer;
    public ReadOnlySpan<byte> Buffer => _buffer;

    private int _offset;
    private bool _escape;
    private int _line;
    private int _column;
    public int Offset => _offset;
    // Hack for situations where you'd want to re-read the current token
    public bool RepeatToken {get; set;}

    public TokenType Token {get; private set;}
    public ReadOnlySpan<byte> TokenSpan {get; private set;}

    public int TokenLine {get; private set;}
    public int TokenColumn {get; private set;}

    public Utf8Lexer(ReadOnlySpan<byte> text)
    {
        _buffer = text;
    }

    public bool ReadToken()
    {
        return ReadTokenCore(false);
    }
    public bool ReadWideToken()
    {
        return ReadTokenCore(true);
    }

    private bool ReadTokenCore(bool wide)
    {
        if(Token == TokenType.None)
        {
            SkipWhitespace();
        }
        if(RepeatToken)
        {
            RepeatToken = false;
            return true;
        }
        if(_offset >= _buffer.Length)
        {
            Token = TokenType.EndOfText;
            return false;
        }
        var start = _offset;
        TokenLine = _line;
        TokenColumn = _column;

        // End of Line
        NormalizeEndOfLine();
        if(_offset >= _buffer.Length)
        {
            Token = TokenType.EndOfText;
            return false;
        }
        if(_buffer[_offset] == (byte)'\n')
        {
            Read();
            TokenSpan = _buffer.Slice(start, _offset - start);
            SkipWhitespace();
            Token = TokenType.EndOfLine;
            return true;
        }
        if(IsSpecialCharacter((char)_buffer[_offset]))
        {
            TokenSpan = _buffer.Slice(_offset, 1);
            Read();
            SkipWhitespace();
            Token = TokenType.SpecialCharacter;
            return true;
        }

        // Literal
        while(Read())
        {
            if(!_escape) {
                if(_buffer[_offset] == (byte)'\n' || IsSpecialCharacter((char)_buffer[_offset]))
                {
                    break;
                }
            }

            if(!wide && !_escape && char.IsWhiteSpace((char)_buffer[_offset]))
            {
                Token = TokenType.Literal;
                TokenSpan = _buffer.Slice(start, _offset - start);

                SkipWhitespace();
                return true;
            }

            _escape = false;
        }
        Token = TokenType.Literal;
        TokenSpan = _buffer.Slice(start, _offset - start);
        return true;
    }

    private void SkipWhitespace()
    {
        do
        {
            if(_escape || _offset >= _buffer.Length || !char.IsWhiteSpace((char)_buffer[_offset]))
            {
                break;
            }
        } while(Read());
        _escape = false;
    }

    private static bool IsSpecialCharacter(char c)
    {
        return c is '<' or '>';
    }

    //TODO: Proper handling for code points wider than one byte
    private bool Read()
    {
        _escape = false;
        _offset++;
        _column++;
        if(_offset >= _buffer.Length)
        {
            Token = TokenType.EndOfText;
            return false;
        }

        if(!NormalizeEndOfLine())
        {
            return false;
        }

        // Escape sequence
        if(_buffer[_offset] == (byte)'\\')
        {
            _offset++;
            _column++;
            _escape = true;
            return _offset < _buffer.Length;
        }

        // Newline
        if(_buffer[_offset] == (byte)'\r' || _buffer[_offset] == (byte)'\n')
        {
            _line++;
            _column = 0;
        }

        return true;
    }

    // CRLF handling; skip CR if it is followed by LF
    private bool NormalizeEndOfLine()
    {
        if(_buffer[_offset] == (byte)'\r' && _offset + 1 < _buffer.Length && _buffer[_offset + 1] == (byte)'\n')
        {
            _offset++;
            if(_offset >= _buffer.Length)
            {
                Token = TokenType.EndOfText;
                return false;
            }
        }
        return true;
    }
}

public enum TokenType
{
    None,
    Literal,
    EndOfLine,
    SpecialCharacter,
    EndOfText,
}
