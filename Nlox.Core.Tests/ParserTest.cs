namespace Nlox.Core.Tests;

public class ParserTest {
    [Theory]
    [InlineData(TokenType.TRUE, true)]
    [InlineData(TokenType.FALSE, false)]
    [InlineData(TokenType.NIL, null)]
    [InlineData(TokenType.NUMBER, 123)]
    [InlineData(TokenType.NUMBER, 123.5)]
    [InlineData(TokenType.STRING, "")]
    [InlineData(TokenType.STRING, "hello world")]
    public void ItParsesLiterals(TokenType type, object? literal) {
        var tokens = new List<Token> {
            new(type, literal?.ToString() ?? "nil", literal, 0),
            new(TokenType.SEMICOLON, ";", null, 0),
            new(TokenType.EOF, "", null, 0)
        };

        var p = new Parser(tokens);
        var parsed = p.Parse();

        var lit = (Expr.Literal)((Stmt.Expression)parsed[0]).Expr;
        Assert.Equal(literal, lit.Value);
    }

    [Theory]
    [InlineData(TokenType.BANG, 1)]
    [InlineData(TokenType.MINUS, 2)]
    public void ItParsesUnaryExpressionsWithLiterals(TokenType type, int right) {
        var tokens = new List<Token> {
            new(type, "", null, 0),
            new(TokenType.NUMBER, right.ToString(), right, 0),
            new(TokenType.SEMICOLON, ";", null, 0),
            new(TokenType.EOF, "", null, 0)
        };

        var p = new Parser(tokens);
        var parsed = p.Parse();

        var unary = (Expr.Unary)((Stmt.Expression)parsed[0]).Expr;
        Assert.Equal(type, unary.Operator.Type);
        Assert.IsType<Expr.Literal>(unary.Right);
    }

    [Theory]
    [InlineData(TokenType.BANG_EQUAL, 1, 1)]
    [InlineData(TokenType.EQUAL_EQUAL, 1, 1)]
    [InlineData(TokenType.PLUS, 1, 1)]
    [InlineData(TokenType.MINUS, 1, 1)]
    [InlineData(TokenType.SLASH, 1, 1)]
    [InlineData(TokenType.STAR, 1, 1)]
    [InlineData(TokenType.GREATER, 1, 1)]
    [InlineData(TokenType.GREATER_EQUAL, 1, 1)]
    [InlineData(TokenType.LESS_EQUAL, 1, 1)]
    [InlineData(TokenType.LESS, 1, 1)]
    public void ItParsesBinaryExpressionsWithLiterals(TokenType type, int left, int right) {
        var tokens = new List<Token> {
            new(TokenType.NUMBER, left.ToString(), left, 0),
            new(type, "", null, 0),
            new(TokenType.NUMBER, right.ToString(), right, 0),
            new(TokenType.SEMICOLON, ";", null, 0),
            new(TokenType.EOF, "", null, 0)
        };

        var p = new Parser(tokens);
        var parsed = p.Parse();

        var binary = (Expr.Binary)((Stmt.Expression)parsed[0]).Expr;
        Assert.Equal(type, binary.Operator.Type);
        Assert.IsType<Expr.Literal>(binary.Left);
        Assert.IsType<Expr.Literal>(binary.Right);
    }

    // [Fact]
    // public void ItSetsErrorFlagWhenMissingClosingParen() {
    //     var tokens = new List<Token> {
    //         new(TokenType.LEFT_PAREN, "(", null, 0),
    //         new(TokenType.NUMBER, "3", 3, 0),
    //         new(TokenType.PLUS, "+", null, 0),
    //         new(TokenType.NUMBER, "3", 3, 0),
    //         new(TokenType.SEMICOLON, ";", null, 0),
    //         new(TokenType.EOF, "", null, 0)
    //     };
    //
    //     Lox.HadError = false;
    //     var p = new Parser(tokens);
    //     var parsed = p.Parse();
    //     Assert.True(Lox.HadError);
    // }
    //
    // [Fact]
    // public void ItSetsErrorFlagWhenMissingExpression() {
    //     var tokens = new List<Token> {
    //         new(TokenType.SEMICOLON, ";", null, 0),
    //         new(TokenType.EOF, "", null, 0)
    //     };
    //
    //     Lox.HadError = false;
    //     var p = new Parser(tokens);
    //     var parsed = p.Parse();
    //     Assert.True(Lox.HadError);
    // }
}
