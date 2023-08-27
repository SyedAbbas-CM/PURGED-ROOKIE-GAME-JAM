using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class TowerManager : Singleton<TowerManager>
{
    ResourceManager resourceManager;
    UIManager uiManager;
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
    private List<GameObject> towers = new List<GameObject>();
    private List<GameObject> walls = new List<GameObject>();

    private void Awake()
    {
        uiManager = UIManager.Instance;
        resourceManager = ResourceManager.Instance;
    }
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

        if (!resourceManager.SpendMetal(10)) // 10 is an example value, replace with your tower's cost
        {
            Debug.LogError("Not enough metal to place tower.");
            return;
        }



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
        towers.Add(newTower);
        Debug.Log("Tower instantiated successfully.");
        newTower.transform.parent = parent.transform;
        uiManager.currentState = UIState.NotPlacingTower;
    }

    public void PlaceWall(Vector3 position, GameObject parent)
    {

        if (!resourceManager.SpendWood(5)) // 5 is an example value, replace with your wall's cost
        {
            Debug.LogError("Not enough wood to place wall.");
            return;
        }



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
        uiManager.currentState = UIState.PlacingTower;
        if (newWall == null)
        {
            Debug.LogError("Failed to instantiate wall.");
            return;
        }
        walls.Add(newWall);
        Debug.Log("Wall instantiated successfully.");
        newWall.transform.parent = parent.transform;
        uiManager.currentState = UIState.NotPlacingTower;
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
    public void RemoveTowerAt(Vector3 position)
    {
        // Find a tower at the specified position
        GameObject towerToRemove = towers.Find(t => Vector3.Distance(t.transform.position, position) < 0.1f);
        if (towerToRemove)
        {
            towers.Remove(towerToRemove);
            Destroy(towerToRemove);
        }
        else
        {
            Debug.Log("NOTHING FOUND AAAAA");
        }
    }
    public void RemoveWallAt(Vector3 position)
    {
        GameObject wallToRemove = walls.Find(w => Vector3.Distance(w.transform.position, position) < 0.1f);
        if (wallToRemove)
        {
            walls.Remove(wallToRemove);
            Destroy(wallToRemove);
            // You can also set the node at this position to be walkable again if required.
        }
        else
        {
            Debug.LogError("No wall found at the specified position.");
        }
    }
}