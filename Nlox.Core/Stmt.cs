namespace Nlox.Core;

public abstract class Stmt {
    public interface Visitor<R> {
        R VisitBlockStmt(Block stmt);
        R VisitExpressionStmt(Expression stmt);
        R VisitIfStmt(If stmt);
        R VisitPrintStmt(Print stmt);
        R VisitVarStmt(Var stmt);
        R VisitWhileStmt(While stmt);
    }
    public class Block : Stmt {
        public Block(List<Stmt> Statements) {
                this.Statements = Statements;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitBlockStmt(this);
        }

        public List<Stmt> Statements{ get; }
    }

    public class Expression : Stmt {
        public Expression(Expr Expr) {
                this.Expr = Expr;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitExpressionStmt(this);
        }

        public Expr Expr{ get; }
    }

    public class If : Stmt {
        public If(Expr Condition, Stmt ThenBranch, Stmt? ElseBranch) {
                this.Condition = Condition;
                this.ThenBranch = ThenBranch;
                this.ElseBranch = ElseBranch;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitIfStmt(this);
        }

        public Expr Condition{ get; }
        public Stmt ThenBranch{ get; }
        public Stmt? ElseBranch{ get; }
    }

    public class Print : Stmt {
        public Print(Expr Expression) {
                this.Expression = Expression;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitPrintStmt(this);
        }

        public Expr Expression{ get; }
    }

    public class Var : Stmt {
        public Var(Token Name, Expr? Initializer) {
                this.Name = Name;
                this.Initializer = Initializer;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitVarStmt(this);
        }

        public Token Name{ get; }
        public Expr? Initializer{ get; }
    }

    public class While : Stmt {
        public While(Expr Condition, Stmt Body) {
                this.Condition = Condition;
                this.Body = Body;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitWhileStmt(this);
        }

        public Expr Condition{ get; }
        public Stmt Body{ get; }
    }


    public abstract R Accept<R>(Visitor<R> visitor);
}
