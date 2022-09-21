namespace Nlox.Core;

internal class ParserError : Exception { }

public class Parser {
    private readonly List<Token> _tokens;
    private int _current;

    public Parser(List<Token> tokens) {
        _tokens = tokens;
    }

    public List<Stmt> Parse() {
        var statements = new List<Stmt>();
        while (!IsAtEnd()) {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Stmt? Declaration() {
        try {
            if (Match(TokenType.VAR)) {
                return VarDeclaration();
            }

            return Statement();
        }
        catch (ParserError error) {
            Synchronize();
            return null;
        }
    }

    private Stmt VarDeclaration() {
        var name = Consume(TokenType.IDENTIFIER, "expect variable name");

        Expr? initializer = null;
        if (Match(TokenType.EQUAL)) {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "expect ';' after variable declaration");
        return new Stmt.Var(name, initializer);
    }

    private Stmt Statement() {
        if (Match(TokenType.IF)) {
            return IfStatement();
        }

        if (Match(TokenType.PRINT)) {
            return PrintStatement();
        }

        if (Match(TokenType.LEFT_BRACE)) {
            return new Stmt.Block(Block());
        }

        return ExpressionStatement();
    }

    private Stmt IfStatement() {
        Consume(TokenType.LEFT_PAREN, "expect '(' after 'if'");
        var condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "expect ')' after if condition");

        var thenBranch = Statement();
        Stmt? elseBranch = null;
        if (Match(TokenType.ELSE)) {
            elseBranch = Statement();
        }

        return new Stmt.If(condition, thenBranch, elseBranch);
    }

    private List<Stmt> Block() {
        List<Stmt> statements = new();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
            statements.Add(Declaration());
        }

        Consume(TokenType.RIGHT_BRACE, "expect '}' after block");
        return statements;
    }

    private Stmt ExpressionStatement() {
        var expr = Expression();
        Consume(TokenType.SEMICOLON, "expect ';' after value.");
        return new Stmt.Expression(expr);
    }

    private Stmt PrintStatement() {
        var value = Expression();
        Consume(TokenType.SEMICOLON, "expect ';' after value.");
        return new Stmt.Print(value);
    }

    private Expr Expression() {
        return Assignment();
    }

    private Expr Assignment() {
        var expr = Equality();

        if (Match(TokenType.EQUAL)) {
            var equals = Previous();
            var value = Assignment();

            if (expr is Expr.Variable variable) {
                var name = variable.Name;
                return new Expr.Assign(name, value);
            }

            Error(equals, "invalid assignment target");
        }

        return expr;
    }

    private Expr Equality() {
        var expr = Comparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
            var op = Previous();
            var right = Comparison();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Comparison() {
        var expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
            var op = Previous();
            var right = Term();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Term() {
        var expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS)) {
            var op = Previous();
            var right = Factor();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Factor() {
        var expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR)) {
            var op = Previous();
            var right = Unary();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Unary() {
        if (Match(TokenType.BANG, TokenType.MINUS)) {
            var op = Previous();
            var right = Unary();
            return new Expr.Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary() {
        if (Match(TokenType.FALSE)) {
            return new Expr.Literal(false);
        }

        if (Match(TokenType.TRUE)) {
            return new Expr.Literal(true);
        }

        if (Match(TokenType.NIL)) {
            return new Expr.Literal(null);
        }

        if (Match(TokenType.NUMBER, TokenType.STRING)) {
            return new Expr.Literal(Previous().Literal);
        }

        if (Match(TokenType.IDENTIFIER)) {
            return new Expr.Variable(Previous());
        }

        if (Match(TokenType.LEFT_PAREN)) {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "expect ')' after expression");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "expect expression");
    }

    private Token Consume(TokenType type, string message) {
        if (Check(type)) {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private ParserError Error(Token token, string message) {
        Lox.Error(token, message);
        return new ParserError();
    }

    private bool Match(params TokenType[] types) {
        foreach (var type in types) {
            if (Check(type)) {
                Advance();
                return true;
            }
        }

        return false;
    }

    private Token Advance() {
        if (!IsAtEnd()) {
            _current++;
        }

        return Previous();
    }

    private Token Previous() {
        return _tokens[_current - 1];
    }

    private bool IsAtEnd() {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek() {
        return _tokens[_current];
    }

    private bool Check(TokenType type) {
        if (IsAtEnd()) {
            return false;
        }

        return Peek().Type == type;
    }

    private void Synchronize() {
        Advance();

        while (!IsAtEnd()) {
            if (Previous().Type == TokenType.SEMICOLON) {
                return;
            }

            switch (Peek().Type) {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }
}
