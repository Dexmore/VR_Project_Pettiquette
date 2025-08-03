using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameProgressData
{
    public bool tutorialCleared = false;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private GameProgressData progressData = new GameProgressData();
    private string progressFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            progressFilePath = System.IO.Path.Combine(Application.persistentDataPath, "progress.json");
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveProgress()
    {
        string json = JsonUtility.ToJson(progressData, true);
        System.IO.File.WriteAllText(progressFilePath, json);
        Debug.Log("[SaveManager] 진행도 저장 완료");
    }

    public void LoadProgress()
    {
        if (System.IO.File.Exists(progressFilePath))
        {
            string json = System.IO.File.ReadAllText(progressFilePath);
            progressData = JsonUtility.FromJson<GameProgressData>(json);
            Debug.Log("[SaveManager] 진행도 로드 완료");
        }
    }

    // 튜토리얼 클리어 시 호출할 함수
    public void SetTutorialCleared()
    {
        progressData.tutorialCleared = true;
        SaveProgress();
    }

    public bool IsTutorialCleared()
    {
        return progressData.tutorialCleared;
    }

    public void SaveGame()
    {
        SaveProgress();
        Debug.Log("[SaveManager] SaveGame 호출됨");
    }
}
