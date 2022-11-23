using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal class InvertNode : UnaryNode
    {
        public InvertNode(IASTNode operand) : base(operand)
        { }

        public override object Eval()
            => -(dynamic)Operand.Eval();
    }
}
