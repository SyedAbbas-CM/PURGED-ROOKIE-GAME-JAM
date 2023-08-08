using UnityEngine;
using System.Collections;


public enum WaveState
{
    Active,
    InActive
}
public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject enemyPrefab;
    public GridGenerator gridGenerator; // Assign this from the inspector

    public WaveState CurrentWaveState { get; private set; } = WaveState.InActive;
    private Vector3 spawnPosition;
    public bool isActive = false;
    public int enemyCount { set; get; }

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
        CurrentWaveState = WaveState.Active;
        for (int i = 0; i < 1; i++) // Spawning 5 enemies as an example
        {
            SpawnEnemy();
            enemyCount++;
            yield return new WaitForSeconds(1f); // Wait for 1 second between enemy spawns
        }
    }
    private void Update()
    {
        if(enemyCount <= 0)
        {
            CurrentWaveState = WaveState.InActive;
        }
    }
}





