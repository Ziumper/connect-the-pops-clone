using ExtensionsUtil;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeValue : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable]
    public class NodeEvents
    {
        public UnityEvent<NodeValue> OnNodeDown;
        public UnityEvent<NodeValue> OnNodeUp;
        public UnityEvent<NodeValue> OnNodeEnter;
        public UnityEvent<NodeValue> OnNodeExit;
        public UnityEvent<NodeValue> OnNodeOnDestroyPosition;
        public UnityEvent<NodeValue> OnNodeFinishedMoving;
        public UnityEvent<NodeValue> OnNodeValueOnDestroy;

        public void RemoveAllListeners()
        {
            OnNodeDown.RemoveAllListeners();
            OnNodeUp.RemoveAllListeners();
            OnNodeEnter.RemoveAllListeners();
            OnNodeExit.RemoveAllListeners();
            OnNodeOnDestroyPosition.RemoveAllListeners();
            OnNodeFinishedMoving.RemoveAllListeners();
        }
    }

    [Serializable]
    private class NodeArrow
    {
        public Image Image;
        public Vector2 Direction;
    }

    [Header("Display")]
    [SerializeField] private List<NodeArrow> nodeArrows;
    [SerializeField] private int nodeValue;
    [SerializeField] private TextMeshProUGUI valueText;

    [Header("Moving variables")]
    [SerializeField] private bool shouldMove;
    [SerializeField] private bool isToDestroy;
    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private Vector3 initaliPosition;
    [SerializeField] private Transform target;
    [SerializeField] private AnimationClip moveAnimation;

    private Animator animator;
    private Dictionary<Vector2, GameObject> arrowsMap = new();

 
    private readonly string scaleCondition = "ScaleUp";
    private readonly string moveTrigger = "Move";

    [Header("Events in NodeValue")]
    public NodeEvents Events;
    
    public int Value
    {
        get { return nodeValue; } 
        set 
        {
            nodeValue = value;
            valueText.text = nodeValue.ToString();
        }
    }
    
    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        Value = nodeValue;
        initaliPosition = transform.parent.position;
        
        foreach(var arrow in nodeArrows)
        {
            arrowsMap.Add(arrow.Direction, arrow.Image.gameObject);
        }
    }

    private void Update()
    {
        if(shouldMove)
        {
            MoveTowardsTarget();
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

    public void HideAllArrows()
    {
        foreach(var nodeArrow in nodeArrows)
        {
            nodeArrow.Image.gameObject.SetActive(false);
        }
    }


    private void MoveTowardsTarget()
    {
        elapsedTime += Time.deltaTime;
        float part = Mathf.Clamp01(elapsedTime / moveAnimation.length);
        transform.parent.position = Vector3.Lerp(initaliPosition, target.position, part);

        if (part >= 1f)
        {
            elapsedTime = 0f;
            shouldMove = false;
            initaliPosition = transform.parent.position;

            if(isToDestroy)
            {
                Events.OnNodeOnDestroyPosition.Invoke(this);
            }

            Events.OnNodeFinishedMoving.Invoke(this);
        }
    }

    public void StartDestroyMoving(Transform target)
    {
        StartMovingTowardsTarget(target);
        isToDestroy = true;
    }

    public void StartMovingTowardsTarget(Transform target)
    {
        animator.SetTrigger(moveTrigger);
        this.target = target;
        shouldMove = true;
    }

    public void OnDestroy()
    {
        Events.OnNodeValueOnDestroy.Invoke(this);
    }
}
