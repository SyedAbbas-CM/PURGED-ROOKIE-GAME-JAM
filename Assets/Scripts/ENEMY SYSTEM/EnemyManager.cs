using UnityEngine;
using System.Collections;
public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GridGenerator gridGenerator; // Assign this from the inspector

    private Vector3 spawnPosition;
    public bool isActive = false;


    public void Activate()
    {
        isActive = true;
        // Spawn your first enemy, or start your spawning routine
        spawnPosition = gridGenerator.GetStartNode().transform.position;
        Debug.Log("A wave is incoming!");
        StartCoroutine(SpawnEnemiesRoutine());
    }

    void SpawnEnemy()
    {
        if (isActive)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            // You might want to have more spawning logic or other behaviors here
        }
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        // This is just a simple example, you can modify the logic as per your requirements
        for (int i = 0; i < 1; i++) // Spawning 5 enemies as an example
        {
            SpawnEnemy();

            yield return new WaitForSeconds(1f); // Wait for 1 second between enemy spawns
        }
    }
}





