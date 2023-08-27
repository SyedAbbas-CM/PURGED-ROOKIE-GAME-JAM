using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public enum UIState { PlacingTower, NotPlacingTower, RemovingTower }


public class UIManager : Singleton<UIManager>
{
    public UIState currentState = UIState.PlacingTower;
    public GameObject selectionButtonPrefab; // A generic button prefab for both towers and walls
    public GameObject ButtonsPrefabs;
    public Transform selectionPanel;
    public Transform removeTowerButton;// Parent where selection buttons will be instantiated
    public TowerManager towerManager;
    public EnemyManager enemyManager;
    public Button readyButton;
    public Button toggleSelectionButton;     // Button to toggle between tower and wall selection
    public Button toggleDisplayButton;  // Button to toggle the display of the selection panel
    public PathManager pathManager;
    public bool isRemoveModeActive = false;
    public GameObject pauseUI;
    private bool isButtonsPrefabs = true;
    private List<GameObject> currentButtons = new List<GameObject>(); // List to keep track of current buttons in the panel


    public void ToggleRemoveMode()
    {
        if (currentState == UIState.RemovingTower)
        {
            currentState = UIState.NotPlacingTower; // Switch back to normal state
            isRemoveModeActive = false;
        }
        else
        {
            currentState = UIState.RemovingTower; // Enter tower removal state
            isRemoveModeActive = true;
        }
    }


    public void ToggleDisplay()
    {
        isButtonsPrefabs = !isButtonsPrefabs;
        ButtonsPrefabs.gameObject.SetActive(isButtonsPrefabs);



    }

    public void TogglePauseMenu()
    {
        pauseUI.SetActive(!pauseUI.activeSelf);
        if (pauseUI.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

    }


    private void Start()
    {
        PopulateSelectionPanel();
    }
    private void Update()
    {
        HandleWaveUIState();
        switch (currentState)
        {
            case UIState.PlacingTower:
                ShowGrid();
                isRemoveModeActive = false;
                break;
            case UIState.NotPlacingTower:
                HideGrid();
                break;

            default:
                HideGrid();
                break;
        }
    }

    private void HandleWaveUIState()
    {
        if (enemyManager.CurrentWaveState == WaveState.NotStarted || enemyManager.CurrentWaveState == WaveState.ExtendedBreak)
        {
            ShowUI();
        }
        else
        {
            HideUI();
        }
    }
    private void ShowUI()
    {
        // Show any UI elements you want to show, e.g.:
        selectionPanel.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(true);
        removeTowerButton.gameObject.SetActive(true);
        // ... and so on for other UI elements
    }

    private void HideUI()
    {
        // Hide any UI elements you want to hide, e.g.:
        selectionPanel.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);
        removeTowerButton.gameObject.SetActive(false);
        // ... and so on for other UI elements
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

        foreach (var prefab in prefabsToUse)
        {
            Debug.Log(prefab);
            GameObject btn = Instantiate(selectionButtonPrefab, selectionPanel);
            btn.GetComponent<SelectionButton>().Initialize(prefab, this);

         
            Debug.Log($"Sprite for {prefab.GetComponent<PlaceableItem>().name}: {prefab.GetComponent<PlaceableItem>().iconSprite}");
            Debug.Log(prefab.GetComponent<PlaceableItem>().iconSprite);
            btn.GetComponent<Image>().sprite = prefab.GetComponent<PlaceableItem>().iconSprite;
            currentButtons.Add(btn);
        }
    }

    public void OnToggleSelectionPressed()
    {
        towerManager.TogglePlacementType();
        PopulateSelectionPanel();

    }
    public void onPauseButtonPressed()
    {
        TogglePauseMenu();
    }
    public void onContinueButtonPressed()
    {
        TogglePauseMenu();
    }
    public void onRestartButtonPressed()
    {
        TogglePauseMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void onMenuButtonPressed()
    {


    }

    public void OnReadyButtonPressed()
    {
        pathManager.CalculatePrimaryPath();
        enemyManager.StartWave();
    }
    private void ShowGrid()
    {
        foreach (NodeScript nodeScript in FindObjectsOfType<NodeScript>())
        {
            nodeScript.ShowOutline();
        }
    }

    private void HideGrid()
    {
        foreach (NodeScript nodeScript in FindObjectsOfType<NodeScript>())
        {
            nodeScript.HideOutline();
        }
    }
    public void StartPlacingTowerMode()
    {
        currentState = UIState.PlacingTower;
    }

    public void StopPlacingTowerMode()
    {
        currentState = UIState.NotPlacingTower;
    }
    public void ActivateRemoveTowerMode()
{
        isRemoveModeActive = true;

}

public void DeactivateRemoveTowerMode()
{
        isRemoveModeActive = false;

}
    
}