using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public List<GameObject> managersToLoad; // Managers to instantiate
    public GameObject UIPrefab; // UI to instantiate in each scene

    private GameObject currentUIInstance; // To track the instantiated UI

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the event

        // Instantiate the managers
        foreach (GameObject managerPrefab in managersToLoad)
        {
            Instantiate(managerPrefab);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupUI();
    }

    private void SetupUI()
    {
        if (UIPrefab)
        {
            // If there's a UI from previous scene, destroy it
            if (currentUIInstance)
            {
                Destroy(currentUIInstance);
            }
            currentUIInstance = Instantiate(UIPrefab);
        }
        else
        {
            Debug.LogError("UIPrefab not set in GameManager.");
        }
    }

    public void LoadNewLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe when destroyed
    }
}