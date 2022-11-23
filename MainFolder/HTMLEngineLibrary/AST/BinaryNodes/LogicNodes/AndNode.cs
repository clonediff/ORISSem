namespace HTMLEngineLibrary.AST.BinaryNodes.LogicNodes
{
    internal class AndNode : BinaryNode
    {
        public AndNode(IASTNode left, IASTNode right) : base(left, right)
        { }

        public override object Eval()
            => (dynamic)Left.Eval() && (dynamic)Right.Eval();
    }
}
