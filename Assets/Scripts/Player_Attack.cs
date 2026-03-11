using UnityEngine;
using System.Collections;

public class Player_Attack : MonoBehaviour
{
    [Header("Values")]
    public int damage = 1;
    public float Health = 100;
    public float knockbackForce;


    [Header("Bools")]
    public bool PlayerAttacking;
    private bool PlayerHitSomething;
    public bool GotHit;

    [Header("Refs")]
    [SerializeField] private BoxCollider2D boxCollider2D;
    EnemyHealth EnemyHit;
    Rigidbody2D rb;


    void Start()
    {
        PlayerAttacking = false;
        boxCollider2D.enabled = false;

        rb = GetComponent<Rigidbody2D>();
    }

    IEnumerator Attacking()
    {
        PlayerAttacking = true;
        PlayerHitSomething = false;

        yield return new WaitForSeconds(0.5f);

        PlayerAttacking = false;
    }
    
    void Update()
    {
        if (PlayerAttacking)
            boxCollider2D.enabled = true;
        else
            boxCollider2D.enabled = false;

        if(Input.GetMouseButtonDown(0) && !PlayerAttacking)
            StartCoroutine(Attacking());

        if(Health <= 0)
        {
            GetComponent<Player_Death>().OnDeath();
            Health = 100;
        }
    }

    public void OnHit(float StunTime , Transform Direction)
    {
        StartCoroutine(onHit(StunTime, Direction));
    } 

    IEnumerator onHit(float StunTime , Transform Direction)
    {
        GotHit = true;

        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(new Vector2(Direction.localScale.x * knockbackForce, 0), ForceMode2D.Impulse);

        yield return new WaitForSeconds(StunTime);
        GotHit = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") && !PlayerHitSomething)
        {
            EnemyHit = other.GetComponent<EnemyHealth>();

            if(EnemyHit != null)
            {
                Debug.Log("HitEnemy");

                EnemyHit.TakeDamage(damage, transform);

                PlayerHitSomething = true;
            }
        }
    }
}