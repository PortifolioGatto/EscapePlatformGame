using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]private float moveSpeed = 12f;
    [SerializeField]private float acceleration = 22f;
    [SerializeField]private float decceleration = 15.5f;
    [SerializeField]private float velPower = 0.9f;
    [SerializeField]private float frictionAmount = 0.2f;
    private float moveDir = 0;

    [Header("Jump")]
    [SerializeField]private float jumpForce = 15f;
    [SerializeField]private float jumpBufferTime = .15f;
    [SerializeField]private float jumpCoyoteTime = .15f;
    [SerializeField]private float jumpCutMultiplier = 1f;

    [SerializeField]private float gravityScale = 4f;
    [SerializeField]private float fallGravityMultiplier = 1.5f;
    private float lastGroundedTime = 0;
    private float lastJumpTime = 0;

    private bool jumpInputReleased = false;
    private bool isJumping;
    [Header("Ground Detection")]
    [SerializeField]private float groundDetectDistance = 0.6f;
    [SerializeField]private LayerMask groundLayer;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        lastGroundedTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if(rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }else{
            rb.gravityScale = gravityScale;
        }

        JumpFunction();
        ApplyMoveVelocity();
        ApplyFriction();
    }

    private void ApplyFriction()
    {
        if(IsGrounded() && Mathf.Abs(moveDir) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));

            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }
    private void ApplyMoveVelocity()
    {
        float targetSpeed = moveDir * moveSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        rb.AddForce(movement * Vector2.right);
    }

    public void OnJump()
    {
        lastJumpTime = jumpBufferTime;
    }
    public void OnJumpUp()
    {
        if(rb.velocity.y > 0 && isJumping)
        {
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }

        jumpInputReleased = true;
        lastJumpTime = 0;
    }
    private void JumpFunction()
    {
        if(lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastGroundedTime = 0;
            lastJumpTime = 0;
            isJumping = true;
            jumpInputReleased = false;
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
        
        if(hit.collider != null)
        {
            if(rb.velocity.y <= 0f)
                isJumping = false;
            lastGroundedTime = jumpCoyoteTime;
            isGrounded = true;
        }   

        return isGrounded;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDetectDistance);
    }
}
