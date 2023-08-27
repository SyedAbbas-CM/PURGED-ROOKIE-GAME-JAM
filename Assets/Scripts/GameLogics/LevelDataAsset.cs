using UnityEngine;
using UnityEditor;

public class LevelDataAsset
{
    [MenuItem("Assets/Create/Level Data")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<LevelData>();
    }
}