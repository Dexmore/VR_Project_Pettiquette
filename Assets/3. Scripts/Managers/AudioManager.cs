using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class AudioManager : MonoBehaviour
{
    [Header("�� Scene�� �´� BGM ����")]
    public AudioSource bgmOutput;
    public List<SceneBgmData> bgmDatas;

    [Header("ȿ���� ����")]
    public AudioSource sfxOutput;
    public List<SFXData> sfxDatas;

    public static AudioManager Instance; // �̱��� Ȱ��ȭ => Instance�� ���� �ٸ� ��ũ��Ʈ�� ���� ����

    private string currentSceneName = ""; // ���� Scene �̸� ����

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
        LoadSFXVolume(); // ���� �����Ҷ� �ҷ�����
        SceneManager.sceneLoaded += SceneLoaded; // Scene�� �ε��� ������ �� Scene�� �´� BGM�� ���� �� �ֵ��� ����
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayBGM(SceneManager.GetActiveScene().name); // ���� �����ϴ� ���� Scene �̸����� ���� ex) Lobby Scene���� �����ϴ� ��� �� Scene�� BGM�� ���
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded; // �̺�Ʈ ����
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGM(scene.name);
    }

    private void PlayBGM(string sceneName)
    {
        if (currentSceneName == sceneName) return;

        currentSceneName = sceneName; // ���� Scene ������Ʈ

        foreach (var data in bgmDatas)
        {
            if (data.sceneName == sceneName)
            {
                bgmOutput.clip = data.bgmClip;
                bgmOutput.Play();
                bgmOutput.loop = true; // BGM �ݺ� ���
                return;
            }
        }

        bgmOutput.Stop(); // ���� �ش� Scene�� �´� AudioClip�� ������ ����
    }

    public void PlaySFX(SFXCategory category)// Audio Source�� ��� �̰Ÿ� �ϼ���!!!!!
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
    public void SaveSFXVolume() // JSON ���·� ������ ����
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

    public void LoadSFXVolume() // JSON ���·� ����� ������ �ҷ�����
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

        CreateDefaultVolume(); // ������ ������ �̰� ������
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
