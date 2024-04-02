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
        public UnityEvent<NodeValue> OnNodeValueDown;
        public UnityEvent<NodeValue> OnNodeValueUp;
        public UnityEvent<NodeValue> OnNodeValueEnter;
        public UnityEvent<NodeValue> OnNodeValueExit;
        public UnityEvent<NodeValue> OnNodeValueOnDestroyPosition;
        public UnityEvent<NodeValue> OnNodeValueFinishedMoving;
        public UnityEvent<NodeValue> OnNodeValueOnDestroy;

        public void RemoveAllListeners()
        {
            OnNodeValueDown.RemoveAllListeners();
            OnNodeValueUp.RemoveAllListeners();
            OnNodeValueEnter.RemoveAllListeners();
            OnNodeValueExit.RemoveAllListeners();
            OnNodeValueOnDestroyPosition.RemoveAllListeners();
            OnNodeValueFinishedMoving.RemoveAllListeners();
        }
    }

    [Serializable]
    private class NodeArrow
    {
        public Image Image;
        public Vector2 Direction;
    }

    [SerializeField] private bool shouldMove;
    [SerializeField] private bool isToDestroy;
    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private Vector3 initaliPosition;
    [SerializeField] private int nodeValue;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private ColorTemplate template;
    [SerializeField] private Image circleIn;
    [SerializeField] private Transform target;
    [SerializeField] private AnimationClip moveAnimation;
    [SerializeField] private List<NodeArrow> nodeArrows;

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
            if(value <= 0)
            {
                nodeValue = 0;
                return;
            }

            nodeValue = ClosestTwoPowerRound(value);
            var color = template.GetColorForValue(nodeValue);

            GetComponent<Image>().color = color;
            foreach(var arrow in nodeArrows)
            {
                arrow.Image.color = color;
            }

            //toggle circle in
            int power = GetClosesTwoPower(value);
            ToggleCircleIn(power, circleIn);
                
            valueText.text = NodeValueToString(nodeValue);
        }
    }

    private void ToggleCircleIn(int power, Image circleImage)
    {
        circleImage.gameObject.SetActive(power >= GameManager.Settings.MaxPowerSpliter);
    }

    private int ClosestTwoPowerRound(int value)
    {
        return (int)Mathf.Pow(2, GetClosesTwoPower(value));
    }

    private int GetClosesTwoPower(int value)
    {
        if (value <= 0)
        {
            return 0;
        }
            
        float logBase2 = Mathf.Log(value, 2);
        return Mathf.FloorToInt(logBase2);
    }
    
    private string NodeValueToString(int value)
    {
        int power = GetClosesTwoPower(value);
        if(power >= GameManager.Settings.MaxPowerSpliter)
        {
            string valueToString = (value / GameManager.Settings.MaxPowerSpliting).ToString() + GameManager.Settings.SpliterSufixAppend;
            return valueToString;
        }

        return value.ToString();
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
        Events.OnNodeValueDown.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Events.OnNodeValueUp.Invoke (this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Events.OnNodeValueEnter.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Events.OnNodeValueExit.Invoke(this);
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
                Events.OnNodeValueOnDestroyPosition.Invoke(this);
            }

            Events.OnNodeValueFinishedMoving.Invoke(this);
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
