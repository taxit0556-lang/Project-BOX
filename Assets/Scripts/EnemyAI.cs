using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    private Transform player;
    //private Player_Attack player_Attack;
    private Rigidbody2D rb;
    public Transform Target;
    public Enemy_Attack enemy_Attack;

    [Header("Movement")]
    public float speed = 3f;
    public float chaseRange = 8f;
    
    [Header("Patrol")]
    public Transform pathA;
    public Transform pathB;
    public float patrolWaitTime = 1.5f;
    private float patrolWaitTimer = 0f;
    private bool waiting = false;

    [Header("Values")]
    public string State;
    public float TimeInState;
    public bool Moving;
    float distance;
    public Vector2 SpawnPoint;

    void Awake()
    {
        //player_Attack = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Attack>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {  
        SetState("Patrol"); 
        FindPlayer();
        Target = pathA;
        Moving = true; // add this
    }


    void OnEnable()
    {
        Player_Death.OnPlayerDeath += HandlePlayerDeath;
    }

    void OnDisable()
    {
        Player_Death.OnPlayerDeath -= HandlePlayerDeath;
    }

    void HandlePlayerDeath()
    {
        StopAllCoroutines();
        transform.position = new Vector2(SpawnPoint.x, SpawnPoint.y);
    }

    void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");


        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        TimeInState += Time.deltaTime;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        distance = Vector2.Distance(transform.position, player.position);
        StateMachine();
    }

    void patrolling()
    {
        if (waiting)
        {
            patrolWaitTimer += Time.deltaTime;
            if (patrolWaitTimer >= patrolWaitTime)
            {
                waiting = false;
                patrolWaitTimer = 0f;
            }
            return;
        }

        if (!Moving)
        {
            Target = (Target == pathA) ? pathB : pathA;
            Moving = true;
        }

        Vector2 direction = (Target.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        if (Vector2.Distance(transform.position, Target.position) < 0.5f)
        {
            Moving = false;
            waiting = true;
            patrolWaitTimer = 0f;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void SetState(string newState)
    {
        if (State != newState)
        {
            State = newState;
            TimeInState = 0f;
        }
    }

    void StateMachine()
    {
        if (State == "Chase")
        {
            ChasePlayer();

            if (distance > chaseRange)
            {
                SetState("Idle");
            }
        }
        else if (State == "Idle")
        {
            StopMoving();

            if (distance <= chaseRange) { SetState("Chase"); return; }
            if (TimeInState > 2f) { SetState("Patrol"); }
        }
        else if (State == "Patrol")
        {
            if (distance <= chaseRange) { SetState("Chase"); return; }
            patrolling();
        }
    }
}