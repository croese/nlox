namespace Nlox.Core.Tests;

public class TokenTest {
    [Theory]
    [InlineData(TokenType.IF, "if", null, 3, "Line 3: IF if ")]
    [InlineData(TokenType.NUMBER, "42.5", 42.5, 11, "Line 11: NUMBER 42.5 42.5")]
    [InlineData(TokenType.STRING, "hello world", "hello world", 42, "Line 42: STRING hello world hello world")]
    public void ToStringReturnsFormattedString(
        TokenType type, string lexeme, object? literal, int line, string expected) {
        var tok = new Token(type, lexeme, literal, line);

        Assert.Equal(tok.ToString(), expected);
    }
}
