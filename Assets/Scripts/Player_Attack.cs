using UnityEngine;
using System.Collections;

public class Player_Attack : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider2D;

    private bool PlayerAttacking;
    private bool PlayerHitSomething;

    EnemyHealth EnemyHit;

    public int damage = 1;

    void Start()
    {
        PlayerAttacking = false;
        boxCollider2D.enabled = false;
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