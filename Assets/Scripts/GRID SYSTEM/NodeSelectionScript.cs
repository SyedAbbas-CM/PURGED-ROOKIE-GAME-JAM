using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NodeSelectionScript : MonoBehaviour
{
    // Reference to the TowerManager
    public TowerManager towerManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button pressed.");

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask("Selectable");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("Raycast hit an object in the 'Selectable' layer.");

                NodeScript nodeScript = hit.collider.GetComponent<NodeScript>();
                if (nodeScript != null)
                {
                    //Debug.Log("Selected node: " + nodeScript.ToString());
                    nodeScript.HandleSelection(towerManager);
                }
                else
                {
                    Debug.LogError("NodeScript component not found on the hit object.");
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any object in the 'Selectable' layer.");
            }
        }
    }
}