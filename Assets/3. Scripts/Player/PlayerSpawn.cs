using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    public string spawnPointName = "PlayerSpawnPoint";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);  // 씬 전환에도 유지
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetSpawnPoint();
    }

    private void SetSpawnPoint()
    {
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }
        else
        {
            Debug.LogWarning($"Spawn point '{spawnPointName}' not found in scene.");
        }
    }
}
