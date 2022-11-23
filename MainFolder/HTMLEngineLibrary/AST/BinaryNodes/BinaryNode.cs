using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal abstract class BinaryNode : IASTNode
    {
        protected IASTNode Left { get; }
        protected IASTNode Right { get; }

        public VariableStorage Variables { get; }

        public BinaryNode(IASTNode left, IASTNode right)
        {
            Left = left;
            Right = right;
        }

        public abstract object Eval();
    }
}
