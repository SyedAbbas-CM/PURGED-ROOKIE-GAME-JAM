using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionButton : MonoBehaviour
{
    public int itemIndex; // This will store either a Tower or a Wall prefab
    private UIManager uiManager;
    private GameObject itemPrefab;

    public void Initialize(GameObject item, UIManager manager, int index)
    {
        itemPrefab = item;
        uiManager = manager; // No need for the static reference since we're passing the manager directly now.
        itemIndex = index;

        // Assume both Tower and Wall have a common interface or base class called 'PlaceableItem' which has a 'itemName' property
        GetComponentInChildren<TextMeshProUGUI>().text = itemPrefab.GetComponent<PlaceableItem>().itemName;
    }

    public void OnButtonPressed()
    {
        if (uiManager.towerManager.currentPlacementType == TowerManager.PlacementType.Tower)
        {
            uiManager.towerManager.SelectTower(itemIndex);
        }
        else if (uiManager.towerManager.currentPlacementType == TowerManager.PlacementType.Wall)
        {
            uiManager.towerManager.SelectWall(itemIndex);
        }
    }
}