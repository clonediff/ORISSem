using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal class ComplexNode : IASTNode
    {
        List<IASTNode> nodes = new();
        public VariableStorage Variables { get; }

        public ComplexNode(VariableStorage storage)
        {
            Variables = storage;
        }

        public void AddNode(IASTNode node)
            => nodes.Add(node);

        public object Eval()
        {
            var result = new StringBuilder();
            nodes.ForEach(x => result.Append(x?.Eval()));
            return result.ToString();
        }
    }
}
