namespace Nlox.Core.Tests;

public class InterpreterTest {
    [Theory]
    [InlineData(null, "nil")]
    [InlineData(42, "42")]
    [InlineData("hello world", "hello world")]
    public static void ItInterpretsLiteralExpressionsAsValues(object? literal, string expected) {
        var buffer = new StringWriter();
        var interpreter = new Interpreter(buffer);

        interpreter.Interpret(new Expr.Literal(literal));

        Assert.Equal(expected, buffer.ToString().TrimEnd());
    }
}
