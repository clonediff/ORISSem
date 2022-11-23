using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal class HTMLNode : IASTNode
    {
        public Token Token { get; }

        public VariableStorage Variables { get; }

        public HTMLNode(Token token)
            => Token = token;

        public object Eval()
            => Token.Text;
    }
}
