using System.Text;

namespace Nlox.Core;

public class AstPrinter : Expr.Visitor<string> {
    public string VisitBinaryExpr(Expr.Binary expr) {
        return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(Expr.Grouping expr) {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Expr.Literal expr) {
        return expr.Value?.ToString() ?? "nil";
    }

    public string VisitUnaryExpr(Expr.Unary expr) {
        return Parenthesize(expr.Operator.Lexeme, expr.Right);
    }

    public string Print(Expr expr) {
        return expr.Accept(this);
    }

    private string Parenthesize(string name, params Expr[] exprs) {
        var buffer = new StringBuilder();

        buffer.Append('(').Append(name);
        foreach (var expr in exprs) {
            buffer.Append(' ');
            buffer.Append(expr.Accept(this));
        }

        buffer.Append(')');

        return buffer.ToString();
    }
}
