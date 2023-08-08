using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 20f;
    public Vector2 panLimit;

    public float rotationSpeed = 20f;
    public KeyCode rotateRightKey = KeyCode.E;
    public KeyCode rotateLeftKey = KeyCode.Q;

    public float scrollSpeed = 20f;
    public float minY = 20f;
    public float maxY = 120f;

    public float zoomLevel;
    public float sensitivity = 1;
    public float speed = 30;
    public float maxZoom = 30;
    float zoomPosition;


    private GridGenerator gridGenerator;

    private void Start()
    {
        gridGenerator = GridGenerator.Instance;
    }

    void Update()
    {
        if (gridGenerator.CurrentState == GridState.Generated)
        {
            // Panning Logic
            if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
            {
                transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
            {
                transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
            }

            float scrollValue = Input.GetAxis("Mouse ScrollWheel");

            zoomLevel += Input.mouseScrollDelta.y * sensitivity;
            zoomLevel = Mathf.Clamp(zoomLevel, 0, maxZoom);
            zoomPosition = Mathf.MoveTowards(zoomPosition, zoomLevel, speed * Time.deltaTime);
            transform.position = transform.position + (transform.forward * zoomPosition);
        
        }
    }
    public void InitializeCamera()
    {
        // Adjust the camera's initial position to center on the grid
        Vector3 gridCenter = gridGenerator.GetGridCenter();
        panLimit = new Vector2(gridGenerator.GetGridWidth() / 2, gridGenerator.GetGridDepth() / 2);
        transform.position = new Vector3(gridCenter.x, maxY, gridCenter.z);
        // Look downwards
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);

        // Dynamically set pan limits based on the grid size
        panLimit = new Vector2(gridGenerator.GetGridWidth() / 2, gridGenerator.GetGridDepth() / 2);
    }
}