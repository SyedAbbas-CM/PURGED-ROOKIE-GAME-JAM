using System.Collections.Generic;
using UnityEngine;
public enum pathState
{
    pathFound,
    pathNotUpdated,
    pathUpdated,
    pathNotFound

}



public class PathManager : Singleton<PathManager>
{


    public GridGenerator gridGenerator;
    public pathState CurrentPathState { get; private set; } = pathState.pathNotFound;
    private List<Node> primaryPath;
    private Pathfinding pathfinding;
    public EnemyManager enemyManager;


    private void Awake()
    {
        Debug.Log("Path Manager Awake");
        gridGenerator = GridGenerator.Instance;
        pathfinding = new Pathfinding(gridGenerator);
        Debug.Log("PathManager Awake. GridGenerator and Pathfinding initialized.");
    }

    private void Start()
    {
        Debug.Log("PathManager Start. Re-initialized Pathfinding.");
        if (gridGenerator.CurrentState == GridState.Generated)
        {
            if (primaryPath == null)
            {
                Debug.Log("Start thing idk");
                //CalculatePrimaryPath();
            }
        }
    }

    //private void Update()
    //{
    //    if (gridGenerator.CurrentState == GridState.Generated)
    //    {
    //        if (primaryPath == null)
    //        {
    //            Debug.Log("Calculating primary path");
    //            CalculatePrimaryPath();
    //        }
    //    }
    //}

    public void CalculatePrimaryPath()
    {
        NodeScript startNodeScript = gridGenerator.GetStartNode();
        NodeScript endNodeScript = gridGenerator.GetEndNode();

        if (startNodeScript != null && endNodeScript != null)
        {
            Debug.Log("Start Node: " + startNodeScript.node.Position + ", End Node: " + endNodeScript.node.Position);
            primaryPath = pathfinding.CalculatePath(startNodeScript.node, endNodeScript.node);

            if (primaryPath == null)
            {
                Debug.LogError("Failed to calculate a valid primary path.");
                return;
            }

            Debug.Log("Primary path calculated:");
            this.CurrentPathState = pathState.pathFound;
            foreach (var node in primaryPath)
            {
                Debug.Log("Node: " + node.Position+" World Position: "+node.WorldPosition);
            }
            onPathGenerated();
        }
        else
        {
            Debug.LogError("Start or End Node is null in GridGenerator. Can't calculate path.");
        }
    }

    public List<Node> GetPrimaryPath()
    {
        Node startNode = gridGenerator.GetStartNode().node;
        Node endNode = gridGenerator.GetEndNode().node;

        if (startNode != null && endNode != null)
        {
            Debug.Log("Getting primary path between: Start Node (" + startNode.Position + ") and End Node (" + endNode.Position + ")");
        }
        else
        {
            Debug.LogError("Failed to get primary path: Start or End Node is null.");
        }

        List<Node> path = pathfinding.CalculatePath(startNode, endNode);
        Debug.Log("Path calculated with " + path.Count + " nodes.");
        return path;
    }
    void onPathGenerated()
    {
        //enemyManager.Activate();
    }
}