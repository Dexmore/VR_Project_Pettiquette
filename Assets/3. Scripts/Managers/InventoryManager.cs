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

        Debug.Log("[InventoryManager] Awake �Ϸ�. ���� ������ ����: " + Items.Count);
    }

    public void AddItem(ItemData item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
            Debug.Log("�κ��丮�� " + item.itemName + " �߰���");

            // �κ��丮 UI ���ΰ�ħ
            InventoryUIController ui = FindObjectOfType<InventoryUIController>();
            if (ui != null)
                ui.RefreshUI();

            // üũ UI ���ΰ�ħ
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
            Debug.Log("�κ��丮���� " + item.itemName + " ���ŵ�");

            // �κ��丮 UI ���ΰ�ħ
            InventoryUIController ui = FindObjectOfType<InventoryUIController>();
            if (ui != null)
                ui.RefreshUI();

            // üũ UI ���ΰ�ħ
            CheckItemUImanager checker = FindObjectOfType<CheckItemUImanager>();
            if (checker != null)
                checker.RefreshCheckUI();
        }
    }

    public void ClearInventory()
    {
        Items.Clear();

        // UI ���ŵ� ����
        FindObjectOfType<InventoryUIController>()?.RefreshUI();
        FindObjectOfType<CheckItemUImanager>()?.RefreshCheckUI();

        Debug.Log("[InventoryManager] �κ��丮 �ʱ�ȭ��");
    }

}
