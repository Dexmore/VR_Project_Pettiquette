using UnityEngine;
using UnityEngine.UI; // UI 표시용, 필요 시 제거 가능

public class LeashControllerManager : MonoBehaviour
{
    private AnimalLogic dog;

    private bool isLeashed = false;
    private bool isMuzzled = false;

    private GameObject Muzzle;
    void Start()
    {
        dog = FindObjectOfType<AnimalLogic>();
        if (dog == null)
        {
            Debug.LogWarning("개 없음!");
        }

        Muzzle = FindChildByName(dog.transform, "Muzzle_Object");
        if (Muzzle == null)
        {
            Debug.LogWarning("Muzzle_Object를 찾지 못했습니다.");
        }
        else
        {
            Muzzle.SetActive(false); // 초기 비활성화
        }
    }

    public void ToggleLeash()
    {
        if (dog == null) return;

        isLeashed = !isLeashed;
        dog.SetLeashed(isLeashed);

        Debug.Log($"[LeashControllerManager] 줄 상태: {(isLeashed ? "연결됨" : "해제됨")}");

        if (Muzzle == null) return;

        isMuzzled = !isMuzzled;
        Muzzle.SetActive(isMuzzled);

        Debug.Log($"[LeashControllerManager] 입마개 상태: {(isMuzzled ? "착용됨" : "해제됨")}");
    }

    private GameObject FindChildByName(Transform parent, string targetName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == targetName)
                return child.gameObject;
        }
        return null;
    }
}
