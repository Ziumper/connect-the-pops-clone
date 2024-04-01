using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour, IVertex<Node, Vector2>
{
    private Dictionary<Node, Vector2> neighbours = new();
    private Dictionary<Vector2, Node> reverseNeighboursMap = new();
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
        reverseNeighboursMap.Add(direction, vertex);
    }

    public void RemoveEdge(Node vertex)
    {
        if(neighbours.TryGetValue(vertex, out var direction))
        {
            neighbours.Remove(vertex);
            reverseNeighboursMap.Remove(direction);
        }
    }

    public Vector2 GetDirectionToNeighbour(Node node)
    {
        if (neighbours.TryGetValue(node, out var direction))
        {
            return direction;
        }

        return Vector2.zero;
    }

    public bool HasNeighoburWithValue(Vector2 direction)
    {
        if(reverseNeighboursMap.TryGetValue(direction, out var neighbour))
        {
            var value = neighbour.GetComponentInChildren<NodeValue>();
            return value != null;
        }

        return false;
    }

    public Node GetNeighbourWithDirection(Vector2 direction)
    {
        if(reverseNeighboursMap.TryGetValue(direction,out var node)) 
        {
            return node;
        }

        return null;
    }
}
