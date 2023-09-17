using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Pathfinding
{
    private GridGenerator grid;
    private List<Node> openSetGizmo = new List<Node>();
    private HashSet<Node> closedSetGizmo = new HashSet<Node>();
    public bool debugMode = true;
    

    public Pathfinding(GridGenerator grid)
    {
        this.grid = grid;
    }

    public List<Node> CalculatePath(Node startNode, Node targetNode)
    {
        if (debugMode) Debug.Log("Starting Path Calculation...");

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
            if (debugMode) Debug.Log($"Current Node: {currentNode.Position}");

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // For visualization
            openSetGizmo = openSet;
            closedSetGizmo = closedSet;

            if (currentNode == targetNode)
            {
                if (debugMode) Debug.Log("Target Node Reached! Path Found!");
                return RetracePath(startNode, targetNode);
            }

            NodeScript currentNodeScript = grid.GetNodeScriptAtGridPosition(new Vector2Int((int)currentNode.Position.x, (int)currentNode.Position.z));
            foreach (NodeScript neighbourScript in grid.GetNeighbours(currentNodeScript))
            {
                Node neighbour = neighbourScript.node;
                if (debugMode) Debug.Log($"Evaluating Neighbour: {neighbour.Position}");

                if (!neighbour.Walkable)
                {
                    if (debugMode) Debug.Log($"Neighbour at {neighbour.Position} is not walkable. Skipping...");
                    continue;
                }

                if (closedSet.Contains(neighbour))
                {
                    if (debugMode) Debug.Log($"Neighbour at {neighbour.Position} is already evaluated. Skipping...");
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    if (debugMode) Debug.Log($"Updating Neighbour Costs at {neighbour.Position}. GCost: {neighbour.GCost}, HCost: {neighbour.HCost}");

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                        if (debugMode) Debug.Log($"Adding Neighbour at {neighbour.Position} to Open Set.");
                    }
                }
            }
        }

        // No path found
        if (debugMode) Debug.Log("Path Calculation Complete. No Path Found.");
        return null;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = (int)Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        int dstZ = (int)Mathf.Abs(nodeA.Position.z - nodeB.Position.z);
        return 10 * (dstX + dstZ);
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
    public List<Node> PathMutator(List<Node> mainPath)
    {
        List<Node> mutatedPath = new List<Node>(mainPath);

        int numberOfMutations = Random.Range(1, mainPath.Count / 4); // Adjust as needed

        for (int i = 0; i < numberOfMutations; i++)
        {
            int randomIndex = Random.Range(1, mainPath.Count - 1); // Exclude start and end nodes
            Node nodeToMutate = mainPath[randomIndex];

            List<Node> alternativeNodes = GetAlternativeNodes(nodeToMutate, mainPath);
            if (alternativeNodes.Count > 0)
            {
                Node replacementNode = alternativeNodes[Random.Range(0, alternativeNodes.Count)];
                mutatedPath[randomIndex] = replacementNode;

                // Recompute the path between the previous node and the next node
                List<Node> recomputedSubpath = AStarWithBias(mutatedPath[randomIndex - 1], mutatedPath[randomIndex + 1], mainPath);
                mutatedPath.InsertRange(randomIndex, recomputedSubpath);
            }
        }

        return mutatedPath;
    }

    private List<Node> GetAlternativeNodes(Node node, List<Node> mainPath)
    {
        List<Node> alternativeNodes = new List<Node>();
        NodeScript nodeScript = grid.GetNodeScriptAtGridPosition(new Vector2Int((int)node.Position.x, (int)node.Position.z));
        foreach (NodeScript neighbourScript in grid.GetNeighbours(nodeScript))
        {
            Node neighbour = neighbourScript.node;
            if (!mainPath.Contains(neighbour) && neighbour.Walkable)
            {
                alternativeNodes.Add(neighbour);
            }
        }
        return alternativeNodes;
    }

    private List<Node> AStarWithBias(Node startNode, Node targetNode, List<Node> mainPath)
    {
        // This is a modified version of your A* function that introduces a bias against nodes in the main path.
        // You can introduce this bias in the cost calculation or in the node selection process.
        // For simplicity, I'll just provide a placeholder here. You can modify it based on your game's requirements.
        // ...
        return new List<Node>(); // Placeholder return
    }
}
