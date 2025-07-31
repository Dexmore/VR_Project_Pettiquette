using UnityEngine;

public class snack : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 "Bowl" 태그를 가진 경우
        if (collision.gameObject.CompareTag("Bowl"))
        {
            // Bowl 오브젝트의 자식 중 첫 번째 비활성화된 오브젝트를 찾아 활성화
            Transform bowl = collision.transform;
            foreach (Transform child in bowl)
            {
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                    Debug.Log($"[snack] '{child.name}' 활성화됨 (Bowl 자식)");
                    break;
                }
            }
        }
    }
}
