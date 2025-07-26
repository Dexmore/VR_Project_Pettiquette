using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // �̱��� Ȱ��ȭ => Instance�� ���� �ٸ� ��ũ��Ʈ�� ���� ����

    [Header("�� Scene�� �´� BGM ����")]
    public AudioSource bgmOutput;
    public List<SceneBgmData> bgmDatas;

    [Header("ȿ���� ����")]
    public List<SFXData> sfxDatas;

    [Header("SFX ������Ʈ Ǯ��")]
    public GameObject sfxPlayerPrefab;
    public int sfxPoolSize = 10;

    private Queue<SFXPlayer> sfxPool = new Queue<SFXPlayer>();

    private string currentSceneName = ""; // ���� Scene �̸� ����
    private Dictionary<SFXCategory, float> sfxVolumeDatas = new Dictionary<SFXCategory, float>();

    private string volumeDataFilePath;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �̱��� ����

            LoadSFXVolume(); // ���� �����Ҷ� �ҷ�����
            volumeDataFilePath = Path.Combine(Application.persistentDataPath, "VolumeData.json"); // Sound Volume ��� ����
            InitPool();
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += SceneLoaded; // Scene�� �ε��� ������ �� Scene�� �´� BGM�� ���� �� �ֵ��� ����
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

    private void InitPool()
    {
        for(int i = 0; i < sfxPoolSize; i++)
        {
            GameObject obj = Instantiate(sfxPlayerPrefab);
            SFXPlayer player = obj.GetComponent<SFXPlayer>();
            player.Init(this);
            obj.SetActive(false); // �ʱ⿡ ��Ȱ��ȭ, ���� �ʿ��Ҷ����� Ȱ��ȭ �� ������ ��ȯ
            sfxPool.Enqueue(player); // �߰�
        }
    }

    public void ReturnSFXPlayer(SFXPlayer player)
    {
        player.gameObject.SetActive(false);
        sfxPool.Enqueue(player);
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

    public void PlaySFX(SFXCategory category, Vector3 pos)// Audio Source�� ��� �̰Ÿ� �ϼ���!!!!!
    {
        SFXData matchData = sfxDatas.Find(data => data.category == category);

        if (matchData != null && matchData.sfxClip != null)
        {
            float volume = GetSFXVolume(category);

            if (sfxPool.Count > 0)
            {
                var player = sfxPool.Dequeue();
                player.gameObject.SetActive(true);
                player.Play(matchData.sfxClip, volume, pos);
            }
            else
            {
                Debug.Log("[AudioManager] SFXPlayer in pool is empty");
            }
        }
        else
        {
            Debug.Log($"AudioManager] SFX '{category}' not found");
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
