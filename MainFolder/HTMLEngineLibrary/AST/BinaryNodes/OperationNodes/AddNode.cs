namespace HTMLEngineLibrary.AST
{
    internal class AddNode : BinaryNode
    {
        public AddNode(IASTNode left, IASTNode right) : base(left, right)
        { }

        public override object Eval()
        {
            return (dynamic)Left.Eval() + (dynamic)Right.Eval();
        }
    }
}
