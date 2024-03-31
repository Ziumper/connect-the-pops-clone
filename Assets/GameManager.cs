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
    public class PositionRow
    {
        public List<Transform> Columns;
    }

    [Serializable]
    public class Settings
    {
        public GameObject NodePrefab;
        public List<PositionRow> Rows;
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
    private DirectedGraph graph;
  
    private void Start()
    {
        entered = new HashSet<Node>();
        graph = new DirectedGraph();

        
        Node[][] grid = new Node[settings.Rows.Count][];
        
        //instantiate nodes
        for(int row = 0; row < settings.Rows.Count; row++)
        {
            grid[row] = new Node[settings.Rows[row].Columns.Count];
            
            for(int column = 0; column < settings.Rows[row].Columns.Count; column++)
            {
                var position = settings.Rows[row].Columns[column]; //getPosition
                var nodeGameObject = Instantiate(settings.NodePrefab,position); //make a game object with position as parent
                var node = nodeGameObject.GetComponent<Node>(); //get node component
                node.name = node.name + "." + position.name; //set readable name

                //add listeners
                node.Events.OnNodeEnter.AddListener(OnNodeEnter); 
                node.Events.OnNodeUp.AddListener(OnNodeUp);
                node.Events.OnNodeDown.AddListener(OnNodeDown);
                node.Events.OnNodeExit.AddListener(OnNodeExit);

                //add it to the spawned list
                gameState.Spawned.Add(node);
                grid[row][column] = node;               
            }
        }


        //here all needed nodes were created so we can start with checking and adding neighbours
        //handle adding neighbours
        /**
         *  it goes like this where x is a center point node
         *  (-1,-1),(0,-1),(1,-1)
         *  (-1,0),   x   ,(1,0)
         *  (-1,1), (0,1) ,(1,1)
         *
         */
        Vector2[] directions =
        {
            new (-1,-1),  new(0,-1),  new (1,-1),
            new (-1,0),               new (1,0),
            new (-1,1),   new(0,1),   new (1,1)
        };

        for (int row = 0; row < settings.Rows.Count; row++)
        {
            for(int column = 0; column < settings.Rows[row].Columns.Count; column++)
            {
                var node = grid[row][column];
                foreach(var direction in directions)
                {
                    var resultVector = new Vector2(column, row) + direction;
                    bool insideGrid = resultVector.x >= 0 && resultVector.x < settings.Rows[column].Columns.Count
                        && resultVector.y >= 0 && resultVector.y < settings.Rows.Count;
                    if (insideGrid)
                    {
                        var neighbourNode = grid[(int)resultVector.y][(int)resultVector.x];
                        graph.AddEdge(node, neighbourNode, direction);
                    }
                }
            }
        }
    }

    private void OnNodeDown(Node node)
    {
        if(gameState.FirstSelectedNode == null)
        {
            if (settings.Debug) { Debug.Log("Node has been down let's do it!", node); }
            gameState.FirstSelectedNode = node;
            gameState.CurrentlySelectedNode = node;
            node.ScaleUp();
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
            
            foreach(var enteredNode in entered)
            {
                enteredNode.ScaleDown();
            }
            
            entered.Clear();
            UpdateSelected();
        }
    }

    private void OnNodeEnter(Node node)
    {
        if(gameState.FirstSelectedNode != null)
        {   
            if (settings.Debug) { Debug.Log("Node entered add it to queue"); }
            
            if(gameState.CurrentlySelectedNode == null)
            {
                node.ScaleUp();
                entered.Add(node);
            } else if (gameState.CurrentlySelectedNode.IsNeighbour(node))
            {
                gameState.CurrentlySelectedNode = node;
            }

            UpdateSelected();
            node.ScaleUp();
            entered.Add(node);
        }
    }

    private void OnNodeExit(Node node)
    {
        if (gameState.FirstSelectedNode != null && node != gameState.FirstSelectedNode && gameState.CurrentlySelectedNode == node)
        {
            if(entered.TryGetValue(node, out var nodeFound))
            {
                nodeFound.ScaleDown();
                entered.Remove(nodeFound);
                gameState.CurrentlySelectedNode = null;
                UpdateSelected();
            }
        }
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
