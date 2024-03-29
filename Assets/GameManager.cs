using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public class Settings
    {
        public int Amount;
        public GameObject NodePrefab;
        public List<Transform> Positions;
        public bool Debug;
    }

    [Serializable]
    public class GameState
    {
        public List<Node> Spawned;
        public List<Node> Selected;
        public int SelectedResult;
        public Node FirstSelectedNode;
        public Node CurrentlySelectedNode;
    }

    [Serializable]
    public class GameManagerEvents
    {
        public UnityEvent<int> OnSelectedResultUpdate;
    }
    
    [SerializeField] private GameState gameState;
    [SerializeField] private Settings settings;
    [SerializeField] private GameManagerEvents events;

    private HashSet<Node> entered;
  
    private void Start()
    {
        entered = new HashSet<Node>();

        for(int i = 0; i < settings.Amount; i++)
        {
            var node = Instantiate(settings.NodePrefab, settings.Positions[i]).GetComponent<Node>();
            
            node.Events.OnNodeEnter.AddListener(OnNodeEnter);
            node.Events.OnNodeUp.AddListener(OnNodeUp);
            node.Events.OnNodeDown.AddListener(OnNodeDown);
            node.Events.OnNodeExit.AddListener(OnNodeExit);

            gameState.Spawned.Add(node);
        }
    }

    private void OnNodeDown(Node node)
    {
        if(gameState.FirstSelectedNode == null)
        {
            if (settings.Debug) { Debug.Log("Node has been down let's do it!", node); }
            gameState.FirstSelectedNode = node;
            entered.Add(node);
            UpdateSelected();
        }
    }

    private void OnNodeUp(Node node)
    {
        if(gameState.FirstSelectedNode != null)
        {

            if (settings.Debug) { Debug.Log("Node has been up, stop checking!", node); }
            gameState.FirstSelectedNode = null;
            entered.Clear();
            UpdateSelected();
        }
    }

    private void OnNodeEnter(Node node)
    {
        if(gameState.FirstSelectedNode != null)
        {
            
            if (settings.Debug) { Debug.Log("Node entered add it to queue"); }
            entered.Add(node);
            UpdateSelected();
        }
    }

    private void OnNodeExit(Node node)
    {
        //if(gameState.FirstSelectedNode != null && node != gameState.FirstSelectedNode && entered.TryGetValue(node, out var nodeFound)) 
        //{
          
        //}   
    }

    private void UpdateSelected()
    {
        gameState.Selected = entered.ToList();

        int sum = 0;

        foreach(var node in gameState.Selected)
        {
            sum += node.Value;
        }

        gameState.SelectedResult = sum;
        events.OnSelectedResultUpdate.Invoke(gameState.SelectedResult);
    }
}
