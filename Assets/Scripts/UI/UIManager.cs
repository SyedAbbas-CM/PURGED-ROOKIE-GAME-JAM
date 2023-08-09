using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject selectionButtonPrefab; // A generic button prefab for both towers and walls
    public Transform selectionPanel;         // Parent where selection buttons will be instantiated
    public TowerManager towerManager;
    public EnemyManager enemyManager;
    public Button readyButton;
    public Button toggleSelectionButton;     // Button to toggle between tower and wall selection
    public TextMeshProUGUI toggleSelectionText;         // Text to indicate current selection mode (tower or wall)

    private List<GameObject> currentButtons = new List<GameObject>(); // List to keep track of current buttons in the panel

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        PopulateSelectionPanel();
        UpdateToggleSelectionText();
    }

    private void PopulateSelectionPanel()
    {
        // First, clear the existing buttons
        foreach (var btn in currentButtons)
        {
            Destroy(btn);
        }
        currentButtons.Clear();

        // Decide which list of prefabs to use based on current placement type in TowerManager
        List<GameObject> prefabsToUse = towerManager.currentPlacementType == TowerManager.PlacementType.Tower ? towerManager.towerPrefabs : towerManager.wallPrefabs;

        int index = 0;
        foreach (var item in prefabsToUse)
        {
            GameObject btn = Instantiate(selectionButtonPrefab, selectionPanel);
            btn.GetComponent<SelectionButton>().Initialize(item, this, index);
            btn.GetComponent<Image>().sprite = item.GetComponent<PlaceableItem>().iconSprite; // Set the sprite
            currentButtons.Add(btn);
            index++;
        }
    }

    public void OnToggleSelectionPressed()
    {
        towerManager.TogglePlacementType();
        PopulateSelectionPanel();
        UpdateToggleSelectionText();
    }

    private void UpdateToggleSelectionText()
    {
        if (towerManager == null)
            Debug.LogError("towerManager is null.");
        else if (towerManager.currentPlacementType == null)
            Debug.LogError("currentPlacementType is null.");
        else if (toggleSelectionText == null)
            Debug.LogError("toggleSelectionText is null.");
        toggleSelectionText.text = towerManager.currentPlacementType == TowerManager.PlacementType.Tower ? "Towers" : "Walls";
    }

    public void OnReadyButtonPressed()
    {
        enemyManager.StartWave();
    }
}