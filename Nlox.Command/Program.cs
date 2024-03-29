﻿using Nlox.Core;
using Environment = System.Environment;

if (args.Length > 1) {
    Console.WriteLine("usage: nlox [script]");
    Environment.Exit(65);
}
else if (args.Length == 1) {
    RunFile(args[0]);
}
else {
    RunPrompt();
}

void RunFile(string path) {
    var source = File.ReadAllText(path);
    Run(source);

    if (Lox.HadError) {
        Environment.Exit(65);
    }

    if (Lox.HadRuntimeError) {
        Environment.Exit(70);
    }
}

void RunPrompt() {
    while (true) {
        Console.Write("> ");
        var line = Console.ReadLine();
        if (line == null) {
            break;
        }

        Run(line);
        Lox.HadError = false;
    }
}

void Run(string source) {
    var scanner = new Scanner(source);
    var tokens = scanner.ScanTokens();

    var parser = new Parser(tokens);
    var statements = parser.Parse();

    if (Lox.HadError) {
        return;
    }

    var interpreter = new Interpreter(Console.Out);
    interpreter.Interpret(statements);
}
