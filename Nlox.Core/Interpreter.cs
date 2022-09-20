namespace Nlox.Core;

public class RuntimeError : Exception {
    public RuntimeError(Token token, string message) : base(message) {
        Token = token;
    }

    public Token Token { get; }
}

public class Interpreter : Expr.Visitor<object?>, Stmt.Visitor<ValueTuple> {
    private readonly TextWriter _writer;
    private Environment _environment = new();

    public Interpreter(TextWriter writer) {
        _writer = writer;
    }

    public object? VisitAssignExpr(Expr.Assign expr) {
        var value = Evaluate(expr.Value);
        _environment.Assign(expr.Name, value);
        return value;
    }

    public object? VisitBinaryExpr(Expr.Binary expr) {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type) {
            case TokenType.MINUS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! - (double)right!;
            case TokenType.SLASH:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! / (double)right!;
            case TokenType.STAR:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! * (double)right!;
            case TokenType.PLUS:
                if (left is double ld && right is double rd) {
                    return ld + rd;
                }

                if (left is string ls && right is string rs) {
                    return ls + rs;
                }

                throw new RuntimeError(expr.Operator, "operands must be two numbers or two strings");
            case TokenType.GREATER:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! > (double)right!;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! >= (double)right!;
            case TokenType.LESS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! < (double)right!;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! <= (double)right!;
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
        }

        return null;
    }

    public object? VisitGroupingExpr(Expr.Grouping expr) {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteralExpr(Expr.Literal expr) {
        return expr.Value;
    }

    public object? VisitUnaryExpr(Expr.Unary expr) {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type) {
            case TokenType.MINUS:
                CheckNumberOperand(expr.Operator, right);
                return -(double)right!;
            case TokenType.BANG:
                return !IsTruthy(right);
        }

        return null;
    }

    public object? VisitVariableExpr(Expr.Variable expr) {
        return _environment.Get(expr.Name);
    }

    public ValueTuple VisitBlockStmt(Stmt.Block stmt) {
        ExecuteBlock(stmt.Statements, new Environment(_environment));
        return ValueTuple.Create();
    }

    public ValueTuple VisitExpressionStmt(Stmt.Expression stmt) {
        Evaluate(stmt.Expr);
        return ValueTuple.Create();
    }

    public ValueTuple VisitPrintStmt(Stmt.Print stmt) {
        var value = Evaluate(stmt.Expression);
        _writer.WriteLine(Stringify(value));
        return ValueTuple.Create();
    }

    public ValueTuple VisitVarStmt(Stmt.Var stmt) {
        object? value = null;
        if (stmt.Initializer != null) {
            value = Evaluate(stmt.Initializer);
        }

        _environment.Define(stmt.Name.Lexeme, value);
        return ValueTuple.Create();
    }

    private void ExecuteBlock(List<Stmt> statements, Environment environment) {
        var previous = _environment;
        try {
            _environment = environment;

            foreach (var statement in statements) {
                Execute(statement);
            }
        }
        finally {
            _environment = previous;
        }
    }

    public void Interpret(List<Stmt> statements) {
        try {
            foreach (var statement in statements) {
                Execute(statement);
            }
        }
        catch (RuntimeError error) {
            Lox.RuntimeError(error);
        }
    }

    private void Execute(Stmt statement) {
        statement.Accept(this);
    }

    private string Stringify(object? value) {
        switch (value) {
            case null:
                return "nil";
            case double: {
                var text = value.ToString();
                if (text.EndsWith(".0")) {
                    text = text.Substring(0, text.Length - 2);
                }

                return text;
            }
            default:
                return value.ToString()!;
        }
    }

    private void CheckNumberOperand(Token op, object? operand) {
        if (operand is double) {
            return;
        }

        throw new RuntimeError(op, "operand must be a number");
    }

    private void CheckNumberOperands(Token op, object? left, object? right) {
        if (left is double && right is double) {
            return;
        }

        throw new RuntimeError(op, "operands must be a numbers");
    }

    private bool IsEqual(object? a, object? b) {
        if (a == null && b == null) {
            return true;
        }

        if (a == null) {
            return false;
        }

        return a.Equals(b);
    }

    private bool IsTruthy(object? obj) {
        return obj switch {
            null => false,
            bool b => b,
            _ => true
        };
    }

    private object? Evaluate(Expr expr) {
        return expr.Accept(this);
    }
}
