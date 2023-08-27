using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public int count;
    public float delayBetweenSpawns;
    public float probability = 1.0f; // Use this to control the probability of spawning this type of enemy in the wave.
}

[System.Serializable]
public class WaveData
{
    public List<EnemyWave> enemyWaves;
    public float delayBetweenWaves;
}

[System.Serializable]
public class SpecialSpawn
{
    public GameObject specialEnemyPrefab;
    public int waveNumber; // On which wave this special spawn should appear
    public float delayAfterWaveStarts; // How many seconds after the wave starts should this enemy appear.
}

public enum WaveState
{
    NotStarted,
    Spawning,
    WaveActive,
    BetweenWaves,
    ExtendedBreak,
    Completed
}

public class EnemyManager : Singleton<EnemyManager>
{
    public GridGenerator gridGenerator;
    public List<WaveData> wavesData;
    public List<SpecialSpawn> specialSpawns;
    private int currentWaveIndex = 0;
    public WaveState CurrentWaveState { get; private set; } = WaveState.NotStarted;
    public float[] customWaveIntervals; // Manually set time between each wave
    public WaveState[] customWaveStates; // Manually set states for each wave
    private Vector3 spawnPosition;

    public int enemyCount { get; set; } = 0;
    public float extendedBreakDuration = 30.0f;

    // UI Information
    public int TotalWaves => wavesData.Count;
    public int CurrentWave => currentWaveIndex;

    private void Start()
    {
        gridGenerator = GridGenerator.Instance;
        getstartPosition();
    }

    public void getstartPosition()
    {
        if (gridGenerator.CurrentState == GridState.Generated)
        {
            spawnPosition = gridGenerator.GetStartNode().transform.position;
        }
    }

    public void StartWave()
    {
        if (CurrentWaveState == WaveState.BetweenWaves || CurrentWaveState == WaveState.NotStarted)
        {
            if (currentWaveIndex < wavesData.Count)
            {
                StartCoroutine(SpawnWaveRoutine(wavesData[currentWaveIndex]));
                currentWaveIndex++;
            }
            else
            {
                Debug.Log("All waves are done!");
                CurrentWaveState = WaveState.Completed;
            }
        }
    }

    IEnumerator SpawnWaveRoutine(WaveData waveData)
    {
        CurrentWaveState = WaveState.Spawning;

        foreach (EnemyWave enemyWave in waveData.enemyWaves)
        {
            for (int i = 0; i < enemyWave.count; i++)
            {
                SpawnEnemy(enemyWave.enemyPrefab);
                yield return new WaitForSeconds(enemyWave.delayBetweenSpawns);
            }
        }

        // Check if there's a special spawn for this wave
        foreach (SpecialSpawn specialSpawn in specialSpawns)
        {
            if (specialSpawn.waveNumber == currentWaveIndex)
            {
                yield return new WaitForSeconds(specialSpawn.delayAfterWaveStarts);
                SpawnEnemy(specialSpawn.specialEnemyPrefab);
            }
        }

        // Check your custom wave states
        if (currentWaveIndex < customWaveStates.Length)
        {
            CurrentWaveState = customWaveStates[currentWaveIndex];
        }
        else
        {
            CurrentWaveState = WaveState.BetweenWaves; // Default
        }

        if (IsBigWave(currentWaveIndex))
        {
            Debug.Log("Big wave completed! Extended break.");
            yield return new WaitForSeconds(extendedBreakDuration);
        }
        else if (currentWaveIndex < customWaveIntervals.Length)
        {
            // Custom interval time from the array
            yield return new WaitForSeconds(customWaveIntervals[currentWaveIndex]);
        }
        else
        {
            // Default interval time
            yield return new WaitForSeconds(waveData.delayBetweenWaves);
        }

        CurrentWaveState = WaveState.BetweenWaves;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemyCount++;
    }

    public void EnemyDied()
    {
        enemyCount--;

        if (enemyCount <= 0 && CurrentWaveState == WaveState.WaveActive)
        {
            CurrentWaveState = WaveState.BetweenWaves;

            if (currentWaveIndex >= wavesData.Count)
            {
                CurrentWaveState = WaveState.Completed;
                Debug.Log("All waves completed!");
            }
        }
    }

    bool IsBigWave(int waveIndex)
    {
        return (waveIndex % 3 == 0);
    }
    public void BreakNearbyWall(GameObject enemyGameObject)
    {
        NodeScript nearbyNode = GridGenerator.Instance.GetNodeScriptAtPosition(enemyGameObject.transform.position);
        List<NodeScript> neighbors = GridGenerator.Instance.GetNeighbours(nearbyNode);

        foreach (NodeScript neighbor in neighbors)
        {
            if (neighbor.IsOccupied && !neighbor.node.Walkable)
            {
                TowerManager.Instance.RemoveWallAt(neighbor.node.WorldPosition);
                // Also regenerate the path
                PathManager.Instance.CalculatePrimaryPath();
                break;
            }
        }
    }
}






