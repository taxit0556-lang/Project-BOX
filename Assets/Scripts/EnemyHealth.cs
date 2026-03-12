using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 25;
    public float knockbackForce = 10f;
    public float hitStunDuration = 1f;

    int currentHealth;
    public bool gotHit;
    Rigidbody2D rb;
    Coroutine hitCoroutine;

    EnemyAI enemyAI;
    Enemy_Attack enemy_Attack;

    void Start()
    {
        enemy_Attack = GetComponent<Enemy_Attack>();
        enemyAI = GetComponent<EnemyAI>();

        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if (!enemy_Attack.Attacktrigger)
        {
            if(!enemy_Attack.afterAttack)
                enemyAI.SetState("Stuned");
                
            currentHealth -= damage;
            Debug.Log("Enemy health: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            float direction = transform.position.x > attacker.position.x ? 1f : -1f;

            if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(GotHit(direction));
        }
    }

    IEnumerator GotHit(float direction)
    {
        gotHit = true;

        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(new Vector2(direction * knockbackForce, 0), ForceMode2D.Impulse);

        yield return new WaitForSeconds(hitStunDuration);

        gotHit = false;
        hitCoroutine = null;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}