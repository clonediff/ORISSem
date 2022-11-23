using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal class SubstractNode : BinaryNode
    {
        public SubstractNode(IASTNode left, IASTNode right) : base(left, right)
        { }

        public override object Eval()
        {
            var left = Left.Eval();
            var right = Right.Eval();

            return (dynamic)left - (dynamic)right;
        }
    }
}
