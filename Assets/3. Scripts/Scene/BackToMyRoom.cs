using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BackToMyRoom : MonoBehaviour
{
    public Button button;
    private AnimalLogic animal;

    private void Start()
    {
        StartCoroutine(AnimalLogicFind());
        button.onClick.AddListener(GoToMyRoom);
    }

    private IEnumerator AnimalLogicFind()
    {
        while (animal == null)
        {
            // ���� �ִ� AnimalLogic ������Ʈ�� ã�� (��Ȱ��ȭ ������Ʈ�� ����)
            animal = FindObjectOfType<AnimalLogic>(true);

            if (animal != null)
            {
                Debug.Log("[BackToMyRoom] AnimalLogic ã��!");
                yield break;
            }

            yield return null; // �� ������ ��� �� �ٽ� ã��
        }
    }
    void GoToMyRoom()
    {
        if (!string.IsNullOrEmpty(animal.petId))
            PetAffinityManager.Instance?.ChangeAffinityAndSave(animal.petId, 10f);
        SceneManager.LoadScene("2. My_Room_Scene");
    }
}
