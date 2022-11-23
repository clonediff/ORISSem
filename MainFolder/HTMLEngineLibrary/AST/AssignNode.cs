namespace HTMLEngineLibrary.AST
{
    internal class AssignNode : IASTNode
    {
        string VarName { get; }
        IASTNode VarValue { get; }

        public VariableStorage Variables { get; }

        public AssignNode(VariableStorage variables, string name, IASTNode value)
        {
            VarName = name;
            VarValue = value;
            Variables = variables;
        }

        public object Eval()
        {
            Variables.SetVariable(VarName, VarValue.Eval());
            return null!;
        }
    }
}
