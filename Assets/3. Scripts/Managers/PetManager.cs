using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetManager : MonoBehaviour
{
    [Header("Dog Prefabs")]
    public GameObject welshCorgiPrefab;
    public GameObject shibaInuPrefab;
    public GameObject dalmaTianPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    // 선택 키(예: "welsh", "shiba", "dalma") 저장 키
    private const string KEY_SELECTED_PET = "selected_pet";
    // 고유 petId 저장 키(처음 만들면 생성해서 PlayerPrefs에 보관)
    private const string KEY_SELECTED_PET_ID = "selected_pet_id";

    void Start()
    {
        // 1) 어떤 품종인지
        string selectedPet = PlayerPrefs.GetString(KEY_SELECTED_PET, "");
        GameObject prefabToSpawn = GetPrefabByName(selectedPet);

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[PetManager] 선택된 강아지 프리팹 없음: '{selectedPet}'");
            return;
        }

        // 2) 스폰
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
        GameObject pet = Instantiate(prefabToSpawn, pos, rot);

        // 3) petId 보장: 비어있으면 PlayerPrefs에서 읽고, 없으면 새로 만들어 저장
        var logic = pet.GetComponentInChildren<AnimalLogic>();
        if (logic == null)
        {
            Debug.LogError("[PetManager] AnimalLogic을 찾을 수 없습니다. 프리팹에 AnimalLogic이 붙어있는지 확인하세요.");
            return;
        }

        // (A) 우선 프리팹에 지정된 petId가 있으면 그걸 사용
        string petId = logic.petId;

        // (B) 없다면 PlayerPrefs에서 읽음
        if (string.IsNullOrEmpty(petId))
            petId = PlayerPrefs.GetString(KEY_SELECTED_PET_ID, "");

        // (C) 그래도 없다면 새로 생성해서 PlayerPrefs에 저장 (간단히 품종명 기반)
        if (string.IsNullOrEmpty(petId))
        {
            // 같은 품종 여러 마리를 쓸 계획이 없으면 품종명만으로도 충분.
            // 여러 마리를 구분하려면 뒤에 GUID/시간 등을 붙여 고유하게 만드세요.
            petId = !string.IsNullOrEmpty(selectedPet) ? selectedPet : pet.name;

            PlayerPrefs.SetString(KEY_SELECTED_PET_ID, petId);
            PlayerPrefs.Save();

            Debug.Log($"[PetManager] petId가 없어 '{petId}' 로 생성 & 저장했습니다.");
        }

        // 최종 petId를 AnimalLogic에 주입
        logic.petId = petId;

        // 4) 현재 친밀도 읽어서 로그
        if (PetAffinityManager.Instance != null)
        {
            float now = PetAffinityManager.Instance.GetAffinity(petId);
            Debug.Log($"[Affinity] 스폰됨 → petId='{petId}', 현재 친밀도={now:F1}");
        }
        else
        {
            Debug.LogWarning("[PetManager] PetAffinityManager.Instance 없음. 친밀도 로깅 생략.");
        }
    }

    private GameObject GetPrefabByName(string name)
    {
        switch (name)
        {
            case "welsh": return welshCorgiPrefab;
            case "shiba": return shibaInuPrefab;
            case "dalma": return dalmaTianPrefab;
            default: return null;
        }
    }
}
