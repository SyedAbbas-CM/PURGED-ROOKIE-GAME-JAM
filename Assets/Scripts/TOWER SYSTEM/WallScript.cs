using UnityEngine;

public class Wall : PlaceableItem
{
    private NodeScript currentNode;

    private void Start()
    {
        MarkNodeAsUnwalkable();
    }

    private void MarkNodeAsUnwalkable()
    {
        // Get the NodeScript we are standing on
        currentNode = GridGenerator.Instance.GetNodeFromWorldPoint(transform.position);

        if (currentNode && currentNode.node != null)
        {
            currentNode.node.Walkable = false;   // Make the node unwalkable
            currentNode.isOccupied = true;  // Mark the node as occupied
        }
    }

    // Ensure that when the wall is destroyed, the node becomes walkable again and is unoccupied.
    private void OnDestroy()
    {
        if (currentNode && currentNode.node != null)
        {
            currentNode.node.Walkable = true;    // Make the node walkable
            currentNode.isOccupied = false; // Mark the node as unoccupied
        }
    }
}