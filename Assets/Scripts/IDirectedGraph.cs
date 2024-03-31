public interface IDirectedGraph<TNode, TDirection>
{
    public void RemoveEdge(TNode first, TNode second);
    /// <summary>
    /// Is adding edge to both verticles
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void AddEdge(TNode start, TNode end, TDirection direction);
    public void RemoveNode(TNode node);
    public int Size { get; }
}
