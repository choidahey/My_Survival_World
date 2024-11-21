using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private float walkSpeed = 5f;
    private float runScale = 5f;
    private float turnSpeed = 200f;
    private float jumpForce = 7f;

    private bool wasGrounded;
    private bool isGrounded;
    private bool jumpInput = false;

    private float currentH = 0f;
    private float currentV = 0f;

    private float jumpTimeStamp = 0f;
    private float minJumpInterval = 0.25f;

    private Vector3 currentDirection = Vector3.zero;

    private Animator animator;
    private Rigidbody rigidBody;
    private List<Collider> collisions = new List<Collider>();


    private void Awake()
    {
        if (GetComponent<Animator>() != null)
            animator = GetComponent<Animator>();

        if (GetComponent<Rigidbody>() != null)
            rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!jumpInput && Input.GetKey(KeyCode.Space))
        {
            jumpInput = true;
        }
    }

    private void FixedUpdate()
    {
        animator.SetBool("isGrounded", isGrounded);

        Move();
        Jump();

        wasGrounded = isGrounded;
        jumpInput = false;
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

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(jumpForce * -Physics.gravity.y);
            rigidBody.AddForce(jumpVelocity, ForceMode.Impulse);
            isGrounded = false;
        }
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
