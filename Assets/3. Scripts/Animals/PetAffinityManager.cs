using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PetAffinityData
{
    public string petId;
    public float affinity;
}

[System.Serializable]
public class AllPetData
{
    public List<PetAffinityData> pets = new List<PetAffinityData>();
}

public class PetAffinityManager : MonoBehaviour
{
    public static PetAffinityManager Instance;

    private string dirPath;
    private string filePath;
    private AllPetData currentData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            dirPath = Path.Combine(Application.persistentDataPath, "SaveData");
            filePath = Path.Combine(dirPath, "pet_affinity.json");
            EnsureDirectory();
            LoadAffinity();
        }
        else
        {
            // ✅ 복제본은 즉시 파괴 (그리고 아래 OnDisable에서 저장하지 않게 가드도 걸어둠)
            Destroy(gameObject);
            return;
        }


    }

    private bool IsPrimary() => ReferenceEquals(this, Instance);

    private void EnsureDirectory()
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
#if UNITY_EDITOR
            Debug.Log($"[Affinity] SaveData 폴더 생성: {dirPath}");
#endif
        }
    }

    public void UpdateAffinity(string petId, float amount)
    {
        if (currentData == null) currentData = new AllPetData();
        if (string.IsNullOrEmpty(petId))
        {
            Debug.LogWarning("[Affinity] petId가 비어 있어서 업데이트 건너뜀");
            return;
        }

        var pet = currentData.pets.Find(p => p.petId == petId);
        if (pet != null)
        {
            pet.affinity = Mathf.Clamp(pet.affinity + amount, 0f, 100f);
        }
        else
        {
            currentData.pets.Add(new PetAffinityData
            {
                petId = petId,
                affinity = Mathf.Clamp(amount, 0f, 100f)
            });
        }

#if UNITY_EDITOR
        Debug.Log($"[Affinity] Update → {petId}: {GetAffinity(petId):F1}");
#endif
    }

    public float GetAffinity(string petId)
    {
        if (currentData == null) return 0f;

        var pet = currentData.pets.Find(p => p.petId == petId);
        if (pet != null)
            return pet.affinity;
        else
        {
            // 없는 PetId는 자동 등록 (친밀도 0)
            var newPet = new PetAffinityData { petId = petId, affinity = 0f };
            currentData.pets.Add(newPet);
            SaveAffinity();  // 즉시 저장
#if UNITY_EDITOR
            Debug.Log($"[Affinity] {petId}가 없어서 새로 추가됨 (Affinity: 0)");
#endif
            return 0f;
        }
    }


    public void SaveAffinity()
    {
        if (!IsPrimary()) return; // ✅ 복제본 저장 차단

        try
        {
            EnsureDirectory();
            if (currentData == null) currentData = new AllPetData();

            string json = JsonUtility.ToJson(currentData, true);
            File.WriteAllText(filePath, json);
#if UNITY_EDITOR
            Debug.Log($"[Affinity] 저장 완료 → {filePath}");
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Affinity] 저장 실패: {e.Message}\nPath: {filePath}");
        }
    }

    public void LoadAffinity()
    {
        if (!IsPrimary()) return; // ✅ 복제본 로드 차단

        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var data = JsonUtility.FromJson<AllPetData>(json);
                currentData = data ?? new AllPetData();

#if UNITY_EDITOR
                Debug.Log($"[Affinity] 불러오기 완료 (개수: {currentData.pets.Count})");
#endif
            }
            else
            {
                currentData = new AllPetData();
                SaveAffinity(); // 초회 생성
#if UNITY_EDITOR
                Debug.LogWarning("[Affinity] 저장 파일 없음 → 새 파일 생성");
#endif
            }
        }
        catch (System.Exception e)
        {
            currentData = new AllPetData();
            Debug.LogError($"[Affinity] 불러오기 실패: {e.Message}\n→ 새 데이터로 초기화 후 저장");
            SaveAffinity();
        }
    }

    private void OnApplicationQuit()
    {
        if (!IsPrimary()) return; // ✅ 복제본 저장 차단
        SaveAffinity();
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        // ✅ 에디터에서 복제본이 파괴될 때 빈 데이터로 덮어쓰지 않도록 방지
        if (!IsPrimary()) return;
        SaveAffinity();
#endif
    }

    // 편의 함수들
    public AllPetData GetCurrentData() => currentData;

    public void SetFromData(AllPetData data)
    {
        if (!IsPrimary()) return;
        currentData = data ?? new AllPetData();
        SaveAffinity();
    }

    public void ChangeAffinityAndSave(string petId, float amount)
    {
        if (!IsPrimary()) return;

        UpdateAffinity(petId, amount);
        SaveAffinity();

#if UNITY_EDITOR
        Debug.Log($"[Affinity] Changed & Saved: {petId} = {GetAffinity(petId):F1}");
#endif
    }

    // 디버그
    [ContextMenu("Debug: Print All Affinities")]
    public void DebugPrintAllAffinities()
    {
        if (!IsPrimary())
        {
            Debug.Log("[Affinity] (복제본) Debug 요청 무시");
            return;
        }

        if (currentData == null || currentData.pets == null || currentData.pets.Count == 0)
        {
            Debug.Log("[Affinity] 데이터 없음");
            return;
        }

        Debug.Log($"[Affinity] 총 {currentData.pets.Count} 마리");
        foreach (var p in currentData.pets)
            Debug.Log($"[Affinity] {p.petId} : {p.affinity:F1}");
    }
}
