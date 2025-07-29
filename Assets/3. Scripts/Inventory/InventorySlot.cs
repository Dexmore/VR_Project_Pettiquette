using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;
    public Button removeButton;

    private ItemData currentData;

    public void InitItem(ItemData data)
    {
        currentData = data;

        if (data == null)
        {
            iconImage.enabled = false;
            removeButton.interactable = false;
            return;
        }

        iconImage.sprite = data.itemIcon;
        iconImage.enabled = true;
        removeButton.interactable = true;
    }

    private void Start()
    {
        if (removeButton != null)
        {
            removeButton.onClick.AddListener(() =>
            {
                if (currentData != null)
                {
                    // 1. 아이템 제거
                    InventoryManager.Instance.RemoveItem(currentData);

                    // 2. 아이템 생성
                    SpawnItemInFrontOfPlayer(currentData);
                }
            });
        }
    }

    private void SpawnItemInFrontOfPlayer(ItemData data)
    {
        if (data.itemPrefab == null)
        {
            Debug.LogWarning($"[InventorySlot] {data.itemName}의 프리팹이 비어있음");
            return;
        }

        Transform playerCam = Camera.main.transform; // VR에서는 XR Origin의 카메라
        Vector3 spawnPos = playerCam.position + playerCam.forward * 1.5f + Vector3.down * 0.3f;
        Quaternion spawnRot = Quaternion.identity;

        Instantiate(data.itemPrefab, spawnPos, spawnRot);
    }
}