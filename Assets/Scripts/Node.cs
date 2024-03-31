using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Serializable]
    private class NodeArrow
    {
        public Image Image;
        public Vector2 Direction;
    }

    [SerializeField] private List<NodeArrow> nodeArrows;
    [SerializeField] private int nodeValue;
    [SerializeField] private TextMeshProUGUI valueText;

    private Animator animator;
    private Dictionary<Node, Vector2> neighbours = new();
    private Dictionary<Vector2, GameObject> arrowsMap = new();
    private readonly string scaleCondition = "ScaleUp";

    public NodeEvents Events;
    public Node Previous;
    public int Value
    {
        get { return nodeValue; } 
        set 
        {
            nodeValue = value;
            valueText.text = nodeValue.ToString();
        }
    }
    
    public void Start()
    {
        animator = GetComponentInParent<Animator>();
        Previous = null;

        Value = nodeValue;
        
        foreach(var arrow in nodeArrows)
        {
            arrowsMap.Add(arrow.Direction, arrow.Image.gameObject);
        }
    }

    public void ShowArrow(Vector2 direction)
    {
        if(arrowsMap.TryGetValue(direction, out GameObject arrow))
        {
            arrow.SetActive(true);
        }
    }

    public void HideArrow(Vector2 direction)
    {
        if(arrowsMap.TryGetValue(direction, out GameObject arrow)) 
        {
            arrow.SetActive(false);
        }
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

    public Vector2 GetDirectionToNeighbour(Node node)
    {
        if(neighbours.TryGetValue(node, out var direction)) {
            return direction;
        }

        return Vector2.zero;
    }

    public void HideAllArrows()
    {
        foreach(var nodeArrow in nodeArrows)
        {
            nodeArrow.Image.gameObject.SetActive(false);
        }
    }
}
