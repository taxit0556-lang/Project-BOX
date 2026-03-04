using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [Header("Assign in Inspector")]
    public GameObject inventoryPanel;

    void Awake()
    {
        instance = this;

        if (inventoryPanel == null)
        {
            Debug.LogError("InventoryPanel is NOT assigned!");
            return;
        }

        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

    public void RefreshUI()
    {
        Debug.Log("Refreshing Inventory UI");
        // we’ll fill this later
    }
}