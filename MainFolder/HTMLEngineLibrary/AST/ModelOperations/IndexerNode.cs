using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal class IndexerNode : IASTNode
    {
        IASTNode Model { get; }
        IASTNode index { get; }

        public VariableStorage Variables { get; }

        string[] indexerMethods = new[] { "get_Item", "get_Chars" };

        public IndexerNode(IASTNode model, IASTNode index)
        {
            Model = model;
            this.index = index;
        }

        public object? Eval()
        {
            var model = Model.Eval();
            var indexer = model?.GetType().GetMethods().FirstOrDefault(x => indexerMethods.Any(y => y == x.Name)) ??
                model?.GetType().GetInterface("IList")?.GetMethod("get_Item");
            var parameters = indexer?.GetParameters();
            if (indexer is null ||
                parameters.Length != 1)
                throw new Exception($"У {model} не существует индексатора");

            return indexer.Invoke(model, new object[] { Convert.ChangeType(index.Eval(), parameters[0].ParameterType) });
        }
    }
}
