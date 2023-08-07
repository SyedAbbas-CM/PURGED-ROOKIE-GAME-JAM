using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float heightOffset = 0.5f; // To raise the enemy above the grid
    public GridGenerator gridGenerator;
    public PathManager pathManager;
    private List<Node> path;

    private void Start()
    {
        gridGenerator = GridGenerator.Instance;
        pathManager = PathManager.Instance;

        path = pathManager.GetPrimaryPath();
    }

    void Update()
    {
        if (path == null || path.Count == 0)
        {
            if (pathManager.CurrentPathState == pathState.pathFound)
            {
                path = pathManager.GetPrimaryPath();
                return;
            }
            else
            {
                Debug.Log("No Path Generated!");
                return;
            }
        }

        Vector3 targetPosition = path[0].WorldPosition + Vector3.up * heightOffset; // Adjusted for height above grid
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Enemy Facing Direction
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Check if it's the last node in the path
            if (path.Count == 1)
            {
                Die();
                return;
            }

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
        return closestNode;
    }

    private void TrimPathUntilNode(Node targetNode)
    {
        while (path.Count > 0 && path[0] != targetNode)
        {
            path.RemoveAt(0);
        }
    }
}