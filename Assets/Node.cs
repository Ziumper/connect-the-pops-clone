using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using ZiumperExtensions;



public class Node : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
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
    public int Value;

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

}
