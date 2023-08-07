using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Pathfinding
{
    private GridGenerator grid;
    private List<Node> openSetGizmo = new List<Node>();
    private HashSet<Node> closedSetGizmo = new HashSet<Node>();
    public bool debugMode = false;
    

    public Pathfinding(GridGenerator grid)
    {
        this.grid = grid;
    }

    public List<Node> CalculatePath(Node startNode, Node targetNode)
    {
        if (debugMode) Debug.Log("Calculating Path...");

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                {
                    currentNode = openSet[i];
                }
            }
            if (debugMode) Debug.Log($"Evaluating node at {currentNode.Position}");

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // For visualization
            openSetGizmo = openSet;
            closedSetGizmo = closedSet;

            if (currentNode == targetNode)
            {
                if (debugMode) Debug.Log("Path found!");
                return RetracePath(startNode, targetNode);
            }

            foreach (NodeScript neighbourScript in grid.GetNeighbours(grid.GetNodeScriptAtPosition(currentNode.Position)))
            {
                Node neighbour = neighbourScript.node;
                if (debugMode) Debug.Log($"Checking neighbour at {neighbour.Position}");

                if (!neighbour.Walkable)
                {
                    if (debugMode) Debug.Log($"Neighbour at {neighbour.Position} is not walkable.");
                    continue;
                }

                if (closedSet.Contains(neighbour))
                {
                    if (debugMode) Debug.Log($"Neighbour at {neighbour.Position} is already in the closed set.");
                    continue;
                }


                int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    if (debugMode) Debug.Log($"Updated neighbour at {neighbour.Position} with GCost {neighbour.GCost}, HCost {neighbour.HCost}, and Parent at {currentNode.Position}");

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                        if (debugMode) Debug.Log($"Added neighbour at {neighbour.Position} to open set.");
                    }
                }
            }
        }

        // No path found
        if (debugMode) Debug.Log("No path found.");
        return null;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = (int)Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        int dstZ = (int)Mathf.Abs(nodeA.Position.z - nodeB.Position.z);

        if (dstX > dstZ)
            return 14 * dstZ + 10 * (dstX - dstZ);
        return 14 * dstX + 10 * (dstZ - dstX);
    }


    public void OnDrawGizmos()
    {
        if (!debugMode) return;

        // Draw open set in blue
        Gizmos.color = Color.blue;
        foreach (Node n in openSetGizmo)
        {
            Gizmos.DrawCube(n.Position, Vector3.one);
        }

        // Draw closed set in red
        Gizmos.color = Color.red;
        foreach (Node n in closedSetGizmo)
        {
            Gizmos.DrawCube(n.Position, Vector3.one);
        }
    }
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }

}
