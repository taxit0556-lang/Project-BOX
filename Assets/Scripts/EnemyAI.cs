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

    // start ray slightly above feet
    Vector2 origin = (Vector2)transform.position + Vector2.down * 0.4f;

    RaycastHit2D wall = Physics2D.Raycast(
        origin,
        Vector2.right * direction,
        0.9f,
        obstacleLayer
    );

    if (!wall)
        return;

    // check if space above the obstacle is free
    Vector2 headOrigin = (Vector2)transform.position + Vector2.up * 0.6f;

    RaycastHit2D ceiling = Physics2D.Raycast(
        headOrigin,
        Vector2.right * direction,
        0.7f,
        obstacleLayer
    );

    if (ceiling)
        return;

    rb.linearVelocity = new Vector2(direction * speed, jumpForce);
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

        Gizmos.DrawWireCube(
            transform.position + Vector3.right * wallCheckDistance,
            wallCheckSize
        );

        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(
            transform.position + Vector3.up * headCheckDistance,
            headCheckSize
        );
    }
}