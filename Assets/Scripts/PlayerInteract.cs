using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 2f;
    public Transform holdPoint;

    private GameObject storedItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");
            TryPickup();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TryConsume();
        }
    }

    void TryPickup()
    {
        if (storedItem != null)
            return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange);

        if (hit == null) return;

        // Jump Orb Unlock
        if (hit.CompareTag("JumpOrb"))
        {
            Player_Movement movement = GetComponent<Player_Movement>();

            if (movement != null)
            {
                movement.canJump = true;

                if (!movement.unlockedVirtues.Contains("Jump"))
                    movement.unlockedVirtues.Add("Jump");

                Debug.Log("Jump Unlocked!");
            }

            Destroy(hit.gameObject);
            return;
        }

        // Pickup items
        if (hit.CompareTag("Pickup"))
        {
            storedItem = hit.gameObject;

            Rigidbody2D rb = storedItem.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.simulated = false;

            storedItem.SetActive(false);

            Debug.Log("Picked up: " + storedItem.name);
        }
    }

    void TryConsume()
    {
        if (storedItem == null)
            return;

        Heal heal= storedItem.GetComponent<Heal>();

        if (heal != null)
        {
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();

            if (playerHealth != null && playerHealth.CanHeal())
            {
                heal.Consume(playerHealth);
                storedItem = null;
            }
            else
            {
                Debug.Log("Health already full");
            }
        }
    }

    void DropItem()
    {
        if (storedItem == null) return;

        storedItem.SetActive(true);
        storedItem.transform.position = holdPoint.position;

        Rigidbody2D rb = storedItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        storedItem = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}