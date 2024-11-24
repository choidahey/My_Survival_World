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
    //private float checkDistance = 1.1f;
    private bool isJumpAir = false;
    private bool isFalling = false;

    private float currentH = 0f;
    private float currentV = 0f;

    private Vector3 currentDirection = Vector3.zero;
    private LayerMask groundLayer;

    private Animator animator;
    private Rigidbody rigidBody;
    private List<Collider> collisions = new List<Collider>();


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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= runScale;
            h *= runScale;
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
            Debug.Log("MoveSpeed" + direction.magnitude);  // walk 는 최대 1, run은 최대 5
        }
    }

    //private bool IsGrounded()
    //{
    //    RaycastHit hit;
        

    //    if (Physics.Raycast(this.transform.position, Vector3.down, out hit, this.checkDistance, groundLayer))
    //        return true;
        
    //    return false;
    //}

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)  // 점프 버튼이 눌렸고 땅에 닿아있을 때 점프 가능
        {
            isGrounded = false;

            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(jumpForce * -Physics.gravity.y);
            rigidBody.AddForce(jumpVelocity, ForceMode.Impulse);

            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isFalling", isGrounded);
        }
        Debug.Log("isGrounded " + isGrounded);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isFalling", isGrounded);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;

        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!collisions.Contains(collision.collider))
                {
                    collisions.Add(collision.collider);
                }
                isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            isGrounded = true;
            if (!collisions.Contains(collision.collider))
            {
                collisions.Add(collision.collider);
            }
        }
        else
        {
            if (collisions.Contains(collision.collider))
            {
                collisions.Remove(collision.collider);
            }
            if (collisions.Count == 0) { isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collisions.Contains(collision.collider))
        {
            collisions.Remove(collision.collider);
        }
        if (collisions.Count == 0) { isGrounded = false; }
    }
}
