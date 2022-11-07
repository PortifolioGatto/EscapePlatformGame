using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]private float moveSpeed;
    [SerializeField]private float acceleration;
    [SerializeField]private float decceleration;
    [SerializeField]private float velPower;
    private float moveDir;

    [Header("Jump")]
    [SerializeField]private float jumpForce;
    private bool isJumping;
    [Header("Ground Detection")]
    [SerializeField]private float groundDetectDistance;
    [SerializeField]private LayerMask groundLayer;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = false;
        }

        ApplyMoveVelocity();
    }

    private void ApplyMoveVelocity()
    {
        float targetSpeed = moveDir * moveSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        rb.AddForce(movement * Vector2.right);
    }

    public void JumpFunction()
    {
        if(IsGrounded())
        {
            if(!isJumping)
            {
                isJumping = true;
            }
        }
    }

    public void ChangeMoveVelocity(float dir)
    {
        moveDir = dir;
    }

    private bool IsGrounded()
    {
        bool isGrounded = false;

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(1f,1f),0f,Vector2.down, groundDetectDistance, groundLayer);
        
        if(hit.collider != null)   isGrounded = true;

        return isGrounded;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDetectDistance);
    }
}
