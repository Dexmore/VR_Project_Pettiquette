using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetManager : MonoBehaviour
{
    [Header("Dog Prefabs")]
    public GameObject welshCorgiPrefab;
    public GameObject shibaInuPrefab;
    public GameObject dalmaTianPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    void Start()
    {
        string selectedPet = PlayerPrefs.GetString("selected_pet", ""); // 기본값 없음 처리

        GameObject prefabToSpawn = GetPrefabByName(selectedPet);
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning($"[PetSpawner] 선택된 강아지 프리팹 없음: {selectedPet}");
        }
    }

    private GameObject GetPrefabByName(string name)
    {
        switch (name)
        {
            case "welsh": return welshCorgiPrefab;
            case "shiba": return shibaInuPrefab;
            case "dalma": return dalmaTianPrefab;
            default: return null;
        }
    }
}
