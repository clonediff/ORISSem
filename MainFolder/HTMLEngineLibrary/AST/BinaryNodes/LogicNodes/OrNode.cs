using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST.BinaryNodes.LogicNodes
{
    internal class OrNode : BinaryNode
    {
        public OrNode(IASTNode left, IASTNode right) : base(left, right)
        { }

        public override object Eval()
            => (dynamic)Left.Eval() || (dynamic)Right.Eval();
    }
}
