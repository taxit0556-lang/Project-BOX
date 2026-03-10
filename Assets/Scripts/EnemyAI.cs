using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Idle
    }

    [Header("References")]
    private Transform player;
    private Rigidbody2D rb;
    //ADDED becuase enemy jump is very floaty
    [Header("Gravity")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Movement")]
    public float speed = 3f;
    public float jumpForce = 7f;
    public float chaseRange = 8f;

    [Header("Patrol")]
    public Transform pathA;
    public Transform pathB;
    private Transform target;

    public float patrolWaitTime = 1.5f;
    private float patrolTimer;

    [Header("Obstacle Detection")]
    public Vector2 wallCheckSize = new Vector2(0.6f, 0.8f);
    public float wallCheckDistance = 0.6f;

    public Vector2 headCheckSize = new Vector2(0.5f, 0.3f);
    public float headCheckDistance = 0.9f;

    public LayerMask obstacleLayer;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Debug")]
    public EnemyState state;
    public float timeInState;

    float distanceToPlayer;
    bool waiting;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        target = pathA;
        FindPlayer();
        SetState(EnemyState.Patrol);
    }

    void Update()
    {
        timeInState += Time.deltaTime;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        StateMachine();
    }

    void FindPlayer()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;
    }
//Added these both to make enemy jumps less floaty
    void FixedUpdate()
    {
         BetterFall();
    }

void BetterFall()
{
    if (rb.linearVelocity.y < 0)
    {
        rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
    }
    else if (rb.linearVelocity.y > 0)
    {
        rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }
}

    void StateMachine()
    {
        switch (state)
        {
            case EnemyState.Patrol:

                if (distanceToPlayer <= chaseRange)
                {
                    SetState(EnemyState.Chase);
                    return;
                }

                Patrol();
                break;

            case EnemyState.Chase:

                if (distanceToPlayer > chaseRange)
                {
                    SetState(EnemyState.Idle);
                    return;
                }

                ChasePlayer();
                break;

            case EnemyState.Idle:

                StopMoving();

                if (distanceToPlayer <= chaseRange)
                {
                    SetState(EnemyState.Chase);
                    return;
                }

                if (timeInState > 2f)
                {
                    SetState(EnemyState.Patrol);
                }

                break;
        }
    }

    void Patrol()
    {
        if (waiting)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                waiting = false;
                patrolTimer = 0;
                target = (target == pathA) ? pathB : pathA;
            }

            return;
        }

        MoveTowards(target.position);

        if (Vector2.Distance(transform.position, target.position) < 0.4f)
        {
            waiting = true;
            StopMoving();
        }
    }

    void ChasePlayer()
    {
        MoveTowards(player.position);
    }

    void MoveTowards(Vector2 destination)
    {
        float direction = Mathf.Sign(destination.x - transform.position.x);

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        TryJump(direction);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

  void TryJump(float direction)
{
    if (!IsGrounded())
        return;

    Vector2 origin = (Vector2)transform.position + Vector2.down * 0.2f;

    // DEBUG LINE
    Debug.DrawRay(origin, Vector2.right * direction * wallCheckDistance, Color.red);

    RaycastHit2D wall = Physics2D.BoxCast(
        origin,
        wallCheckSize,
        0f,
        Vector2.right * direction,
        wallCheckDistance,
        obstacleLayer
    );

    if (wall.collider == null)
        return;

    Vector2 headOrigin = (Vector2)transform.position + Vector2.up * 0.6f;

    RaycastHit2D ceiling = Physics2D.BoxCast(
        headOrigin,
        headCheckSize,
        0f,
        Vector2.right * direction,
        headCheckDistance,
        obstacleLayer
    );

    if (ceiling.collider != null)
        return;

    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
}
    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void SetState(EnemyState newState)
    {
        if (state != newState)
        {
            state = newState;
            timeInState = 0;
        }
    }

   void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;

    float dir = 1;

    if (Application.isPlaying && player != null)
        dir = Mathf.Sign(player.position.x - transform.position.x);

    Vector3 wallPos = transform.position + Vector3.right * dir * wallCheckDistance;

    Gizmos.DrawWireCube(wallPos, wallCheckSize);

    Gizmos.color = Color.blue;

    Vector3 headPos = transform.position + Vector3.right * dir * headCheckDistance + Vector3.up * 0.6f;

    Gizmos.DrawWireCube(headPos, headCheckSize);
}
}