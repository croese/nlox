namespace Nlox.Core.Tests;

public class InterpreterTest {
    [Theory]
    [InlineData(null, "nil")]
    [InlineData(42, "42")]
    [InlineData("hello world", "hello world")]
    public static void ItInterpretsLiteralExpressionsAsValues(object? literal, string expected) {
        var buffer = new StringWriter();
        var interpreter = new Interpreter(buffer);

        var statements = new List<Stmt> { new Stmt.Expression(new Expr.Literal(literal)) };
        interpreter.Interpret(statements);

        Assert.Equal(expected, buffer.ToString().TrimEnd());
    }
}
