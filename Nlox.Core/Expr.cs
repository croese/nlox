namespace Nlox.Core;

public abstract class Expr {
    public interface Visitor<R> {
        R VisitBinaryExpr(Binary expr);
        R VisitGroupingExpr(Grouping expr);
        R VisitLiteralExpr(Literal expr);
        R VisitUnaryExpr(Unary expr);
    }
    public class Binary : Expr {
        Binary(Expr Left, Token Operator, Expr Right) {
                this.Left = Left;
                this.Operator = Operator;
                this.Right = Right;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitBinaryExpr(this);
        }

        public Expr Left{ get; }
        public Token Operator{ get; }
        public Expr Right{ get; }
    }

    public class Grouping : Expr {
        Grouping(Expr Expression) {
                this.Expression = Expression;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitGroupingExpr(this);
        }

        public Expr Expression{ get; }
    }

    public class Literal : Expr {
        Literal(Object Value) {
                this.Value = Value;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitLiteralExpr(this);
        }

        public Object Value{ get; }
    }

    public class Unary : Expr {
        Unary(Token Operator, Expr Right) {
                this.Operator = Operator;
                this.Right = Right;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitUnaryExpr(this);
        }

        public Token Operator{ get; }
        public Expr Right{ get; }
    }


    public abstract R Accept<R>(Visitor<R> visitor);
}
