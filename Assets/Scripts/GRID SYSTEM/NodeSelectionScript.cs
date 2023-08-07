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
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // We only check for layers in the "Selectable" layer
            int layerMask = LayerMask.GetMask("Selectable");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                NodeScript nodeScript = hit.collider.GetComponent<NodeScript>();
                if (nodeScript != null)
                {
                    nodeScript.HandleSelection(towerManager);
                }
            }
        }
    }
}