using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    // Reference to the Tower prefabs
    public List<GameObject> towerPrefabs;

    public void PlaceTower(Vector3 position, GameObject parent, int towerIndex = 0)
    {
        // Check if the towerIndex is within the bounds of the list
        if (towerIndex < 0 || towerIndex >= towerPrefabs.Count)
        {
            Debug.LogError("Invalid tower index");
            return;
        }

        // Instantiate the tower at the position of the node and make it a child of the node
        GameObject newTower = Instantiate(towerPrefabs[towerIndex], position, Quaternion.identity);
        newTower.transform.parent = parent.transform;
    }
}