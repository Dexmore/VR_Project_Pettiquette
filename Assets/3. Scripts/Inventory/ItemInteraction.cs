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
            Debug.LogError($"[ItemInteraction] XRGrabInteractable�� �����ϴ�: {name}");
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
        // ������Ʈ�� �տ��� ������, ���� �ı����� �ʾ�����, ��ư�� ������ ��
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
