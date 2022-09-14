namespace Nlox.Core;

public class Scanner {
    private static readonly Dictionary<string, TokenType> Keywords = new() {
        { "and", TokenType.AND },
        { "class", TokenType.CLASS },
        { "else", TokenType.ELSE },
        { "false", TokenType.FALSE },
        { "for", TokenType.FOR },
        { "fun", TokenType.FUN },
        { "if", TokenType.IF },
        { "nil", TokenType.NIL },
        { "or", TokenType.OR },
        { "print", TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super", TokenType.SUPER },
        { "this", TokenType.THIS },
        { "true", TokenType.TRUE },
        { "var", TokenType.VAR },
        { "while", TokenType.WHILE }
    };

    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private int _current;
    private int _line = 1;
    private int _start;

    public Scanner(string source) {
        _source = source;
    }

    public List<Token> ScanTokens() {
        while (!IsAtEnd()) {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOF, "", null, _line));
        return _tokens;
    }

    private void ScanToken() {
        var c = Advance();
        switch (c) {
            case '(':
                AddToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                AddToken(TokenType.COMMA);
                break;
            case '.':
                AddToken(TokenType.DOT);
                break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case ';':
                AddToken(TokenType.SEMICOLON);
                break;
            case '*':
                AddToken(TokenType.STAR);
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/')) {
                    while (Peek() != '\n' && !IsAtEnd()) {
                        Advance();
                    }
                }
                else {
                    AddToken(TokenType.SLASH);
                }

                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                _line++;
                break;
            case '"':
                ParseString();
                break;
            default:
                if (IsDigit(c)) {
                    ParseNumber();
                }
                else if (IsAlpha(c)) {
                    ParseIdentifier();
                }
                else {
                    Lox.Error(_line, $"unexpected character '{c}'.");
                }

                break;
        }
    }

    private void ParseIdentifier() {
        while (IsAlphaNumeric(Peek())) {
            Advance();
        }

        var text = _source.Substring(_start, _current - _start);
        AddToken(Keywords.TryGetValue(text, out var type) ? type : TokenType.IDENTIFIER);
    }

    private bool IsAlphaNumeric(char c) {
        return IsDigit(c) || IsAlpha(c);
    }

    private bool IsAlpha(char c) {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    private void ParseNumber() {
        while (IsDigit(Peek())) {
            Advance();
        }

        if (Peek() == '.' && IsDigit(PeekNext())) {
            Advance(); // eat the .

            while (IsDigit(Peek())) {
                Advance();
            }
        }

        AddToken(TokenType.NUMBER, double.Parse(_source.Substring(_start, _current - _start)));
    }

    private char PeekNext() {
        if (_current + 1 >= _source.Length) {
            return '\0';
        }

        return _source[_current + 1];
    }

    private bool IsDigit(char c) {
        return c is >= '0' and <= '9';
    }

    private void ParseString() {
        while (Peek() != '"' && !IsAtEnd()) {
            if (Peek() == '\n') {
                _line++;
            }

            Advance();
        }

        if (IsAtEnd()) {
            Lox.Error(_line, "unterminated string.");
            return;
        }

        Advance(); // eat closing "

        var value = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.STRING, value);
    }

    private char Peek() {
        if (IsAtEnd()) {
            return '\0';
        }

        return _source[_current];
    }

    private bool Match(char expected) {
        if (IsAtEnd()) {
            return false;
        }

        if (_source[_current] != expected) {
            return false;
        }

        _current++;
        return true;
    }

    private void AddToken(TokenType type) {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object? literal) {
        var text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line));
    }

    private char Advance() {
        return _source[_current++];
    }

    private bool IsAtEnd() {
        return _current >= _source.Length;
    }
}
