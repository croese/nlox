﻿using System.Text;

if (args.Length != 1) {
    Console.Error.WriteLine("usage: astgen <output_directory>");
    Environment.Exit(64);
}

var outputDir = args[0];
DefineAst(outputDir, "Expr", new List<string> {
    "Assign   : Token Name, Expr Value",
    "Binary   : Expr Left, Token Operator, Expr Right",
    "Grouping : Expr Expression",
    "Literal  : Object? Value",
    "Logical  : Expr Left, Token Operator, Expr Right",
    "Unary    : Token Operator, Expr Right",
    "Variable : Token Name"
});

DefineAst(outputDir, "Stmt", new List<string> {
    "Block      : List<Stmt> Statements",
    "Break      : Token Token",
    "Expression : Expr Expr",
    "If         : Expr Condition, Stmt ThenBranch, Stmt? ElseBranch",
    "Print      : Expr Expression",
    "Var        : Token Name, Expr? Initializer",
    "While      : Expr Condition, Stmt Body"
});

void DefineAst(string outputDir, string baseName, List<string> types) {
    var path = Path.Combine(outputDir, baseName + ".cs");
    var buffer = new StringBuilder();

    buffer.AppendLine("namespace Nlox.Core;");
    buffer.AppendLine();
    buffer.AppendLine($"public abstract class {baseName} {{");

    DefineVisitor(buffer, baseName, types);

    foreach (var type in types) {
        var className = type.Split(":")[0].Trim();
        var fields = type.Split(":")[1].Trim();
        DefineType(buffer, baseName, className, fields);
        buffer.AppendLine();
    }

    buffer.AppendLine();
    buffer.AppendLine("    public abstract R Accept<R>(Visitor<R> visitor);");

    buffer.AppendLine("}");
    File.WriteAllText(path, buffer.ToString());
}

void DefineVisitor(StringBuilder buffer, string baseName, List<string> types) {
    buffer.AppendLine("    public interface Visitor<R> {");

    foreach (var type in types) {
        var typeName = type.Split(":")[0].Trim();
        buffer.AppendLine($"        R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }

    buffer.AppendLine("    }");
}

void DefineType(StringBuilder buffer, string baseName, string className, string fieldList) {
    buffer.AppendLine($"    public class {className} : {baseName} {{");

    // constructor
    buffer.AppendLine($"        public {className}({fieldList}) {{");

    var fields = new string[] { };
    if (!string.IsNullOrWhiteSpace(fieldList)) {
        fields = fieldList.Split(", ");
        foreach (var field in fields) {
            var name = field.Split(" ")[1];
            buffer.AppendLine($"                this.{name} = {name};");
        }
    }

    buffer.AppendLine("        }");

    // visitor
    buffer.AppendLine();
    buffer.AppendLine("        public override R Accept<R>(Visitor<R> visitor) {");
    buffer.AppendLine($"            return visitor.Visit{className}{baseName}(this);");
    buffer.AppendLine("        }");

    // fields
    buffer.AppendLine();
    foreach (var field in fields) {
        buffer.AppendLine($"        public {field}{{ get; }}");
    }

    buffer.AppendLine("    }");
}
