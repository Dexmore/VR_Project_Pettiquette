using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUIController : MonoBehaviour
{
    [Header("슬롯 프리팹과 부모")]
    public GameObject slotPrefab;
    public Transform slotParent;

    [Header("UI Button")]
    public Button button;

    [Header("닫을 대상")]
    public GameObject closeTarget;
    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (closeTarget != null)
                {
                    closeTarget.SetActive(false);
                    UIManager.Instance.CloseAllSubCanvas();
                }
            });
        }
    }
    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("[InventoryUIController] InventoryManager.Instance가 null입니다.");
            return;
        }

        if(InventoryManager.Instance.Items == null)
        {
            Debug.LogWarning("[InventoryUIController] InventoryManager.Instance.Items가 null입니다.");
        }

        Debug.Log("[InventoryUIController] RefreshUI 시작");

        // 기존 슬롯 삭제
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        // 슬롯 20개 생성 (5x4 그리드)
        int totalSlotCount = 20;
        List<ItemData> items = InventoryManager.Instance.Items;

        for (int i = 0; i < totalSlotCount; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotParent);

            InventorySlot slot = slotGO.GetComponent<InventorySlot>();
            if (slot != null)
            {
                if (i < items.Count)
                    slot.InitItem(items[i]); // 아이템 할당
                else
                    slot.InitItem(null);     // 빈 칸
            }
        }
    }
}
