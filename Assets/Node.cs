using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Node : MonoBehaviour, IVertex<Node, Vector2>,
    IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable]
    public class NodeEvents
    {
        public UnityEvent<Node> OnNodeDown;
        public UnityEvent<Node> OnNodeUp;
        public UnityEvent<Node> OnNodeEnter;
        public UnityEvent<Node> OnNodeExit;
    }

    public NodeEvents Events;
    public Node Previous;
    public int Value;

    private Animator animator;
    private Dictionary<Node, Vector2> neighbours = new();
    private readonly string scaleCondition = "ScaleUp";
    
    public void Start()
    {
        animator = GetComponent<Animator>();
        Previous = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Events.OnNodeDown.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Events.OnNodeUp.Invoke (this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Events.OnNodeEnter.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Events.OnNodeExit.Invoke(this);
    }

    public void ScaleUp()
    {
        animator.SetBool(scaleCondition, true);
    }

    public void ScaleDown()
    {
        animator.SetBool(scaleCondition, false);
    }

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
}
