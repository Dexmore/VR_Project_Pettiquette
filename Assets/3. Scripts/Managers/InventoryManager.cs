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

            // 인벤토리 UI 새로고침
            InventoryUIController ui = FindObjectOfType<InventoryUIController>();
            if (ui != null)
                ui.RefreshUI();

            // 체크 UI 새로고침
            CheckItemUImanager checker = FindObjectOfType<CheckItemUImanager>();
            if (checker != null)
                checker.RefreshCheckUI();
        }
    }

    public void RemoveItem(ItemData item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
            Debug.Log("인벤토리에서 " + item.itemName + " 제거됨");

            // 인벤토리 UI 새로고침
            InventoryUIController ui = FindObjectOfType<InventoryUIController>();
            if (ui != null)
                ui.RefreshUI();

            // 체크 UI 새로고침
            CheckItemUImanager checker = FindObjectOfType<CheckItemUImanager>();
            if (checker != null)
                checker.RefreshCheckUI();
        }
    }

    public void ClearInventory()
    {
        Items.Clear();

        // UI 갱신도 같이
        FindObjectOfType<InventoryUIController>()?.RefreshUI();
        FindObjectOfType<CheckItemUImanager>()?.RefreshCheckUI();

        Debug.Log("[InventoryManager] 인벤토리 초기화됨");
    }

}
