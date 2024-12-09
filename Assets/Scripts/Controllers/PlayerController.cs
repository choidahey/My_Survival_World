using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runScale = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    private bool isGround = true;
    private bool isFalling = false;

    private float currentH = 0f;
    private float currentV = 0f;
    private Vector3 currentDirection = Vector3.zero;

    private Animator animator;
    private Rigidbody rigidBody;
    private LayerMask groundMask;

    private float runStaminaCost = 30f;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        groundMask = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        CheckGrounded();

        Move();
        Jump();
        UpdateFallingState();

        animator.SetBool("isGround", isGround);
        animator.SetBool("isFalling", isFalling);
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, groundCheckDistance, groundMask))
        {
            if (!isGround)
            {
                isFalling = false;
            }
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    private void Move()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Transform camera = Camera.main.transform;
        bool isRunning = false;

        if (StaminaManager.instance.isExhausted)
        {
            v = 0f;
            h = 0f;
            animator.SetFloat("MoveSpeed", 0f);
            animator.SetBool("isExhausted", true);
            return;
        }
        else
        {
            animator.SetBool("isExhausted", false);
        }

        // 달리기 조건
        if (Input.GetKey(KeyCode.LeftShift) && StaminaManager.instance.HasEnoughStamina(runStaminaCost * Time.deltaTime))
        {
            isRunning = true;
            StaminaManager.instance.ConsumeStamina(runStaminaCost * Time.deltaTime);
            v *= runScale;
            h *= runScale;
        }

        float interpolation = 10f;
        currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);
        currentH = Mathf.Lerp(currentH, h, Time.deltaTime * interpolation);

        Vector3 direction = camera.forward * currentV + camera.right * currentH;
        float directionLength = direction.magnitude;
        direction.y = 0f;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            currentDirection = Vector3.Slerp(currentDirection, direction, Time.deltaTime * interpolation);
            transform.rotation = Quaternion.LookRotation(currentDirection);
            transform.position += currentDirection * walkSpeed * Time.deltaTime;
            animator.SetFloat("MoveSpeed", direction.magnitude);
        }
        else
        {
            animator.SetFloat("MoveSpeed", 0f);
        }

        if (!isRunning)
        {
            StaminaManager.instance.EnableStaminaRegen();
        }
        else
        {
            StaminaManager.instance.DisableStaminaRegen();
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGround)
        {
            isFalling = false;

            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(jumpForce * -Physics.gravity.y);
            rigidBody.AddForce(jumpVelocity, ForceMode.Impulse);
        }
    }

    private void UpdateFallingState()
    {
        if (!isGround && rigidBody.velocity.y < 0)
        {
            isFalling = true;
        }
        else if (isGround)
        {
            isFalling = false;
        }
    }
}
