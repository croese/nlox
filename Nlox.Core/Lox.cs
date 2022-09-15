namespace Nlox.Core;

public class Lox {
    public static bool HadError { get; set; }

    public static bool HadRuntimeError { get; set; }

    internal static void Error(int line, string message) {
        Report(line, "", message);
    }

    internal static void Error(Token token, string message) {
        Report(token.Line, token.Type == TokenType.EOF ? " at end" : $" at '{token.Lexeme}'", message);
    }

    internal static void RuntimeError(RuntimeError error) {
        Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
        HadRuntimeError = true;
    }

    private static void Report(int line, string where, string message) {
        Console.Error.WriteLine($"[line {line}] error{where}: {message}");
        HadError = true;
    }
}
