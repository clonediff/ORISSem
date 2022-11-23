using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal abstract class UnaryNode : IASTNode
    {
        protected IASTNode Operand { get; }

        public VariableStorage Variables { get; }

        public UnaryNode(IASTNode operand)
            => Operand = operand;

        public abstract object Eval();
    }
}
