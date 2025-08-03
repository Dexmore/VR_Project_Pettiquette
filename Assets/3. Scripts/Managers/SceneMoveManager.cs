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
        string targetScene = sceneToLoad;

        // 튜토리얼 스킵 체크
        if (sceneToLoad == "TutorialScene" && SaveManager.Instance != null)
        {
            if (SaveManager.Instance.IsTutorialCleared())
            {
                targetScene = "2. My_Room_Scene"; // 튜토리얼 클리어했으면 방 씬으로 이동
                Debug.Log("튜토리얼 클리어 상태 → 방으로 이동");
            }
            else
            {
                Debug.Log("튜토리얼 미클리어 → 튜토리얼 진행");
            }
        }

        if (!string.IsNullOrEmpty(targetScene))
        {
            Debug.Log($"A 버튼 눌림 → 씬 전환: {targetScene}");
            InventoryManager.Instance?.ClearInventory();
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("sceneToLoad가 비어 있습니다!");
        }
    }

}
