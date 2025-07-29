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
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"A ��ư ���� �� �� ��ȯ: {sceneToLoad}");
            InventoryManager.Instance?.ClearInventory();
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("sceneToLoad�� ��� �ֽ��ϴ�!");
        }
    }
}
