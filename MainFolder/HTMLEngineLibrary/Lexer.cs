using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HTMLEngineLibrary
{
    internal class Lexer
    {
        static Dictionary<string, TokenType> OperationTokenTypes = new()
        {
            ["<="] = TokenType.LESS_OR_EQUAL,
            [">="] = TokenType.BIGGER_OR_EQUAL,
            ["=="] = TokenType.EQUAL,
            ["!="] = TokenType.NOT_EQUAL,

            ["if"] = TokenType.IF,
            ["else"] = TokenType.ELSE,
            ["elif"] = TokenType.ELIF,
            ["end"] = TokenType.END,

            ["and"] = TokenType.AND,
            ["or"] = TokenType.OR,

            ["for"] = TokenType.FOR,
            ["in"] = TokenType.IN,

            ["="] = TokenType.ASSIGN,

            ["+"] = TokenType.PLUS,
            ["-"] = TokenType.MINUS,
            ["*"] = TokenType.MULTIPLY,
            ["/"] = TokenType.DIVIDE,
            ["."] = TokenType.ACCESSOR,
            ["["] = TokenType.INDEXER_START,
            ["]"] = TokenType.INDEXER_END,
            ["<"] = TokenType.LESS,
            [">"] = TokenType.BIGGER,
            ["("] = TokenType.LPAR,
            [")"] = TokenType.RPAR
        };

        private string Input { get; }
        private List<Token> Tokens { get; } = new List<Token>();
        private int Pos { get; set; }

        public Lexer(string input) => Input = input;

        public List<Token> Tokenize()
        {
            while (!IsEnd())
            {
                if (CheckNext("{{")) 
                    foreach (var token in TokenizeExpr())
                        Tokens.Add(token);
                else 
                    Tokens.Add(TokenizeText());
            }

            return Tokens;
        }

        private List<Token> TokenizeExpr()
        {
            Pos += 2;
            var result = new List<Token>();
            while (!IsEnd() && !CheckNext("}}"))
            {
                var operationFound = false;
                foreach (var (key, type) in OperationTokenTypes)
                    if (CheckNext(key))
                    {
                        result.Add(new Token(type, key, Pos));
                        Pos += key.Length;
                        operationFound = true;
                        break;
                    }
                if (!operationFound)
                {
                    if (char.IsDigit(Get(0))) result.Add(TokenizeNumber());
                    else if (char.IsLetter(Get(0))) result.Add(TokenizeVariable());
                    else if (Get(0) == '"') result.Add(TokenizeStringValue());
                    else if (char.IsWhiteSpace(Get(0))) Pos++;
                    else
                        throw new Exception($"Неизвестный символ на позиции {Pos}");
                }
            }

            if (IsEnd() || !CheckNext("}}"))
                throw new Exception("Ожидалась закрывающая скобка }}");

            result.Add(new Token(TokenType.EXPR_END, "}}", Pos));
            Pos += 2;   

            return result;
        }

        private Token TokenizeStringValue()
        {
            var startPos = Pos;
            Pos++;
            var sb = new StringBuilder();
            while (!IsEnd() && Get(0) != '"')
            {
                if (Get(0) == '\\')
                {
                    Pos++;
                    sb.Append(Get(0) switch
                    {
                        'n' => '\n',
                        't' => '\t',
                        '\\' => '\\',
                        '"' => '\"'
                    });
                }
                else sb.Append(Get(0));
                Pos++;
            }
            if (IsEnd())
                throw new Exception("Ожидалась закрывающая кавычка \"");
            Pos++;

            return new Token(TokenType.STRING_VALUE, sb.ToString(), startPos);
        }

        private Token TokenizeVariable()
        {
            var startPos = Pos;
            var sb = new StringBuilder();
            while(char.IsLetterOrDigit(Get(0)) || Get(0) == '_')
            {
                sb.Append(Get(0));
                Pos++;
            }
            return new Token(TokenType.VARIABLE, sb.ToString(), startPos);
        }

        private Token TokenizeNumber()
        {
            var startPos = Pos;
            var sb = new StringBuilder();
            var containsPoint = false;
            while (true)
            {
                if (Get(0) == '.')
                {
                    if (containsPoint) throw new Exception($"Неправильное число на позиции {startPos}");
                    containsPoint = true;
                }
                else if (!char.IsDigit(Get(0)))
                    break;
                sb.Append(Get(0));
                Pos++;
            }
            return new Token(TokenType.NUMBER, sb.ToString(), startPos);
        }

        private Token TokenizeText()
        {
            var startPos = Pos;
            var sb = new StringBuilder();
            while (!IsEnd() && !CheckNext("{{"))
            {
                sb.Append(Get(0));
                Pos++;
            }
            return new Token(TokenType.TEXT, sb.ToString(), startPos);
        }

        bool IsEnd()
            => Get(0) == '\0';

        char Get(int rel)
        {
            int position = Pos + rel;
            if (position >= Input.Length) return '\0';
            return Input[position];
        }

        bool CheckNext(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (Get(i) != str[i]) return false;
            return true;
        }
    }
}
