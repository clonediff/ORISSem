using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST.BinaryNodes
{
    internal class DivideNode : BinaryNode
    {
        public DivideNode(IASTNode left, IASTNode right) : base(left, right)
        { }

        public override object Eval()
            => (dynamic)Left.Eval() / (dynamic)Right.Eval();
    }
}
