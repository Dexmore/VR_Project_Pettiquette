using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ItemInteraction : MonoBehaviour
{
    public ItemData itemData;
    private XRGrabInteractable grab;
    private bool released = false;

    private void Awake()
    {
        if (TryGetComponent(out grab) == false)
        {
            Debug.LogError($"[ItemInteraction] XRGrabInteractable이 없습니다: {name}");
            return;
        }

        grab.selectExited.AddListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        released = true;
    }

    private void Update()
    {
        // 오브젝트가 손에서 놓였고, 아직 파괴되지 않았으며, 버튼이 눌렸을 때
        if (released && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (itemData != null)
            {
                InventoryManager.Instance.AddItem(itemData);
                Destroy(gameObject);
            }
        }
    }
}
