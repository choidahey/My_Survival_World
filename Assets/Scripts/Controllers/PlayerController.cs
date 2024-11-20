using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 15f;
    private float jumpForce = 5f;
    private bool isGrounded = true;
    private int groundLayer;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    [Header("Camera Settings")]
    [SerializeField] private CameraController target_camera;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (LayerMask.NameToLayer("Ground") != -1)
            groundLayer = LayerMask.NameToLayer("Ground");
    }

    private void Update() 
    {
        Move();
        RotatePlayer();
        Jump();
    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirZ;
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * moveSpeed;

        rb.MovePosition(transform.position + velocity * Time.deltaTime);

    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(jumpForce * -Physics.gravity.y);
            rb.AddForce(jumpVelocity, ForceMode.Impulse);

            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == groundLayer)
            isGrounded = true;
    }

    private void RotatePlayer()
    {
        if (Input.GetMouseButton(1))
        {
            if (target_camera == null)
            {
                Debug.LogError("카메라 없어요");
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * target_camera.Sensitivity;

            float targetRotationY = transform.eulerAngles.y + mouseX;
            float currentRotationY = transform.eulerAngles.y;
            float smoothRotationY = Mathf.LerpAngle(currentRotationY, targetRotationY, Time.deltaTime / 0.1f);

            Quaternion targetRotation = Quaternion.Euler(0f, smoothRotationY, 0f);
            rb.MoveRotation(targetRotation);
        }
    }
}
