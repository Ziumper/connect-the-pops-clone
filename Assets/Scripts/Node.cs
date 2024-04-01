using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, IVertex<Node, Vector2>
{
    private Dictionary<Node, Vector2> neighbours = new();
    public Node Previous;
   
    public bool IsNeighbour(Node node)
    {
        if (neighbours.ContainsKey(node))
        {
            return true;
        }

        return false;
    }

    public void AddEdge(Vector2 direction, Node vertex)
    {
        neighbours.Add(vertex, direction);
    }

    public void RemoveEdge(Node vertex)
    {
        neighbours.Remove(vertex);
    }

    public Vector2 GetDirectionToNeighbour(Node node)
    {
        if (neighbours.TryGetValue(node, out var direction))
        {
            return direction;
        }

        return Vector2.zero;
    }

}
