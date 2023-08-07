using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public GridGenerator gridGenerator;
    public PathManager pathManager;
    private List<Node> path;

    private void Start()
    {
        gridGenerator = GridGenerator.Instance;
        pathManager = PathManager.Instance;

        path = pathManager.GetPrimaryPath();

        Debug.Log("Enemy started. Calculated path: ");
        foreach (var node in path)
        {
            Debug.Log("Node: " + node.WorldPosition);
        }
    }

    void Update()
    {
        if (path == null || path.Count == 0)
        {
            if (pathManager.CurrentPathState == pathState.pathFound)
            {
                Debug.LogWarning("Path is null or empty, recalculating...");
                path = pathManager.GetPrimaryPath();
                return;
            }
            else
            {
                Debug.Log("No Path Generated!");
                return;
            }
        }
        Debug.Log("world positio: "+path[0].WorldPosition);
        transform.position = Vector3.MoveTowards(transform.position, path[0].WorldPosition, speed * Time.deltaTime);
        Debug.Log("DISTANCE: " + Vector3.Distance(transform.position, path[0].WorldPosition));
        if (Vector3.Distance(transform.position, path[0].WorldPosition) < 0.01f)
        {
            
            Debug.Log("Reached node: " + path[0].WorldPosition);
            path.RemoveAt(0);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(this.gameObject);
    }

    private Node FindClosestNodeOnPath(Vector3 position, List<Node> path)
    {
        float shortestDistance = Mathf.Infinity;
        Node closestNode = null;

        foreach (Node node in path)
        {
            float currentDistance = Vector3.Distance(position, node.WorldPosition);
            if (currentDistance < shortestDistance)
            {
                shortestDistance = currentDistance;
                closestNode = node;
            }
        }
        Debug.Log("Closest node to position " + position + " is: " + closestNode.WorldPosition);
        return closestNode;
    }

    private void TrimPathUntilNode(Node targetNode)
    {
        Debug.Log("Trimming path until node: " + targetNode.WorldPosition);
        while (path.Count > 0 && path[0] != targetNode)
        {
            path.RemoveAt(0);
        }
    }
}