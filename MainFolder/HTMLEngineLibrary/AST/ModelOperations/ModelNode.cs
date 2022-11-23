using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal class ModelNode : IASTNode
    {
        object Model { get; }

        public VariableStorage Variables { get; }
        public string ModelName { get; }

        public ModelNode(VariableStorage variables, string name)
        {
            Variables = variables;
            ModelName = name;
        }

        public ModelNode(object model)
        {
            Model = model;
        }
        public object Eval()
            => Variables is null ?
            Model :
            Variables.GetVariable(ModelName);
    }
}
