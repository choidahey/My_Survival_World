using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private float walkSpeed = 5f;
    private float runScale = 5f;
    private float jumpForce = 7f;

    private bool isGrounded = true;
    private bool isFalling = false;

    private float currentH = 0f;
    private float currentV = 0f;

    private Vector3 currentDirection = Vector3.zero;
    private LayerMask groundLayer;

    private Animator animator;
    private Rigidbody rigidBody;
    private List<Collider> collisions = new List<Collider>();

    private float runStaminaCost = 30f;


    private void Awake()
    {
        if (GetComponent<Rigidbody>() != null)
            rigidBody = GetComponent<Rigidbody>();

        if (GetComponent<Animator>() != null)
            animator = GetComponent<Animator>();

        groundLayer = LayerMask.NameToLayer("Ground");
    }

    private void Update()
    {
        Move();
        Jump();

    }

    private void Move()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Transform camera = Camera.main.transform;

        bool isRunning = false;

        if (StaminaManager.instance.isExhausted)
        {
            v *= 0f;
            h *= 0f;

            animator.SetFloat("MoveSpeed", 0f);
            // TODO :: 탈진 모션 추가하기

            return;
        }

        if (Input.GetKey(KeyCode.LeftShift) && StaminaManager.instance.HasEnoughStamina(runStaminaCost * Time.deltaTime))
        {
            isRunning = true;
            StaminaManager.instance.ConsumeStamina(runStaminaCost * Time.deltaTime);
            v *= runScale;
            h *= runScale;
        }
        else
        {
            v *= 1f;
            h *= 1f;
            isRunning = false;
        }


        float interpolation = 10f;

        currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);
        currentH = Mathf.Lerp(currentH, h, Time.deltaTime * interpolation);

        Vector3 direction = camera.forward * currentV + camera.right * currentH;

        float directionLength = direction.magnitude;

        direction.y = 0;
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
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false;
            isFalling = true;

            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(jumpForce * -Physics.gravity.y);
            rigidBody.AddForce(jumpVelocity, ForceMode.Impulse);

            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isFalling", isFalling);
        }

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isFalling", isFalling);
    }
}
