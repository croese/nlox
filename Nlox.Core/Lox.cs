namespace Nlox.Core;

public class Lox {
    public static bool HadError { get; set; }

    internal static void Error(int line, string message) {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message) {
        Console.Error.WriteLine($"[line {line}] error{where}: {message}");
    }
}
