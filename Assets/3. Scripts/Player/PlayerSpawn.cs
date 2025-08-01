using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    public string spawnPointName = "PlayerSpawnPoint";
    public string targetSceneName = "MyRoomScene"; // �̵� ��ġ�� ������ �� �̸�

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);  // �� ��ȯ���� ����
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
        // 1������ ���: �� ������Ʈ�� ���� ������ Ȱ��ȭ���� �ʾ��� �� ����
        yield return null;

        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;

            Debug.Log($"[PlayerSpawn] ��ġ �̵� �Ϸ� �� {spawnPointName} : {spawnPoint.transform.position}");
        }
        else
        {
            Debug.LogWarning($"[PlayerSpawn] '{spawnPointName}'�� ã�� �� �����ϴ�. (��: {SceneManager.GetActiveScene().name})");
        }
    }
}
