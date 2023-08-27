using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData : ScriptableObject
{
    public string levelName;
    public string sceneName;
    public int gridSizeX;
    public int gridSizeY;
    public List<TowerPlacementData> initialTowers; // This has been renamed for clarity
    public int maxEnemiesPerWave;
    public List<TowerInfo> towersAvailable;
    public List<WallInfo> wallsAvailable;
    public List<WaveData> wavesData;
}

[System.Serializable]
public class TowerPlacementData
{
    public Vector2Int gridPosition;
    public int towerTypeIndex;
}

[System.Serializable]
public class TowerInfo
{
    public string name;
    public GameObject prefab; // Drag and drop tower prefab here
}

[System.Serializable]
public class WallInfo
{
    public string name;
    public GameObject prefab; // Drag and drop wall prefab here
}

