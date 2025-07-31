using UnityEngine;

public class snack : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� "Bowl" �±׸� ���� ���
        if (collision.gameObject.CompareTag("Bowl"))
        {
            // Bowl ������Ʈ�� �ڽ� �� ù ��° ��Ȱ��ȭ�� ������Ʈ�� ã�� Ȱ��ȭ
            Transform bowl = collision.transform;
            foreach (Transform child in bowl)
            {
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                    Debug.Log($"[snack] '{child.name}' Ȱ��ȭ�� (Bowl �ڽ�)");
                    break;
                }
            }
        }
    }
}
