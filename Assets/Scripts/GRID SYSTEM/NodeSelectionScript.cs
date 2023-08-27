using UnityEngine;

public class NodeSelectionScript : MonoBehaviour
{
    public TowerManager towerManager;
    public UIManager uiManager;

    private void Awake()
    {
        towerManager = TowerManager.Instance;
        uiManager = UIManager.Instance;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        // For desktop and Unity Editor
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(Input.mousePosition);
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        // For mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                HandleInput(touch.position);
            }
        }
#endif
    }

    void HandleInput(Vector2 position)
    {
        Debug.Log("Input detected.");

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(position);

        int layerMask = LayerMask.GetMask("Selectable", "Tower");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log("Raycast hit an object in the 'Selectable' or 'Tower' layer.");

            NodeScript nodeScript = hit.collider.GetComponent<NodeScript>();

            if (uiManager.isRemoveModeActive)
            {
                HandleRemoveMode(hit, nodeScript);
                return;
            }

            if (nodeScript != null && uiManager.currentState == UIState.PlacingTower)
            {
                nodeScript.HandleSelection(towerManager);
            }
            else
            {
                Debug.Log("NodeScript component not found on the hit object on: " + hit);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any object in the 'Selectable' or 'Tower' layer.");
        }
    }

    void HandleRemoveMode(RaycastHit hit, NodeScript nodeScript)
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Tower"))
        {
            towerManager.RemoveTowerAt(hit.collider.transform.position);
            uiManager.currentState = UIState.NotPlacingTower;

            NodeScript correspondingNodeScript = hit.collider.GetComponentInParent<NodeScript>();
            if (correspondingNodeScript)
            {
                correspondingNodeScript.IsOccupied = false;
                correspondingNodeScript.node.Walkable = true;
                correspondingNodeScript.ShowOutline();
            }
        }
    }
}