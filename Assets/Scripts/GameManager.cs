using ExtensionsUtil;
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
        public Node First;
        public Node Current;
        public Node Previous;
    }

    [Serializable]
    public class GameManagerEvents
    {
        public UnityEvent<int> OnSelectedResultUpdate;
    }
    
    [SerializeField] private GameState state;
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
                var node = nodeGameObject.GetComponentInChildren<Node>(); //get node component
                node.name = node.name + "." + position.name; //set readable name

                //add listeners
                node.Events.OnNodeEnter.AddListener(OnNodeEnter); 
                node.Events.OnNodeUp.AddListener(OnNodeUp);
                node.Events.OnNodeDown.AddListener(OnNodeDown);
                node.Events.OnNodeExit.AddListener(OnNodeExit);

                //add it to the spawned list
                state.Spawned.Add(node);
                grid[row][column] = node;               
            }
        }


        //here all needed nodes have been created so we can start with checking and adding neighbours
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
        if(state.First == null)
        {
            if (settings.Debug) { Debug.Log("Node has been down let's do it!", node); }
            state.First = node;
            state.Current = node;
            node.ScaleUp();
            entered.Add(node);
            UpdateSelected();
        }
    }

    private void OnNodeUp(Node node)
    {
        if(state.First != null)
        {
            if (settings.Debug) { Debug.Log("Node has been up, stop checking!", node); }
            state.First = null;
            state.Current = null;
            state.Previous = null;
            
            foreach(var enteredNode in entered)
            {
                enteredNode.ScaleDown();
                enteredNode.HideAllArrows();   
            }
            
            entered.Clear();
            UpdateSelected();
        }
    }

    private void OnNodeEnter(Node node)
    {
        if (settings.Debug) { Debug.Log("Node entered", node.gameObject); }

        if (state.First != null) //only when first is on
        {
            //switch currently selected node if it's neighbour
            if (state.Current.IsNeighbour(node))
            {
                Vector2 directionToNeighbour = state.Current.GetDirectionToNeighbour(node);
                //when coming to entered previous one remove selected from entered ones!
                if (entered.TryGetValue(node, out var enteredNode)) //is inside entered nodes
                {
                    if(node != state.Current && state.Current.Previous == node) //the same as the one that we were coming from
                    {
                        state.Current.ScaleDown();
                        entered.Remove(state.Current);

                        state.Current.HideArrow(directionToNeighbour);
                        node.HideArrow(directionToNeighbour.GetOpposite());

                        UpdateSelected();
                        
                        state.Current.Previous = null; //reset current previous node
                        state.Current = node; // set entered node as current
                        //previous one is inide current already from history
                        state.Previous = state.Current.Previous; //we display it only for debug
                    }
                }
                else //coming to fresh one!
                {
                    state.Current.ShowArrow(directionToNeighbour);
                    node.ShowArrow(directionToNeighbour.GetOpposite());

                    node.ScaleUp();
                    entered.Add(node);
                    UpdateSelected();
                    node.Previous = state.Current;
                    state.Current = node;
                    state.Previous = state.Current.Previous;
                }
            }
        }
    }

    private void OnNodeExit(Node node)
    {
        //so far not needed
    }

    private void UpdateSelected()
    {
        state.Selected = entered.ToList();

        int sum = 0;

        foreach(var node in state.Selected)
        {
            sum += node.Value;
        }

        state.SelectedResult = sum;
        events.OnSelectedResultUpdate.Invoke(state.SelectedResult);
    }
}
