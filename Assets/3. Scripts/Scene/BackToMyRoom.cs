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
            // 씬에 있는 AnimalLogic 컴포넌트를 찾음 (비활성화 오브젝트도 포함)
            animal = FindObjectOfType<AnimalLogic>(true);

            if (animal != null)
            {
                Debug.Log("[BackToMyRoom] AnimalLogic 찾음!");
                yield break;
            }

            yield return null; // 한 프레임 대기 후 다시 찾기
        }
    }
    void GoToMyRoom()
    {
        if (!string.IsNullOrEmpty(animal.petId))
            PetAffinityManager.Instance?.ChangeAffinityAndSave(animal.petId, 10f);
        SceneManager.LoadScene("2. My_Room_Scene");
    }
}
