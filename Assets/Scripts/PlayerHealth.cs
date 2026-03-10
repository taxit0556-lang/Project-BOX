using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public ParticleSystem healEffect;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public bool CanHeal()
    {
        return currentHealth < maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (healEffect != null)
        {
            Instantiate(healEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Player healed. Current health: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died");
        gameObject.SetActive(false);
    }
}