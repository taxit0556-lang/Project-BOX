using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Heal Effect")]
    public ParticleSystem healEffect;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Player took damage. Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Player healed. Health: " + currentHealth);

        if (healEffect != null)
        {
            Instantiate(healEffect, transform.position, Quaternion.identity);
        }
    }

    void Die()
    {
        Debug.Log("Player died");

        // For now disabling player
        gameObject.SetActive(false);

        // Later addding respawn
    }


    public bool CanHeal()
    {   //does not allow player to cap health when he eats heal.
        return currentHealth < maxHealth;
    }
}