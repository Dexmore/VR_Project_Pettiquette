using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class AudioManager : MonoBehaviour
{
    [Header("각 Scene에 맞는 BGM 설정")]
    public AudioSource bgmOutput;
    public List<SceneBgmData> bgmDatas;

    [Header("효과음 설정")]
    public AudioSource sfxOutput;
    public List<SFXData> sfxDatas;

    public static AudioManager Instance; // 싱글톤 활성화 => Instance를 통해 다른 스크립트에 접근 가능

    private string currentSceneName = ""; // 현재 Scene 이름 저장

    private string volumeDataFilePath = Path.Combine(Application.persistentDataPath, "SFXVolumeData.json");

    private Dictionary<SFXCategory, float> sfxVolumeDatas = new Dictionary<SFXCategory, float>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        LoadSFXVolume(); // 게임 시작할때 불러오기
        SceneManager.sceneLoaded += SceneLoaded; // Scene을 로드할 때마다 각 Scene에 맞는 BGM이 나올 수 있도록 설정
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayBGM(SceneManager.GetActiveScene().name); // 게임 시작하는 곳의 Scene 이름으로 시작 ex) Lobby Scene에서 시작하는 경우 이 Scene의 BGM이 재생
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded; // 이벤트 제거
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGM(scene.name);
    }

    private void PlayBGM(string sceneName)
    {
        if (currentSceneName == sceneName) return;

        currentSceneName = sceneName; // 현재 Scene 업데이트

        foreach (var data in bgmDatas)
        {
            if (data.sceneName == sceneName)
            {
                bgmOutput.clip = data.bgmClip;
                bgmOutput.Play();
                bgmOutput.loop = true; // BGM 반복 재생
                return;
            }
        }

        bgmOutput.Stop(); // 만약 해당 Scene과 맞는 AudioClip이 없으면 정지
    }

    public void PlaySFX(SFXCategory category)// Audio Source를 들고 이거를 하세요!!!!!
    {
        SFXData matchData = sfxDatas.Find(data => data.sfxName == category.ToString());

        if(matchData != null && matchData.sfxClip != null)
        {
            float volume = sfxVolumeDatas.ContainsKey(category) ? sfxVolumeDatas[category] : 1f;
            sfxOutput.PlayOneShot(matchData.sfxClip, volume);
        }
    }

    public void SetSFXVolume(SFXCategory category, float volume)
    {
        sfxVolumeDatas[category] = volume;
        SaveSFXVolume();
    }

    public float GetSFXVolume(SFXCategory category)
    {
        return sfxVolumeDatas.ContainsKey(category) ? sfxVolumeDatas[category] : 1f;
    }
    public void SaveSFXVolume() // JSON 형태로 저장할 예정
    {
        SFXVolumeData data = new SFXVolumeData();

        foreach(var category in sfxVolumeDatas)
        {
            data.categoryVolumes.Add(new CategoryVolumeData
            {
                categoryName = category.Key.ToString(),
                categoryVolume = category.Value
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(volumeDataFilePath, json);
    }

    public void LoadSFXVolume() // JSON 형태로 저장된 데이터 불러오기
    {
        sfxVolumeDatas.Clear();

        if (!File.Exists(volumeDataFilePath))
        {
            Debug.Log("SFX volume file Not Found, create new one");
            return;
        }

        string json = File.ReadAllText(volumeDataFilePath);

        SFXVolumeData data = JsonUtility.FromJson<SFXVolumeData>(json);

        foreach(var vol in data.categoryVolumes)
        {
            if(Enum.TryParse(vol.categoryName, out SFXCategory category))
            {
                sfxVolumeDatas[category] = vol.categoryVolume;
            }
        }

        CreateDefaultVolume(); // 데이터 없으면 이거 쓰세요
    }

    private void CreateDefaultVolume()
    {
        foreach(SFXCategory category in Enum.GetValues(typeof(SFXCategory)))
        {
            if (!sfxVolumeDatas.ContainsKey(category))
            {
                sfxVolumeDatas[category] = 1f;
            }
        }
    }
}
