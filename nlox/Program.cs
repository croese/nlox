using System;
using System.Collections.Generic;
using System.IO;

namespace nlox
{
  class Program
  {
    private static bool hadError = false;

    static void Main(string[] args)
    {
      if (args.Length > 1)
      {
        Console.WriteLine("Usage: nlox [script]");
      }
      else if (args.Length == 2)
      {
        RunFile(args[0]);
      }
      else
      {
        RunPrompt();
      }
    }

    private static void RunPrompt()
    {
      while (true)
      {
        Console.Write("> ");
        Run(Console.ReadLine());
        hadError = false;
      }
    }

    private static void RunFile(string path)
    {
      var source = File.ReadAllText(path);
      Run(source);
      if (hadError)
        Environment.Exit(65);
    }

    private static void Run(string source)
    {
      var scanner = new Scanner(source);
      List<Token> tokens = scanner.ScanTokens();

      foreach (var token in tokens)
      {
        Console.WriteLine(token);
      }
    }

    public static void Error(int line, string message)
    {
      report(line, "", message);
    }

    private static void report(int line, string where, string message)
    {
      Console.Error.WriteLine($"[line {line}] Error{where}: ${message}");
      hadError = true;
    }
  }
}