using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public Vector2 panLimit;

    public float scrollSpeed = 20f;
    public float minY = 20f;
    public float maxY = 120f;

    public float zoomLevel;
    public float sensitivity = 1;
    public float speed = 30;
    public float maxZoom = 30;
    public float minZoom = 0;
    float zoomPosition;

    private GridGenerator gridGenerator;

    private Vector2 touchStart;

    private void Start()
    {
        gridGenerator = GridGenerator.Instance;
    }

    void Update()
    {
        if(gridGenerator == null)
        {
            return;
        }
        if (gridGenerator.CurrentState == GridState.Generated)
        {
            // PC controls
#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetKey("w"))
            {
                transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("s"))
            {
                transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("a"))
            {
                transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("d"))
            {
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
            }
            HandleScrollWheelZoom();
#endif

            // Mobile controls
#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStart = touch.position;
                        break;
                    case TouchPhase.Moved:
                        Vector2 direction = touchStart - touch.position;
                        transform.Translate(direction.x * panSpeed * Time.deltaTime, 0, direction.y * panSpeed * Time.deltaTime, Space.World);
                        touchStart = touch.position;
                        break;
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                UpdateZoom(-deltaMagnitudeDiff);
            }
#endif
        }
    }

    public void InitializeCamera()
    {
        Vector3 gridCenter = gridGenerator.GetGridCenter();
        panLimit = new Vector2(gridGenerator.GetGridWidth() / 2, gridGenerator.GetGridDepth() / 2);
        transform.position = new Vector3(gridCenter.x, maxY, gridCenter.z);
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);
        panLimit = new Vector2(gridGenerator.GetGridWidth() / 2, gridGenerator.GetGridDepth() / 2);
    }
    void UpdateZoom(float deltaMagnitudeDiff)
    {
        float desiredZoom = transform.position.y + deltaMagnitudeDiff * sensitivity;
        Debug.Log($"Delta Magnitude: {deltaMagnitudeDiff}, Desired Zoom: {desiredZoom}");
        desiredZoom = Mathf.Clamp(desiredZoom, minY, maxY);
        transform.position = new Vector3(transform.position.x, desiredZoom, transform.position.z);
    }
    void HandleScrollWheelZoom()
    {
        float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
        if (ScrollWheelChange != 0)
        {
            float R = ScrollWheelChange * 15;
            float PosX = Camera.main.transform.eulerAngles.x + 90;
            float PosY = -1 * (Camera.main.transform.eulerAngles.y - 90);
            PosX = PosX / 180 * Mathf.PI;
            PosY = PosY / 180 * Mathf.PI;
            float X = R * Mathf.Sin(PosX) * Mathf.Cos(PosY);
            float Z = R * Mathf.Sin(PosX) * Mathf.Sin(PosY);
            float Y = R * Mathf.Cos(PosX);
            float CamX = Camera.main.transform.position.x;
            float CamY = Camera.main.transform.position.y;
            float CamZ = Camera.main.transform.position.z;
            Camera.main.transform.position = new Vector3(CamX + X, CamY + Y, CamZ + Z);
        }
    }
}