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

            InventoryUIController ui = FindObjectOfType<InventoryUIController>();
            if (ui != null)
                ui.RefreshUI(); // ���� ȣ��
        }
    }

}
