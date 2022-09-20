namespace Nlox.Core;

public abstract class Expr {
    public interface Visitor<R> {
        R VisitAssignExpr(Assign expr);
        R VisitBinaryExpr(Binary expr);
        R VisitGroupingExpr(Grouping expr);
        R VisitLiteralExpr(Literal expr);
        R VisitUnaryExpr(Unary expr);
        R VisitVariableExpr(Variable expr);
    }
    public class Assign : Expr {
        public Assign(Token Name, Expr Value) {
                this.Name = Name;
                this.Value = Value;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitAssignExpr(this);
        }

        public Token Name{ get; }
        public Expr Value{ get; }
    }

    public class Binary : Expr {
        public Binary(Expr Left, Token Operator, Expr Right) {
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
        public Grouping(Expr Expression) {
                this.Expression = Expression;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitGroupingExpr(this);
        }

        public Expr Expression{ get; }
    }

    public class Literal : Expr {
        public Literal(Object? Value) {
                this.Value = Value;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitLiteralExpr(this);
        }

        public Object? Value{ get; }
    }

    public class Unary : Expr {
        public Unary(Token Operator, Expr Right) {
                this.Operator = Operator;
                this.Right = Right;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitUnaryExpr(this);
        }

        public Token Operator{ get; }
        public Expr Right{ get; }
    }

    public class Variable : Expr {
        public Variable(Token Name) {
                this.Name = Name;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitVariableExpr(this);
        }

        public Token Name{ get; }
    }


    public abstract R Accept<R>(Visitor<R> visitor);
}
