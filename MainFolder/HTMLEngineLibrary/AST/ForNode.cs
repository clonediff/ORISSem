using System.Text;

namespace HTMLEngineLibrary.AST
{
    internal class ForNode : IASTNode
    {
        public IASTNode Body { get; }
        public IASTNode IterationModel { get; }

        string NewVar { get; }

        public VariableStorage Variables { get; }

        public ForNode(VariableStorage variableStorage, string newVar, IASTNode iterationModel, IASTNode body)
        {
            Body = body;
            IterationModel = iterationModel;
            Variables = variableStorage;
            NewVar = newVar;
        }

        public object Eval()
        {
            var result = new StringBuilder();

            var model = IterationModel.Eval();

            foreach(var t in (dynamic)model)
            {
                Variables.SetVariable(NewVar, t);
                result.Append(Body.Eval().ToString()!);
            }

            return result.ToString();
        }
    }
}
