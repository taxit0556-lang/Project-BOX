using UnityEngine;

public class JumpVirtue: MonoBehaviour
{
    private bool playerInRange;
    private Player_Movement player;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            player.canJump = true;
            player.unlockedVirtues.Add("Jump");

            Debug.Log("Jump Unlocked!");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.GetComponent<Player_Movement>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}