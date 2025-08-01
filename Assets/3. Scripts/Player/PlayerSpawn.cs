using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    public string spawnPointName = "PlayerSpawnPoint";
    public string targetSceneName = "MyRoomScene"; // 이동 위치를 강제할 씬 이름

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
        if (scene.name == targetSceneName)
        {
            StartCoroutine(SetSpawnPointDelayed());
        }
    }

    private IEnumerator SetSpawnPointDelayed()
    {
        // 1프레임 대기: 씬 오브젝트가 아직 완전히 활성화되지 않았을 수 있음
        yield return null;

        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;

            Debug.Log($"[PlayerSpawn] 위치 이동 완료 → {spawnPointName} : {spawnPoint.transform.position}");
        }
        else
        {
            Debug.LogWarning($"[PlayerSpawn] '{spawnPointName}'를 찾을 수 없습니다. (씬: {SceneManager.GetActiveScene().name})");
        }
    }
}
