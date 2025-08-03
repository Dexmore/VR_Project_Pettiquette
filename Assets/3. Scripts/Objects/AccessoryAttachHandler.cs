using UnityEngine;

public class AccessoryAttachHandler : MonoBehaviour
{
    public enum AccessoryType { Leash, Muzzle }
    public AccessoryType accessoryType;

    private bool isAttached = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isAttached) return;

        if (other.CompareTag("DogNeck") && accessoryType == AccessoryType.Leash)
        {
            AttachToTarget(other.transform);
        }
        else if (other.CompareTag("DogMouth") && accessoryType == AccessoryType.Muzzle)
        {
            AttachToTarget(other.transform);
        }
    }

    private void AttachToTarget(Transform target)
    {
        // �θ� �������� ���̳� �Կ� ����
        transform.SetParent(target);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Collider, Rigidbody ��Ȱ��ȭ
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        isAttached = true;

        Debug.Log($"[AccessoryAttachHandler] {accessoryType} ���� �Ϸ�");
    }
}
