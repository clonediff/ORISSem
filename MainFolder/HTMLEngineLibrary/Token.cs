namespace HTMLEngineLibrary
{
    record Token(TokenType Type, string Text, int Pos);
    
    internal enum TokenType
    {
        TEXT,
        NUMBER,
        VARIABLE,
        STRING_VALUE,

        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,

        IF,
        ELSE,
        ELIF,
        
        FOR,
        IN,

        END,

        BIGGER,
        BIGGER_OR_EQUAL,
        LESS,
        LESS_OR_EQUAL,

        EQUAL,
        NOT_EQUAL,

        AND,
        OR,

        INDEXER_START,
        INDEXER_END,

        ACCESSOR,

        ASSIGN,

        EXPR_END,

        LPAR,
        RPAR
    }
}
