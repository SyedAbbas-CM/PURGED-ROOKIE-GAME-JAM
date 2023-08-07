using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    // Reference to the Tower prefabs
    public List<GameObject> towerPrefabs;

    public void PlaceTower(Vector3 position, GameObject parent, int towerIndex = 0)
    {
        if (towerIndex < 0 || towerIndex >= towerPrefabs.Count)
        {
            Debug.LogError($"Invalid tower index {towerIndex}. Must be between 0 and {towerPrefabs.Count - 1}.");
            return;
        }

        if (towerPrefabs[towerIndex] == null)
        {
            Debug.LogError($"Tower prefab at index {towerIndex} is null.");
            return;
        }

        Debug.Log($"Placing tower of type {towerPrefabs[towerIndex].name} at position {position}.");
        GameObject newTower = Instantiate(towerPrefabs[towerIndex], position, Quaternion.identity);

        if (newTower == null)
        {
            Debug.LogError("Failed to instantiate tower.");
            return;
        }

        Debug.Log("Tower instantiated successfully.");
        newTower.transform.parent = parent.transform;
    }
}