using UnityEngine;
using System.Collections;

public class Player_Attack : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider2D;

    private bool PlayerAttacking;
    private bool PlayerHitSomething;

    EnemyAI EnemyHit;

    void Start()
    {
        PlayerAttacking = false;
    }

    IEnumerator Attacking()
    {
        PlayerAttacking = true;
        yield return new WaitForSeconds(0.5f);
        PlayerAttacking = false;
        PlayerHitSomething = false;
    }
    
    void Update()
    {
        if (PlayerAttacking)
        {
            boxCollider2D.enabled = true;
        }
        else
            boxCollider2D.enabled = false;


        if(Input.GetMouseButtonDown(0) && !PlayerAttacking)
            StartCoroutine("Attacking");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!PlayerHitSomething)
            {
                EnemyHit = other.gameObject.GetComponent<EnemyAI>();
                Debug.Log("HitEnemy");
                EnemyHit.OnHit(10);
                PlayerHitSomething = true;
            }
        }
    }
}
