namespace Nlox.Core;

public class RuntimeError : Exception {
    public RuntimeError(Token token, string message) : base(message) {
        Token = token;
    }

    public Token Token { get; }
}

public class Interpreter : Expr.Visitor<object?> {
    private readonly TextWriter _writer;

    public Interpreter(TextWriter writer) {
        _writer = writer;
    }

    public void Interpret(Expr expression) {
        try {
            var value = Evaluate(expression);
            _writer.WriteLine(Stringify(value));
        }
        catch (RuntimeError error) {
            Lox.RuntimeError(error);
        }
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
