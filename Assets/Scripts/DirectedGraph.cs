using ExtensionsUtil;
using System.Collections.Generic;
using UnityEngine;

public class DirectedGraph : IDirectedGraph<Node, Vector2>
{
    public DirectedGraph()
    {
        vertices = new HashSet<Node>();
    }

    private readonly HashSet<Node> vertices;

    public int Size => vertices.Count;

    public void AddEdge(Node first, Node second, Vector2 direction)
    {
        AddToList(first);
        AddToList(second);
        AddNeighbors(first, second, direction);
    }

    public void RemoveEdge(Node first, Node second)
    {
        if (vertices.TryGetValue(first, out var firstActual) && vertices.TryGetValue(second, out var secondActual))
        {
            RemoveNeighbors(firstActual, secondActual);
            RemoveNeighbors(secondActual, firstActual);
        }
    }

    private void AddToList(Node vertex)
    {
        vertices.Add(vertex);
    }

    private void AddNeighbors(Node start, Node end, Vector2 direction)
    {
        if (vertices.TryGetValue(start, out var actualFirst))
        {
            start = actualFirst;
        }

        if (vertices.TryGetValue(end, out var actualSecond))
        {
            end = actualSecond;
        }

        var oppsite = direction.GetOpposite();
        AddNeighbor(start, end, direction);
        AddNeighbor(end, start, oppsite);
    }

    private void AddNeighbor(Node start, Node end, Vector2 direction)
    {
        if (!start.IsNeighbour(end))
        {
            start.AddEdge(direction, end);
        }
    }

    private void RemoveNeighbors(Node first, Node second)
    {
        RemoveNeighbour(first, second);
        RemoveNeighbour(second, first);
    }

    private void RemoveNeighbour(Node start, Node end)
    {
        if (start.IsNeighbour(end))
        {
            start.RemoveEdge(end);
        }
    }

    public void RemoveNode(Node node)
    {
        foreach(var vertex in vertices)
        {
            vertex.RemoveEdge(node);
        }

        vertices.Remove(node);
    }
}
