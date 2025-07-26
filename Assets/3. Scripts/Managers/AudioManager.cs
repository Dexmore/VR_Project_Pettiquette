using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // 싱글톤 활성화 => Instance를 통해 다른 스크립트에 접근 가능

    [Header("각 Scene에 맞는 BGM 설정")]
    public AudioSource bgmOutput;
    public List<SceneBgmData> bgmDatas;

    [Header("효과음 설정")]
    public List<SFXData> sfxDatas;

    [Header("SFX 오브젝트 풀링")]
    public GameObject sfxPlayerPrefab;
    public int sfxPoolSize = 10;

    private Queue<SFXPlayer> sfxPool = new Queue<SFXPlayer>();

    private string currentSceneName = ""; // 현재 Scene 이름 저장
    private Dictionary<SFXCategory, float> sfxVolumeDatas = new Dictionary<SFXCategory, float>();

    private string volumeDataFilePath;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 싱글톤 설정

            LoadSFXVolume(); // 게임 시작할때 불러오기
            volumeDataFilePath = Path.Combine(Application.persistentDataPath, "VolumeData.json"); // Sound Volume 경로 설정
            InitPool();
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += SceneLoaded; // Scene을 로드할 때마다 각 Scene에 맞는 BGM이 나올 수 있도록 설정
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

    private void InitPool()
    {
        for(int i = 0; i < sfxPoolSize; i++)
        {
            GameObject obj = Instantiate(sfxPlayerPrefab);
            SFXPlayer player = obj.GetComponent<SFXPlayer>();
            player.Init(this);
            obj.SetActive(false); // 초기에 비활성화, 이후 필요할때마다 활성화 후 끝나면 반환
            sfxPool.Enqueue(player); // 추가
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

    public void PlaySFX(SFXCategory category, Vector3 pos)// Audio Source를 들고 이거를 하세요!!!!!
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
