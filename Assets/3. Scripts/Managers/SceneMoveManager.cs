using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneMoveManager : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference aButton;

    [Header("Target Scene Name")]
    public string sceneToLoad; // 인스펙터에서 씬 이름 입력

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
            Debug.Log($"A 버튼 눌림 → 씬 전환: {sceneToLoad}");
            InventoryManager.Instance?.ClearInventory();
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("sceneToLoad가 비어 있습니다!");
        }
    }
}
