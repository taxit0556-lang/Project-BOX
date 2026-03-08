using UnityEngine;

public class Heal : MonoBehaviour
{
    public int healAmount = 2;

    public void Consume(PlayerHealth player)
    {
        if (player.CanHeal())
        {
            player.Heal(healAmount);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Health already full");
        }
    }
}