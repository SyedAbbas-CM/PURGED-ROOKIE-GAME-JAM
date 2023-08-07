using UnityEditor;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    public Node node;
    private bool isOccupied = false;

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

    public void HandleSelection(TowerManager towerManager)
    {
        if (!IsOccupied)
        {
            Debug.Log("Node is not occupied. Trying to place tower.");

            towerManager.PlaceTower(this.transform.position, this.gameObject);
            IsOccupied = true;
            node.Walkable = false;
        }
        else
        {
            Debug.LogWarning("Node is already occupied. Tower placement failed.");
        }
    }
    public void SetWalkable(bool value)
    {
        node.Walkable = value;
    }
    public override string ToString()
    {
        return $"Node at Grid Position: ({node.Position.x}, {node.Position.y}), Walkable: {node.Walkable}";
    }
    private void OnMouseDown()
    {
        Debug.Log($"Node clicked: {this.ToString()}");
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
}