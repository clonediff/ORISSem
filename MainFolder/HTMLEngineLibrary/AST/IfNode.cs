using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal class IfNode : IASTNode
    {
        public IASTNode Condition { get; }
        public IASTNode Then { get; }
        public IASTNode Else { get; }

        public VariableStorage Variables { get; }

        public IfNode(VariableStorage storage, IASTNode condition, IASTNode then, IASTNode @else = null)
        {
            Condition = condition;
            Then = then;
            Else = @else;
            Variables = storage;
        }

        public object Eval()
        {
            if ((dynamic)Condition.Eval())
                return Then.Eval();
            return Else?.Eval();
        }
    }
}
