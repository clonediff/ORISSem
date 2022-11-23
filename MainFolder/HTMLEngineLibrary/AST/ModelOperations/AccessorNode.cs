using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal class AccessorNode : IASTNode
    {
        IASTNode Model { get; }
        string Property { get; }

        public VariableStorage Variables { get; }

        public AccessorNode(IASTNode model, string property)
        {
            Model = model;
            Property = property;
        }

        public dynamic? Eval()
        {
            var model = Model.Eval();
            var type = model?.GetType();
            var property = type!.GetProperty(Property);
            if (property is null)
                throw new ArgumentException($"Не существует свойства {Property} у {model}");
            var value = property?.GetValue(model);
            return value;
        }
    }
}
