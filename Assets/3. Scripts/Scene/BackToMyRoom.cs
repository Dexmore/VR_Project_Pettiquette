using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMyRoom : MonoBehaviour
{
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(GoToMyRoom);
    }

    void GoToMyRoom()
    {
        SceneManager.LoadScene("2. My_Room_Scene");
    }
}
