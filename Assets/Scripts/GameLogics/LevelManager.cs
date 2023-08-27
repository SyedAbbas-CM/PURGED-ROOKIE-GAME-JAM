using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public LevelData currentLevelData; // Drag your LevelData asset here in the inspector
    private GridGenerator gridGenerator;


    void Start()
    {
        gridGenerator = GetComponent<GridGenerator>();
        if (currentLevelData == null)
        {
            Debug.LogError("No LevelData assigned!");
            return;
        }

        SetupLevel();
        SetupTowersAndWalls();
        InitializeEnemyManager();
    }
    private void InitializeEnemyManager()
    {
        EnemyManager.Instance.wavesData = currentLevelData.wavesData;
    }
    private void SetupLevel()
    {

        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator component not found on this GameObject.");
            return;
        }

        gridGenerator.GenerateGrid(currentLevelData.gridSizeX, currentLevelData.gridSizeY);

        foreach (TowerPlacementData towerData in currentLevelData.initialTowers)
        {
            PlaceItemOnNode(towerData.gridPosition.x, towerData.gridPosition.y, towerData.towerTypeIndex);
        }
    }

    public void LoadNextLevel(LevelData nextLevel)
    {
        currentLevelData = nextLevel;
        SceneManager.LoadScene(nextLevel.sceneName);
    }
    private void PlaceItemOnNode(int x, int y, int towerTypeIndex)
    {
        NodeScript node = gridGenerator.GetNodeAtPosition(x, y);
        if (node != null && !node.IsOccupied)
        {
            TowerManager.Instance.SelectTower(towerTypeIndex);
            node.HandleSelection(TowerManager.Instance);
        }
        else
        {
            Debug.LogError($"Node not found or is occupied at position ({x}, {y}).");
        }
    }

    private void SetupTowersAndWalls()
    {
        TowerManager.Instance.towerPrefabs.Clear();
        foreach (TowerInfo towerInfo in currentLevelData.towersAvailable)
        {
            TowerManager.Instance.towerPrefabs.Add(towerInfo.prefab);
        }

        TowerManager.Instance.wallPrefabs.Clear();
        foreach (WallInfo wallInfo in currentLevelData.wallsAvailable)
        {
            TowerManager.Instance.wallPrefabs.Add(wallInfo.prefab);
        }
    }
}