using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float chaseRange = 8f;

    private Rigidbody2D rb;

    public float Health;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if(distance < chaseRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }


        if(Health <= 0)
            Destroy(gameObject);
    }

    public void OnHit(float HitAmount)
    {
        Health -= HitAmount;
    }
}