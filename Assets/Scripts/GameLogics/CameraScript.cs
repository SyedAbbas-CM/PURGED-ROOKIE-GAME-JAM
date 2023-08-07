using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Set this to the grid's center transform in the inspector.

    private void Start()
    {
        if (target)
        {
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z - 10);
            transform.LookAt(target);
        }
    }
}

