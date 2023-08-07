using UnityEngine;

public class IsometricCameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;

    public float scrollSpeed = 20f;
    public float minY = 20f;
    public float maxY = 120f;

    private GridGenerator gridGenerator;

    private void Start()
    {
        gridGenerator = GridGenerator.Instance;

        if (gridGenerator.CurrentState == GridState.Generated)
        {
            InitializeCamera();
        }
    }
    public void InitializeCamera()
    {
        // Adjust the camera's initial position to center on the grid
        Vector3 gridCenter = gridGenerator.GetGridCenter();
        transform.position = new Vector3(gridCenter.x, transform.position.y, gridCenter.z - gridGenerator.GetGridDepth() / 2);

        // Dynamically set pan limits based on the grid size
        panLimit = new Vector2(gridGenerator.GetGridWidth() / 2, gridGenerator.GetGridDepth() / 2);
    }

    void Update()
    {
        if (gridGenerator.CurrentState == GridState.Generated)
        {
            Vector3 pos = transform.position;

            if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                pos.z += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
            {
                pos.z -= panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                pos.x += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
            {
                pos.x -= panSpeed * Time.deltaTime;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

            pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

            transform.position = pos;
        }
    }
}