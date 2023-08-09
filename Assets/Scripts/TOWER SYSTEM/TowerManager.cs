using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public int selectedPrefabIndex = 0;
    // Enums to determine what the user wants to place
    public enum PlacementType
    {
        Tower,
        Wall
    }
    public PlacementType currentPlacementType = PlacementType.Tower;

    // Reference to the Tower and Wall prefabs
    public List<GameObject> towerPrefabs;
    public List<GameObject> wallPrefabs;

    // Toggle the placement type between Tower and Wall
    public void TogglePlacementType()
    {
        currentPlacementType = currentPlacementType == PlacementType.Tower ? PlacementType.Wall : PlacementType.Tower;
    }

    // Place either Tower or Wall based on the current placement type
    public void Place(Vector3 position, GameObject parent)
    {
        switch (currentPlacementType)
        {
            case PlacementType.Tower:
                PlaceTower(position, parent);
                break;
            case PlacementType.Wall:
                PlaceWall(position, parent);
                break;
            default:
                Debug.LogError("Invalid placement type.");
                break;
        }
    }

    public void PlaceTower(Vector3 position, GameObject parent)
    {
        if (selectedPrefabIndex < 0 || selectedPrefabIndex >= towerPrefabs.Count)
        {
            Debug.LogError($"Invalid tower index {selectedPrefabIndex}. Must be between 0 and {towerPrefabs.Count - 1}.");
            return;
        }

        if (towerPrefabs[selectedPrefabIndex] == null)
        {
            Debug.LogError($"Tower prefab at index {selectedPrefabIndex} is null.");
            return;
        }

        Debug.Log($"Placing tower of type {towerPrefabs[selectedPrefabIndex].name} at position {position}.");
        GameObject newTower = Instantiate(towerPrefabs[selectedPrefabIndex], position, Quaternion.identity);

        if (newTower == null)
        {
            Debug.LogError("Failed to instantiate tower.");
            return;
        }

        Debug.Log("Tower instantiated successfully.");
        newTower.transform.parent = parent.transform;
    }

    public void PlaceWall(Vector3 position, GameObject parent)
    {
        if (selectedPrefabIndex < 0 || selectedPrefabIndex >= wallPrefabs.Count)
        {
            Debug.LogError($"Invalid wall index {selectedPrefabIndex}. Must be between 0 and {wallPrefabs.Count - 1}.");
            return;
        }

        if (wallPrefabs[selectedPrefabIndex] == null)
        {
            Debug.LogError($"Wall prefab at index {selectedPrefabIndex} is null.");
            return;
        }

        Debug.Log($"Placing wall of type {wallPrefabs[selectedPrefabIndex].name} at position {position}.");
        GameObject newWall = Instantiate(wallPrefabs[selectedPrefabIndex], position, Quaternion.identity);

        if (newWall == null)
        {
            Debug.LogError("Failed to instantiate wall.");
            return;
        }

        Debug.Log("Wall instantiated successfully.");
        newWall.transform.parent = parent.transform;
    }
    public void SelectTower(int towerIndex)
    {
        currentPlacementType = PlacementType.Tower;
        selectedPrefabIndex = towerIndex;
    }

    public void SelectWall(int wallIndex)
    {
        currentPlacementType = PlacementType.Wall;
        selectedPrefabIndex = wallIndex;
    }
}