using ExtensionsUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
    public class GameManagerEvents
    {
        public UnityEvent<int> OnSelectedResultUpdate;
    }


    private HashSet<NodeValue> active;
    private DirectedGraph graph;
    private Node[][] grid;
    private List<NodeValue> destroyList;

    private readonly Vector2[] directions =
        {
            new (-1,-1),  new(0,-1),  new (1,-1),
            new (-1,0),               new (1,0),
            new (-1,1),   new(0,1),   new (1,1)
        };

    [SerializeField] private GameState state;
    [SerializeField] private Settings settings;
    [SerializeField] private GameManagerEvents events;

    private void Start()
    {
        active = new HashSet<NodeValue>();
        graph = new DirectedGraph();
        grid = InstatiateNodes();
        destroyList = new List<NodeValue>();

        AddNeighboursForNodes(grid);
    }

    
    private NodeValue CreateNodeValue(Node spotNode)
    {
        var nodeGameObject = Instantiate(settings.NodePrefab, spotNode.transform); //make a game object with position as parent
        var value = nodeGameObject.GetComponentInChildren<NodeValue>(); //get node value component
        value.name = value.name + "." + spotNode.name; //set readable name

        //add listeners
        value.Events.OnNodeEnter.AddListener(OnNodeEnter);
        value.Events.OnNodeUp.AddListener(OnNodeValueUp);
        value.Events.OnNodeDown.AddListener(OnNodeValueDown);
        value.Events.OnNodeExit.AddListener(OnNodeExit);
     

        return value;
    }

    private Node[][] InstatiateNodes()
    {
        Node[][] grid = new Node[settings.Rows.Count][];

        //instantiate nodes
        for (int row = 0; row < settings.Rows.Count; row++)
        {
            grid[row] = new Node[settings.Rows[row].Columns.Count];

            for (int column = 0; column < settings.Rows[row].Columns.Count; column++)
            {
                var node = settings.Rows[row].Columns[column].GetComponent<Node>();
                CreateNodeValue(node);

                grid[row][column] = node;
            }
        }

        return grid;
    }

    private void AddNeighboursForNodes(Node[][] grid)
    {
        for (int row = 0; row < settings.Rows.Count; row++)
        {
            for (int column = 0; column < settings.Rows[row].Columns.Count; column++)
            {
                var node = grid[row][column];
                foreach (var direction in directions)
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

    private void OnNodeValueDown(NodeValue value)
    {
        if (state.First == null)
        {
            if (settings.Debug) { Debug.Log("Node has been down let's do it!", value); }
            state.First = value.GetComponentInParent<Node>();
            state.Last = value.GetComponentInParent<Node>();
            value.ScaleUp();
            active.Add(value);
            UpdateSelected();
        }
    }

    private void OnNodeValueUp(NodeValue node)
    {
        if (state.First != null)
        {
            if (settings.Debug) { Debug.Log("Node has been up, stop checking!", node); }

            foreach (var enteredNode in active)
            {
                enteredNode.ScaleDown();
                enteredNode.HideAllArrows();
            }

            //if at least two let's change the value of the first, destroy rest and spawn next ones 
            if (active.Count > 1)
            {
                var entered = active.ToArray();
                var lastValue = state.Last.GetComponentInChildren<NodeValue>();

                //get the last one should be the one that last!
                for (int i = entered.Length - 2; i >= 0; i--)
                {
                    var enteredNodeValue = entered[i];

                    destroyList.Add(enteredNodeValue);
                    enteredNodeValue.Events.OnNodeOnDestroyPosition.AddListener(OnNodeValueDestroyPosition);

                    //start moving
                    enteredNodeValue.StartDestroyMoving(state.Last.transform);
                }

                //now just make it through entire grid and move all empty spaces to fill up

            }

            //first cleanup
            state.First = null;
            state.Previous = null;
            
            active.Clear();
            UpdateSelected();
        }
    }

    private void OnNodeEnter(NodeValue value)
    {
        if (settings.Debug) { Debug.Log("Node entered", value.gameObject); }

        if (state.First != null) //only when first is on
        {
            //switch currently selected node if it's neighbour
            var node = value.GetComponentInParent<Node>();
            if (state.Last.IsNeighbour(node))
            {
                Vector2 directionToNeighbour = state.Last.GetDirectionToNeighbour(node);
                //when coming to entered previous one remove selected from entered ones!
                if (active.TryGetValue(value, out var enteredNode)) //is inside entered nodes
                {
                    if (value != state.Last && state.Last.Previous == node) //the same as the one that we were coming from
                    {
                        var lastValue = state.Last.GetComponentInChildren<NodeValue>();
                        lastValue.ScaleDown();
                        active.Remove(lastValue);

                        lastValue.HideArrow(directionToNeighbour);
                        value.HideArrow(directionToNeighbour.GetOpposite());

                        UpdateSelected();

                        state.Last.Previous = null; //reset current previous node
                        state.Last = node; // set entered node as current
                        //previous one is inide current already from history
                        state.Previous = state.Last.Previous; //we display it only for debug
                    }
                }
                else //coming to fresh one!
                {
                    var lastValue = state.Last.GetComponentInChildren<NodeValue>();
                    lastValue.ShowArrow(directionToNeighbour);
                    value.ShowArrow(directionToNeighbour.GetOpposite());

                    value.ScaleUp();
                    active.Add(value);
                    UpdateSelected();
                    node.Previous = state.Last;
                    state.Last = node;
                    state.Previous = state.Last.Previous;
                }
            }
        }
    }

    private void OnNodeExit(NodeValue value)
    {
        //so far not needed
    }

    private void OnNodeValueDestroyPosition(NodeValue value)
    {
        //destroy and clenaup
        //cleanup
        var enteredNode = value.GetComponentInParent<Node>();
        enteredNode.Previous = null;

        var lastValue = state.Last.GetComponentInChildren<NodeValue>();
        lastValue.Value += value.Value;
        value.Events.RemoveAllListeners();

        value.Events.OnNodeValueOnDestroy.AddListener(OnNodeValueOnDestroy);

        var containerNode = enteredNode.transform.GetChild(0).gameObject;

        Destroy(containerNode);
    }

    private void OnNodeValueOnDestroy(NodeValue value)
    {
        if (settings.Debug)
        {
            Debug.Log("Finished destroying");
        }
        
        destroyList.Remove(value);
        if(destroyList.Count == 0)
        {
            StartCoroutine(SpawnNewOnesInNextFrame());
        }
    }

    private IEnumerator SpawnNewOnesInNextFrame() 
    {
        yield return new WaitForEndOfFrame();
        state.Last = null;
        if (settings.Debug) { Debug.Log("All nodes have been destroyed"); }

        int columnsLength = settings.Rows[0].Columns.Count;
        var upDirection = Vector2.down;
        var downDirection = upDirection.GetOpposite();

        Dictionary<Node, NodeValue> moveDictionary = new Dictionary<Node, NodeValue>();
        Dictionary<int, List<int>> emptyNodesIndexes = new();

        //fill up the move dictionary and empty nodes indexes
        for (int column = 0; column < columnsLength; column++)
        {
            int moveCounter = 0;
            for (int indexOfRow = settings.Rows.Count - 1; indexOfRow >= 0; indexOfRow--)
            {
                var current = grid[indexOfRow][column];
                var nodeValueToMove = current.GetComponentInChildren<NodeValue>();
                bool hasValueToMove = nodeValueToMove != null;
                if (moveCounter > 0 && hasValueToMove)
                {
                    var moveCurrent = current;
                    for (int i = 0; i < moveCounter; i++)
                    {
                        moveCurrent = moveCurrent.GetNeighbourWithDirection(downDirection);
                    }

                    //here we moved enough times    
                    moveDictionary.Add(moveCurrent, nodeValueToMove);
                }

                if (!hasValueToMove) //it does not have any value 
                {
                    moveCounter++;
                }
            }

            List<int> empty = new List<int>();
            //fill up empty nodes 
            for (int emptyRow = 0; emptyRow < moveCounter; emptyRow++)
            {
                empty.Add(emptyRow);
            }

            emptyNodesIndexes.Add(column, empty);
        }

        //move the nodes
        foreach (var node in moveDictionary.Keys)
        {
            var moveValueNode = moveDictionary[node];
            moveValueNode.gameObject.transform.parent.transform.SetParent(node.gameObject.transform);
            moveValueNode.StartMovingTowardsTarget(node.transform);
        }

    }


    private void UpdateSelected()
    {
        var listOfValues = new List<NodeValue>();

        int sum = 0;
        foreach (var selected in active)
        {
            listOfValues.Add(selected.GetComponentInChildren<NodeValue>());
            sum += selected.Value;
        }

        state.SelectedResult = sum;
        events.OnSelectedResultUpdate.Invoke(state.SelectedResult);
    }
}
