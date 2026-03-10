using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 25;
    int currentHealth;
    public bool gotHit;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        StartCoroutine(GotHit());
        currentHealth -= damage;

        Debug.Log("Enemy health: " + currentHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator GotHit()
    {
        gotHit = true;
        yield return new WaitForSeconds(1f);  
        gotHit = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}