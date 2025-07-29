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
                    // 1. ������ ����
                    InventoryManager.Instance.RemoveItem(currentData);

                    // 2. ������ ����
                    SpawnItemInFrontOfPlayer(currentData);
                }
            });
        }
    }

    private void SpawnItemInFrontOfPlayer(ItemData data)
    {
        if (data.itemPrefab == null)
        {
            Debug.LogWarning($"[InventorySlot] {data.itemName}�� �������� �������");
            return;
        }

        Transform playerCam = Camera.main.transform; // VR������ XR Origin�� ī�޶�
        Vector3 spawnPos = playerCam.position + playerCam.forward * 1.5f + Vector3.down * 0.3f;
        Quaternion spawnRot = Quaternion.identity;

        Instantiate(data.itemPrefab, spawnPos, spawnRot);
    }
}