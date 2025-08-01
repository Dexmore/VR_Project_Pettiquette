using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.XR.CoreUtils;

public class PlayerSpawn : MonoBehaviour
{
    public string spawnPointName = "PlayerSpawnPoint";
    public float cameraHeightOffset = 1.36144f; // 시야 높이

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint == null)
        {
            Debug.LogError($"[SpawnManager] SpawnPoint '{spawnPointName}'를 찾을 수 없습니다.");
            yield break;
        }

        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("[SpawnManager] XR Origin을 찾을 수 없습니다.");
            yield break;
        }

        Vector3 adjustedPos = spawnPoint.transform.position + new Vector3(0f, cameraHeightOffset, 0f);
        xrOrigin.MoveCameraToWorldLocation(adjustedPos);
        xrOrigin.transform.rotation = spawnPoint.transform.rotation;

        Debug.Log($"[SpawnManager] XR Origin 시야 높이 보정 후 이동 완료 → {adjustedPos}");
    }
}