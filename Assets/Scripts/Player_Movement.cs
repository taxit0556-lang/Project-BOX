using UnityEngine;
//this is a celeste type of movement script
public class Player_Movement : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed;
    public float jumpingPower;
    public float CyoteTime;
    public float jumpBuffering_Window;


    private float jumpBuffering_Timer;
    private bool CanjumpBuffer;
    private bool StartjumpBuffer_Timer;

    [Header("WallClimbing")]
    public bool isWallSliding;
    public bool isWallJumping;
    public bool SameWallJumping;
    private float wallSlidingSpeed = 2f;
    private float wallJumpTime = 0.2f;
    public float wallJumpDirection;
    public float LastTimeWallJumped;
    public bool StartWallJumpTimer;

    private float wallJumpCounter;
    public Vector2 wallJumpingPower;

    public string State;

    Rigidbody2D rb;
    public Vector2 movement;

    [Header("GroundCheck / WallCheck")]
    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private Transform GroundCheck;
    [SerializeField]private LayerMask WallLayer;
    [SerializeField]private Transform WallCheck;
    private float LastTimeGrounded;
    private float TimeGrounded;

    [SerializeField] private SpriteRenderer spriteRenderer;

    void Awake()
    {
        jumpBuffering_Timer = 1000000000;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        int RoundedMoveSpeed = (int)MoveSpeed;
        wallJumpingPower = new Vector2(MoveSpeed, 13f);

        Vector3 localScale = transform.localScale;

        if (movement.x > 0 && !isWallJumping && !isWallSliding)
        {
            localScale.x = 1;
            transform.localScale = localScale;
        }
        else if (movement.x < 0 && !isWallJumping && !isWallSliding)
        {
            localScale.x = -1;
            transform.localScale = localScale;
        }

        //normal Jump
        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }
        if(Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        if(!IsGrounded())
        {
            if (isWallJumping)
            {
                StartWallJumpTimer = true;
            }

            if(StartWallJumpTimer)
                LastTimeWallJumped += 1 * Time.deltaTime;


            if (isWallSliding)
            {
                StartWallJumpTimer = false;
                LastTimeWallJumped = 0;
            }
        }
        JumpBuffering();
        WallSlide();
        WallJump();
    }

    void FixedUpdate()
    {
        if (!IsGrounded())
        {
            LastTimeGrounded += 1 * Time.deltaTime;

            TimeGrounded = 0;
        }
        else
        {
            TimeGrounded += 1 * Time.deltaTime;
            
            LastTimeGrounded = 0;
        }

        if(!isWallJumping)
        {
            rb.linearVelocity = new Vector2(movement.x * MoveSpeed, rb.linearVelocity.y);
        }
        else if (!isWallJumping && !IsGrounded() && movement.x == 0)
        {
            Debug.Log("NotMoving");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        }
        if(rb.linearVelocity.y > 0)
            rb.gravityScale = 2f;

        else if(rb.linearVelocity.y < 0)
            rb.gravityScale = 3.5f;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, 0.3f, groundLayer);
    }
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(WallCheck.position, 0.2f, WallLayer);
    }
    private void WallSlide()
    {
        if(IsWalled() && !IsGrounded() && movement.x != 0 || IsWalled() && !IsGrounded() && LastTimeWallJumped > 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }


    private void WallJump()
    {
        Vector3 localScale = transform.localScale;
        if (isWallSliding)
        {
            isWallJumping = false;
            if(transform.localScale.x > 0)
                wallJumpDirection = -transform.localScale.x;

            if(transform.localScale.x < 0)
                wallJumpDirection = -transform.localScale.x;

            wallJumpCounter = wallJumpTime;

            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Space) && wallJumpCounter > 0f)
        {    
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpCounter = 0f;

            if(transform.localScale.x != wallJumpDirection)
            {
                if(wallJumpDirection > 0)
                    localScale.x = 1f;

                if(wallJumpDirection < 0)
                    localScale.x = -1f;

                    
                transform.localScale = localScale;
            }
            Invoke(nameof(StopWallJump), 0.3f);
        }

        if (IsGrounded() && isWallJumping)
        {
           StopWallJump();
        }



        if(-movement.x == wallJumpDirection && isWallJumping && LastTimeWallJumped >= 0.05f)
        {             
            transform.localScale = localScale;
            SameWallJumping = true;
        }
        else
        {
            SameWallJumping = false;
        }
    }
    private void StopWallJump()
    {
        isWallJumping = false;
        SameWallJumping = false;
    }

    void JumpBuffering()

    {
        if(!IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            jumpBuffering_Timer = 0;
            StartjumpBuffer_Timer = true;
        }
        else if(jumpBuffering_Timer > jumpBuffering_Window)
            StartjumpBuffer_Timer = false;


        if(jumpBuffering_Timer < jumpBuffering_Window)
        {
            CanjumpBuffer = true;
        }
        else
            CanjumpBuffer = false;


        if (CanjumpBuffer && IsGrounded())
        {
            CanjumpBuffer = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);

            if(Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }

        if (StartjumpBuffer_Timer)
        {
            jumpBuffering_Timer += 1 * Time.deltaTime;
        }
        else
            jumpBuffering_Timer = jumpBuffering_Window;
    }
}
