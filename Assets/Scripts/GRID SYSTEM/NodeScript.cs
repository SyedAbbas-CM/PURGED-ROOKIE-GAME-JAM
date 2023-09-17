using UnityEditor;
using UnityEngine;
using System.Collections;

public class NodeScript : MonoBehaviour
{
    public Node node;
    public bool isOccupied;




    private Renderer nodeRenderer;
    public bool IsOccupied
    {
        get
        {
            return isOccupied;
        }
        set
        {
            isOccupied = value;
        }
    }
    void Awake()
    {


        nodeRenderer = GetComponent<MeshRenderer>();
        if (nodeRenderer == null)
        {
            Debug.LogError("No MeshRenderer found on this node.");
        }
    }
    public void HandleSelection(TowerManager towerManager)
    {
        if (node == null)
        {
            Debug.LogError("Node is null in this NodeScript.");
            return; // Exit early to avoid further errors
        }

        if (!IsOccupied)
        {
            Debug.Log("Node is not occupied. Trying to place item.");

            // Based on the placement type, place a tower or a wall
            if (towerManager.currentPlacementType == TowerManager.PlacementType.Tower)
            {
                towerManager.PlaceTower(this.transform.position, this.gameObject);
            }
            else if (towerManager.currentPlacementType == TowerManager.PlacementType.Wall)
            {
                towerManager.PlaceWall(this.transform.position, this.gameObject);
            }

            IsOccupied = true;
            node.Walkable = false;
        }
        else
        {
            Debug.LogWarning("Node is already occupied. Placement failed.");
        }
    }
    public void SetWalkable(bool value)
    {
        node.Walkable = value;
    }
    private void OnMouseDown()
    {
        Debug.Log("Node clicked");
    }
    public float X
    {
        get { return node.Position.x; }
        set { node.Position = new Vector3(value, node.Position.y, node.Position.z); }
    }

    public float Y
    {
        get { return node.Position.y; }
        set { node.Position = new Vector3(node.Position.x, value, node.Position.z); }
    }

    void OnDrawGizmos()
    {
        if (node == null) return;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        Handles.Label(transform.position + new Vector3(0.5f, 1f, 0.5f), $"({node.Position.x}, {node.Position.y}, {node.Position.z})", style);
    }
    public void ShowOutline()
    {
        nodeRenderer.enabled = true;
    }

    public void HideOutline()
    {
        nodeRenderer.enabled = false;
    }
    public void RequestTowerRemoval()
    {
        if (IsOccupied)
        {
            RemoveTowerOrWall();
        }
    }
    private void RemoveTowerOrWall()
    {
        IsOccupied = false;
        SetWalkable(true);

        TowerManager.Instance.RemoveTowerAt(this.transform.position); // Adjusted this line
    }

    public IEnumerator BreakWall()
    {
        // Change the wall's color to red
        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }

        // Wait for a delay of 1-2 seconds
        yield return new WaitForSeconds(1.5f); // You can adjust this delay to be 1 or 2 seconds based on your preference

        // Remove the wall
        TowerManager.Instance.RemoveWallAt(this.transform.position);
    }
}