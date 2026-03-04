using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<InventoryItem> items = new List<InventoryItem>();

    void Awake()
    {
        instance = this;
    }

    public void AddItem(InventoryItem item)
    {
        items.Add(item);
        Debug.Log("Added: " + item.itemName);

        InventoryUI.instance.RefreshUI();
    }

    public InventoryItem RemoveLastItem()
    {
        if (items.Count == 0) return null;

        InventoryItem item = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);

        InventoryUI.instance.RefreshUI();

        return item;
    }
}