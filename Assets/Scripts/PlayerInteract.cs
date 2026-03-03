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
    }

    void TryPickup()
    {
        Debug.Log("TryPickup CALLED");
        Debug.Log("storedItem: " + storedItem);

        if (storedItem != null)
        {
            Debug.Log("EXITING because already holding");
            return;
        }

        // Changed from Raycast to OverlapCircle
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange);

        if (hit != null && hit.CompareTag("Pickup"))
        {
            storedItem = hit.gameObject;

            Rigidbody2D rb = storedItem.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.simulated = false;

            storedItem.SetActive(false);

            Debug.Log("Picked up: " + storedItem.name);
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