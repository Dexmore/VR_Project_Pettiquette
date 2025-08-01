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

    // ���� Ű(��: "welsh", "shiba", "dalma") ���� Ű
    private const string KEY_SELECTED_PET = "selected_pet";
    // ���� petId ���� Ű(ó�� ����� �����ؼ� PlayerPrefs�� ����)
    private const string KEY_SELECTED_PET_ID = "selected_pet_id";

    void Start()
    {
        // 1) � ǰ������
        string selectedPet = PlayerPrefs.GetString(KEY_SELECTED_PET, "");
        GameObject prefabToSpawn = GetPrefabByName(selectedPet);

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[PetManager] ���õ� ������ ������ ����: '{selectedPet}'");
            return;
        }

        // 2) ����
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
        GameObject pet = Instantiate(prefabToSpawn, pos, rot);

        // 3) petId ����: ��������� PlayerPrefs���� �а�, ������ ���� ����� ����
        var logic = pet.GetComponentInChildren<AnimalLogic>();
        if (logic == null)
        {
            Debug.LogError("[PetManager] AnimalLogic�� ã�� �� �����ϴ�. �����տ� AnimalLogic�� �پ��ִ��� Ȯ���ϼ���.");
            return;
        }

        // (A) �켱 �����տ� ������ petId�� ������ �װ� ���
        string petId = logic.petId;

        // (B) ���ٸ� PlayerPrefs���� ����
        if (string.IsNullOrEmpty(petId))
            petId = PlayerPrefs.GetString(KEY_SELECTED_PET_ID, "");

        // (C) �׷��� ���ٸ� ���� �����ؼ� PlayerPrefs�� ���� (������ ǰ���� ���)
        if (string.IsNullOrEmpty(petId))
        {
            // ���� ǰ�� ���� ������ �� ��ȹ�� ������ ǰ�������ε� ���.
            // ���� ������ �����Ϸ��� �ڿ� GUID/�ð� ���� �ٿ� �����ϰ� ���弼��.
            petId = !string.IsNullOrEmpty(selectedPet) ? selectedPet : pet.name;

            PlayerPrefs.SetString(KEY_SELECTED_PET_ID, petId);
            PlayerPrefs.Save();

            Debug.Log($"[PetManager] petId�� ���� '{petId}' �� ���� & �����߽��ϴ�.");
        }

        // ���� petId�� AnimalLogic�� ����
        logic.petId = petId;

        // 4) ���� ģ�е� �о �α�
        if (PetAffinityManager.Instance != null)
        {
            float now = PetAffinityManager.Instance.GetAffinity(petId);
            Debug.Log($"[Affinity] ������ �� petId='{petId}', ���� ģ�е�={now:F1}");
        }
        else
        {
            Debug.LogWarning("[PetManager] PetAffinityManager.Instance ����. ģ�е� �α� ����.");
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
