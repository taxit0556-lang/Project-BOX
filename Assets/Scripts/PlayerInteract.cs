using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 2f;
    public Transform holdPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");
            TryPickup();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q pressed");
            DropItem();
        }
    }

    void TryPickup()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange);

        if (hit == null)
        {
            Debug.Log("Nothing to pick up");
            return;
        }

        if (!hit.CompareTag("Pickup"))
        {
            Debug.Log("Object not tagged as Pickup");
            return;
        }

        GameObject obj = hit.gameObject;

        PickupItem pickup = obj.GetComponent<PickupItem>();

        if (pickup == null)
        {
            Debug.Log("No PickupItem script found!");
            return;
        }

        // Add to inventory
        InventoryManager.instance.AddItem(pickup.itemData);

        Debug.Log("Picked up: " + pickup.itemData.itemName);

        // Remove from world
        Destroy(obj);
    }

    void DropItem()
{
    InventoryItem item = InventoryManager.instance.RemoveLastItem();

    if (item == null)
    {
        Debug.Log("No item to drop");
        return;
    }

    Instantiate(item.prefab, holdPoint.position, Quaternion.identity);

    Debug.Log("Dropped: " + item.itemName);
}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}