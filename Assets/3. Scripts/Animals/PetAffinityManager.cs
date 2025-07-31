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

    // 경로 구성
    private string dirPath;   // .../SaveData
    private string filePath;  // .../SaveData/pet_affinity.json

    private AllPetData currentData;

    private void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ▶ 저장 경로: persistentDataPath 사용 (빌드 시 안전)
        //   에디터에서도 정상 동작합니다.
        dirPath = Path.Combine(Application.persistentDataPath, "SaveData");
        filePath = Path.Combine(dirPath, "pet_affinity.json");

        // 폴더 보장
        EnsureDirectory();

        // 로드 (최초 실행이면 기본 파일 생성)
        LoadAffinity();
    }

    private void EnsureDirectory()
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
#if UNITY_EDITOR
            Debug.Log($"SaveData 폴더 생성: {dirPath}");
#endif
        }
    }

    public void UpdateAffinity(string petId, float amount)
    {
        if (currentData == null) currentData = new AllPetData();

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
    }

    public float GetAffinity(string petId)
    {
        if (currentData == null) return 0f;
        var pet = currentData.pets.Find(p => p.petId == petId);
        return pet != null ? pet.affinity : 0f;
    }

    public void SaveAffinity()
    {
        try
        {
            EnsureDirectory(); // 혹시 모를 폴더 삭제 대비

            if (currentData == null) currentData = new AllPetData();

            string json = JsonUtility.ToJson(currentData, true);
            File.WriteAllText(filePath, json);
#if UNITY_EDITOR
            Debug.Log($"친밀도 저장 완료\n→ {filePath}");
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 친밀도 저장 실패: {e.Message}\nPath: {filePath}");
        }
    }

    public void LoadAffinity()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                // JsonUtility는 빈 문자열/잘못된 포맷이면 null을 반환할 수 있음
                var data = JsonUtility.FromJson<AllPetData>(json);
                if (data == null)
                {
                    // 파일이 있지만 파싱 실패 → 초기화 후 덮어쓰기
                    currentData = new AllPetData();
                    SaveAffinity();
#if UNITY_EDITOR
                    Debug.LogWarning("⚠ 저장 파일 파싱 실패 → 새 데이터로 재생성");
#endif
                }
                else
                {
                    currentData = data;
#if UNITY_EDITOR
                    Debug.Log("🟢 친밀도 불러오기 완료");
#endif
                }
            }
            else
            {
                // 최초 실행: 기본 데이터 생성 후 파일도 바로 만들어 둠
                currentData = new AllPetData();
                SaveAffinity();
#if UNITY_EDITOR
                Debug.LogWarning("⚠ 저장된 친밀도 데이터가 없어 새 파일을 생성했습니다.");
#endif
            }
        }
        catch (System.Exception e)
        {
            // 로드 중 문제 → 깨끗한 데이터로 재시작
            currentData = new AllPetData();
            Debug.LogError($"❌ 친밀도 불러오기 실패: {e.Message}\n→ 새 데이터로 초기화 후 저장");
            SaveAffinity();
        }
    }

    private void OnApplicationQuit()
    {
        SaveAffinity(); // 종료 시 자동 저장
    }

    private void OnDisable()
    {
        // 에디터에서 Play 중지 시 OnApplicationQuit가 안 불릴 수 있으니 백업
#if UNITY_EDITOR
        SaveAffinity();
#endif
    }

    // ▼ 선택 API들
    public AllPetData GetCurrentData() => currentData;

    public void SetFromData(AllPetData data)
    {
        currentData = data ?? new AllPetData();
        SaveAffinity();
    }

    // 변화 적용 + 즉시 저장
    public void ChangeAffinityAndSave(string petId, float amount)
    {
        UpdateAffinity(petId, amount);
        SaveAffinity();
        Debug.Log($"[Affinity] Changed & Saved: {petId} => {GetAffinity(petId):F1}");
    }
}