using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public enum GridState
{
    NotGenerated,
    Generated
}

public class GridGenerator : Singleton<GridGenerator>
{
    private Camera mainCamera;
    private Vector3 cameraCenter;
    public GameObject nodePrefab; // The node prefab
    public int width = 10; // Width of the grid
    public int height = 10; // Height of the grid
    private GameObject nodeHolder;
    private NodeScript[,] grid; // 2D array of nodes
    public Dictionary<Node, NodeScript> nodeDictionary = new Dictionary<Node, NodeScript>();
    private NodeScript startNode;
    private NodeScript endNode;
    [Header("Start and End Nodes")]
    public Vector2Int startNodePosition = new Vector2Int(0, 0); // Default bottom-left
    public Vector2Int endNodePosition = new Vector2Int(0, 9);   // Default top-left\
    public PathManager pathManager;
    public EnemyManager enemyManager;
    public GridState CurrentState { get; private set; } = GridState.NotGenerated;


    void Start()
    {
        mainCamera = Camera.main;
        nodeHolder = new GameObject("NodeHolder");
        nodeHolder.transform.position = Vector3.zero;
        grid = new NodeScript[width, height];
        GenerateGrid(width, height);
    }
    public void GenerateGrid(int W, int H)
    {
        Vector3 offset = new Vector3(width / 2.0f, 0, height / 2.0f);
        for (int x = 0; x < W; x++)
        {
            for (int y = 0; y < H; y++)
            {
                // Instantiate a new node at the current position
                Vector3 instantiatePosition = new Vector3(x*2, 0, y*2) - offset;
                GameObject newNode = Instantiate(nodePrefab, instantiatePosition, Quaternion.identity);


                // Make the node a child of nodeHolder
                newNode.transform.parent = nodeHolder.transform;

                // Add a NodeScript component to the new node
                NodeScript nodeScript = newNode.AddComponent<NodeScript>();
                nodeScript.node = new Node
                {
                    Position = new Vector3(x, 0, y),
                    Walkable = true,
                    WorldPosition = newNode.transform.position
                };

                // Add the node to the grid
                grid[x, y] = nodeScript;

                // Add the node and NodeScript to the dictionary
                nodeDictionary[nodeScript.node] = nodeScript;

                // Make the node invisible
                //newNode.GetComponent<Renderer>().enabled = false;
            }
        }
        startNode = grid[startNodePosition.x, startNodePosition.y];
        endNode = grid[endNodePosition.x, endNodePosition.y];
        startNode.GetComponent<Renderer>().enabled = true;
        endNode.GetComponent<Renderer>().enabled = true;
        startNode.GetComponent<Renderer>().material.color = Color.green;
        endNode.GetComponent<Renderer>().material.color = Color.red;
        startNode.canPlace = false;
        endNode.canPlace = false;

        CurrentState = GridState.Generated;

        // Notify systems that are interested in the grid's generation completion
        OnGridGenerated();

    }

    public List<NodeScript> GetNeighbours(NodeScript nodeScript)
    {
        List<NodeScript> neighbours = new List<NodeScript>();

        int currentX = (int)nodeScript.node.Position.x;
        int currentZ = (int)nodeScript.node.Position.z;

        // Check North
        if (currentZ + 1 < height)
        {
            neighbours.Add(grid[currentX, currentZ + 1]);
        }
        // Check South
        if (currentZ - 1 >= 0)
        {
            neighbours.Add(grid[currentX, currentZ - 1]);
        }
        // Check East
        if (currentX + 1 < width)
        {
            neighbours.Add(grid[currentX + 1, currentZ]);
        }
        // Check West
        if (currentX - 1 >= 0)
        {
            neighbours.Add(grid[currentX - 1, currentZ]);
        }

        return neighbours;
    }

    public NodeScript GetNodeScriptAtNode(Node node)
    {
        if (nodeDictionary.ContainsKey(node))
        {
            return nodeDictionary[node];
        }
        else
        {
            Debug.LogError("Node not found in dictionary");
            return null;
        }
    }


    void expandGrid()
    {

    }
    public NodeScript GetStartNode()
    {
        return startNode;
    }

    public NodeScript GetEndNode()
    {
        return endNode;
    }

    void OnGridGenerated()
    {
        startNode.node.GCost = 0;
        startNode.node.Walkable = true;
        endNode.node.Walkable = true;
        pathManager.CalculatePrimaryPath();
        Camera.main.GetComponent<TopDownCameraController>().InitializeCamera();
        enemyManager.getstartPosition();
    }
    public float GetGridWidth()
    {
        return width;
    }

    public float GetGridDepth()
    {
        return height;
    }

    public Vector3 GetGridCenter()
    {
        return nodeHolder.transform.position;
    }
    public NodeScript GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        // Assuming the node's X and Z coordinates correspond to the world position's X and Z
        int x = Mathf.FloorToInt(worldPosition.x + width / 2.0f);  // Adjusting for the offset
        int z = Mathf.FloorToInt(worldPosition.z + height / 2.0f); // Adjusting for the offset

        // Check boundary conditions
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return grid[x, z];
        }

        return null; // or handle this situation accordingly
    }
    public NodeScript GetNodeAtPosition(int x, int y)
    {
        // Assuming your grid data is stored in a 2D array called nodes.
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return grid[x, y].GetComponent<NodeScript>();
        }
        return null;
    }
    public NodeScript GetNodeScriptAtPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x + width) / 2.0f); // Adjust for the node's spacing
        int z = Mathf.FloorToInt((position.z + height) / 2.0f);

        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return grid[x, z];
        }
        else
        {
            Debug.LogError("Position out of grid bounds");
            return null;
        }
    }
}
