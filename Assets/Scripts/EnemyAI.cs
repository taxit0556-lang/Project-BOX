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
    public Vector2 boxCastSize = new Vector2(0.6f, 0.6f);
    public float boxCastDistance = 0.7f;
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

    Vector2 origin = transform.position;

    RaycastHit2D hit = Physics2D.BoxCast(
        origin,
        boxCastSize,
        0f,
        Vector2.right * direction,
        boxCastDistance,
        obstacleLayer
    );

    if (hit.collider != null)
    {
        rb.linearVelocity = new Vector2(direction * speed, jumpForce);
    }
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

        Vector3 dir = Vector3.right;

        Gizmos.DrawWireCube(
            transform.position + dir * boxCastDistance,
            boxCastSize
        );
    }
}