namespace Nlox.Core.Tests;

public class ScannerTest {
    [Fact]
    public void ScanTokensReturnsListOfTokens() {
        var source = @"// this is a comment
(){} // grouping stuff
!*+-/=<>. <= >= == != // operators
nil true false 1234 12.34 """" ""Hello, world!"" ;
print
and or var if else while for fun return class this super";

        var expected = new List<Token> {
            new(TokenType.LEFT_PAREN, "(", null, 2),
            new(TokenType.RIGHT_PAREN, ")", null, 2),
            new(TokenType.LEFT_BRACE, "{", null, 2),
            new(TokenType.RIGHT_BRACE, "}", null, 2),

            new(TokenType.BANG, "!", null, 3),
            new(TokenType.STAR, "*", null, 3),
            new(TokenType.PLUS, "+", null, 3),
            new(TokenType.MINUS, "-", null, 3),
            new(TokenType.SLASH, "/", null, 3),
            new(TokenType.EQUAL, "=", null, 3),
            new(TokenType.LESS, "<", null, 3),
            new(TokenType.GREATER, ">", null, 3),
            new(TokenType.DOT, ".", null, 3),
            new(TokenType.LESS_EQUAL, "<=", null, 3),
            new(TokenType.GREATER_EQUAL, ">=", null, 3),
            new(TokenType.EQUAL_EQUAL, "==", null, 3),
            new(TokenType.BANG_EQUAL, "!=", null, 3),

            new(TokenType.NIL, "nil", null, 4),
            new(TokenType.TRUE, "true", null, 4),
            new(TokenType.FALSE, "false", null, 4),
            new(TokenType.NUMBER, "1234", 1234, 4),
            new(TokenType.NUMBER, "12.34", 12.34, 4),
            new(TokenType.STRING, "\"\"", "", 4),
            new(TokenType.STRING, "\"Hello, world!\"", "Hello, world!", 4),
            new(TokenType.SEMICOLON, ";", null, 4),

            new(TokenType.PRINT, "print", null, 5),

            new(TokenType.AND, "and", null, 6),
            new(TokenType.OR, "or", null, 6),
            new(TokenType.VAR, "var", null, 6),
            new(TokenType.IF, "if", null, 6),
            new(TokenType.ELSE, "else", null, 6),
            new(TokenType.WHILE, "while", null, 6),
            new(TokenType.FOR, "for", null, 6),
            new(TokenType.FUN, "fun", null, 6),
            new(TokenType.RETURN, "return", null, 6),
            new(TokenType.CLASS, "class", null, 6),
            new(TokenType.THIS, "this", null, 6),
            new(TokenType.SUPER, "super", null, 6),

            new(TokenType.EOF, "", null, 6)
        };

        var s = new Scanner(source);
        var tokens = s.ScanTokens();

        for (var i = 0; i < tokens.Count; i++) {
            Assert.Equal(expected[i].ToString(), tokens[i].ToString());
        }
    }
}
