using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<ItemData> Items = new List<ItemData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[InventoryManager] Awake 완료. 현재 아이템 개수: " + Items.Count);
    }

    public void AddItem(ItemData item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
            Debug.Log("인벤토리에 " + item.itemName + " 추가됨");

            InventoryUIController ui = FindObjectOfType<InventoryUIController>();
            if (ui != null)
                ui.RefreshUI(); // 수동 호출
        }
    }

}
