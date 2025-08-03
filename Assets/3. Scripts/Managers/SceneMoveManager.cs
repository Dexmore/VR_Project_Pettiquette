using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneMoveManager : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference aButton;

    [Header("Target Scene Name")]
    public string sceneToLoad; // �ν����Ϳ��� �� �̸� �Է�

    private void OnEnable()
    {
        if (aButton != null)
        {
            aButton.action.performed += OnAPressed;
            aButton.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (aButton != null)
        {
            aButton.action.performed -= OnAPressed;
            aButton.action.Disable();
        }
    }

    private void OnAPressed(InputAction.CallbackContext ctx)
    {
        string targetScene = sceneToLoad;

        // Ʃ�丮�� ��ŵ üũ
        if (sceneToLoad == "TutorialScene" && SaveManager.Instance != null)
        {
            if (SaveManager.Instance.IsTutorialCleared())
            {
                targetScene = "2. My_Room_Scene"; // Ʃ�丮�� Ŭ���������� �� ������ �̵�
                Debug.Log("Ʃ�丮�� Ŭ���� ���� �� ������ �̵�");
            }
            else
            {
                Debug.Log("Ʃ�丮�� ��Ŭ���� �� Ʃ�丮�� ����");
            }
        }

        if (!string.IsNullOrEmpty(targetScene))
        {
            Debug.Log($"A ��ư ���� �� �� ��ȯ: {targetScene}");
            InventoryManager.Instance?.ClearInventory();
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("sceneToLoad�� ��� �ֽ��ϴ�!");
        }
    }

}
