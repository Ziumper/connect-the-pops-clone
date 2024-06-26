﻿public interface IVertex<TNode, TDirection>
{
    void AddEdge(TDirection direction, TNode vertex);
    void RemoveEdge(TNode vertex);
    bool IsNeighbour(TNode node);
    TDirection GetDirectionToNeighbour(TNode node);
    bool HasNeighoburWithValue(TDirection direction);
}
