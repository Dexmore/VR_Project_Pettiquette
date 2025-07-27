using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("Player UI Datas")]
    public GameObject MenuCanvas;

    [Header("Player Audio Datas")]
    public GameObject AudioManager;

    [Header("Player Inventory Datas")]
    public GameObject InventoryManagerPrefab;

    private GameObject menuInstance, audioInstance, inventoryInstance;

    private void Start()
    {
        // 메뉴 생성
        if (MenuCanvas != null)
        {
            menuInstance = Instantiate(MenuCanvas);
            menuInstance.SetActive(false);
        }

        // 오디오 매니저 생성
        if (AudioManager != null)
        {
            audioInstance = Instantiate(AudioManager);
        }

        // 인벤토리 매니저 생성 (싱글톤 검사로 중복 방지됨)
        if (InventoryManager.Instance == null && InventoryManagerPrefab != null)
        {
            inventoryInstance = Instantiate(InventoryManagerPrefab);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (menuInstance != null)
            {
                if (menuInstance.activeSelf)
                {
                    Debug.Log("Menu is already active. Ignoring Backspace.");
                    return;
                }

                menuInstance.SetActive(true);
                UIManager.Instance?.OpenMenu();
                SetupCanvasCamera();
            }
        }
    }

    private void LateUpdate()
    {
        if (menuInstance != null && menuInstance.activeSelf)
        {
            FollowCamera(menuInstance.transform);
        }
    }

    private void SetupCanvasCamera()
    {
        var canvas = menuInstance.GetComponent<Canvas>();
        if (canvas != null && canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    private void FollowCamera(Transform uiTransform)
    {
        Camera cam = Camera.main;
        if (cam == null || uiTransform == null)
            return;

        float distanceFromCamera = 1f;
        Vector3 targetPosition = cam.transform.position + cam.transform.forward * distanceFromCamera;

        // 벽 Raycast 충돌 감지
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, distanceFromCamera + 0.1f))
        {
            // 벽에 가까우면 UI를 벽 앞에 위치시킴
            targetPosition = hit.point - cam.transform.forward * 0.1f;
        }

        // 바닥 Raycast 충돌 감지 (아래로)
        if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit groundHit, 1.5f))
        {
            float minHeight = groundHit.point.y + 0.05f; // 살짝 띄우기
            if (targetPosition.y < minHeight)
                targetPosition.y = minHeight;
        }

        uiTransform.position = targetPosition;
        uiTransform.rotation = Quaternion.LookRotation(cam.transform.forward, Vector3.up);
    }

}
