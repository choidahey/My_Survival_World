using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    private float sensitivity = 5f;
    private float cameraRotationLimitX = 40f;
    private float rotationSmoothTime = 0.1f;

    [Header("Player Settings")]
    [SerializeField] private Rigidbody playerRb;

    private float currentCameraRotationX = 0f;

    private Camera targetCamera;

    public float Sensitivity
    {
        get { return sensitivity; }
    }

    private void Start()
    {
        targetCamera = GetComponent<Camera>();
        if (targetCamera == null)
        {
            Debug.LogError("CameraController가 Camera 컴포넌트를 찾을 수 없습니다.");
        }
    }

    private void Update()
    {
        HandleCameraRotation();
    }

    private void HandleCameraRotation()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            currentCameraRotationX -= mouseY;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimitX, cameraRotationLimitX);

            float currentRotationX = transform.localEulerAngles.x;
            if (currentRotationX > 180f)
                currentRotationX -= 360f;

            float smoothRotationX = Mathf.LerpAngle(currentRotationX, currentCameraRotationX, Time.deltaTime / rotationSmoothTime);

            transform.localEulerAngles = new Vector3(smoothRotationX, 0f, 0f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
