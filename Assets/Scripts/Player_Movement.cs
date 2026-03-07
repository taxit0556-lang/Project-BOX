using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Jump")]
    public float jumpForce = 14f;
    public float coyoteTime = 0.12f;
    public float jumpBuffer = 0.12f;

    float coyoteCounter;
    float jumpBufferCounter;

    [Header("Jump Lock")]
    public bool canJump = true;

    [Header("Virtues")]
    public List<string> unlockedVirtues = new List<string>();

    [Header("Dash")]
    public float dashSpeed = 22f;
    public float dashTime = 0.18f;
    public float dashFreezeTime = 0.05f;

    bool isDashing;
    bool canDash = true;

    [Header("Wall")]
    public Vector2 wallJumpForce = new Vector2(12f,16f);
    public float wallJumpLockTime = 0.18f;

    bool isWallJumping;
    float wallJumpLockCounter;

    public float wallStickTime = 0.15f;
    float wallStickCounter;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 1.5f;
    public float wallSlideGravity = 0.5f;

    bool isWallSliding;

    [Header("Checks")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    Rigidbody2D rb;
    SpriteRenderer sprite;

    float horizontal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        Flip();

        HandleTimers();

        HandleInput();

        HandleWallSlide();

        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowVirtues();
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        Move();
        Gravity();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBuffer;

        // NORMAL JUMP
        if (jumpBufferCounter > 0 && coyoteCounter > 0 && canJump && !isWallSliding)
        {
            Jump();
        }

        // WALL JUMP
        if (Input.GetKeyDown(KeyCode.Space) && isWallSliding && !IsGrounded())
        {
            WallJump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void Move()
    {
        if (isWallJumping)
        {
            wallJumpLockCounter -= Time.fixedDeltaTime;

            if (wallJumpLockCounter <= 0)
                isWallJumping = false;

            return;
        }

        float targetSpeed = horizontal * moveSpeed;

        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        jumpBufferCounter = 0;
        coyoteCounter = 0;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x,0);

        rb.AddForce(Vector2.up * jumpForce,ForceMode2D.Impulse);
    }

    void WallJump()
    {
        Debug.Log("WallJumped");

        isWallJumping = true;
        isWallSliding = false;

        wallJumpLockCounter = wallJumpLockTime;

        float direction = -transform.localScale.x;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(new Vector2(direction * wallJumpForce.x, wallJumpForce.y),ForceMode2D.Impulse);
    }

    void HandleWallSlide()
    {
        if (isWallJumping) return;

        if (IsWalled() && !IsGrounded() && rb.linearVelocity.y < 0)
        {
            if (horizontal != 0)
            {
                isWallSliding = true;
                wallStickCounter = wallStickTime;
            }
            else
            {
                wallStickCounter -= Time.deltaTime;

                if (wallStickCounter > 0)
                    isWallSliding = true;
                else
                    isWallSliding = false;
            }
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rb.gravityScale = wallSlideGravity;

            if (rb.linearVelocity.y < -wallSlideSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x,-wallSlideSpeed);
            }
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalTimeScale = Time.timeScale;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(dashFreezeTime);
        Time.timeScale = originalTimeScale;

        Vector2 dir = new Vector2(horizontal, Input.GetAxisRaw("Vertical"));

        if (dir == Vector2.zero)
            dir = new Vector2(transform.localScale.x,0);

        dir.Normalize();

        rb.gravityScale = 0;
        rb.linearVelocity = dir * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = 3;
        isDashing = false;
    }

    void HandleTimers()
    {
        if (IsGrounded())
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        jumpBufferCounter -= Time.deltaTime;

        if (IsGrounded())
            canDash = true;
    }

    void Gravity()
    {
        if (isWallSliding) return;

        if (rb.linearVelocity.y < 0)
            rb.gravityScale = 6f;
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
            rb.gravityScale = 5f;
        else
            rb.gravityScale = 3f;
    }

    void Flip()
    {
        if (horizontal > 0)
            transform.localScale = new Vector3(1,1,1);

        if (horizontal < 0)
            transform.localScale = new Vector3(-1,1,1);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position,0.25f,groundLayer);
    }

    bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position,0.25f,wallLayer);
    }

    void ShowVirtues()
    {
        Debug.Log("Unlocked Virtues:");

        foreach(string virtue in unlockedVirtues)
        {
            Debug.Log("- " + virtue);
        }
    }
}