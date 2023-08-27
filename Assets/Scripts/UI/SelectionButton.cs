using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionButton : MonoBehaviour
{
    public int itemIndex; // This will store either a Tower or a Wall prefab index
    private UIManager uiManager;
    private TowerManager.PlacementType placementType; // This will determine if it's a tower or wall

    public void Initialize(GameObject prefab, UIManager uiManager)
    {
        this.uiManager = uiManager;

        // Determine if it's a tower or wall
        if (uiManager.towerManager.towerPrefabs.Contains(prefab))
        {
            itemIndex = uiManager.towerManager.towerPrefabs.IndexOf(prefab);
            placementType = TowerManager.PlacementType.Tower;
        }
        else if (uiManager.towerManager.wallPrefabs.Contains(prefab))
        {
            itemIndex = uiManager.towerManager.wallPrefabs.IndexOf(prefab);
            placementType = TowerManager.PlacementType.Wall;
        }
        else
        {
            Debug.LogError("The provided prefab is neither a tower nor a wall.");
            return;
        }

        // Add the click listener to the button
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (placementType == TowerManager.PlacementType.Tower)
        {

            uiManager.towerManager.SelectTower(itemIndex);
            uiManager.currentState = UIState.PlacingTower;


        }
        else if (placementType == TowerManager.PlacementType.Wall)
        {
            uiManager.towerManager.SelectWall(itemIndex);
            uiManager.currentState = UIState.PlacingTower;
        }
    }

}