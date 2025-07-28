using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ItemInteraction : MonoBehaviour
{
    private XRGrabInteractable grab;
    private bool isHeld = false;

    [Header("Input")]
    public InputActionReference inventoryAddAction; // ← Input Action에 연결 (B 버튼)

    [Header("Item Info")]
    public ItemData itemData;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        inventoryAddAction.action.performed += OnInventoryAdd;
    }

    private void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);

        inventoryAddAction.action.performed -= OnInventoryAdd;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    private void OnInventoryAdd(InputAction.CallbackContext context)
    {
        if (isHeld)
        {
            InventoryManager.Instance.AddItem(itemData); // 실제 등록
            Destroy(gameObject);
        }
    }
}
