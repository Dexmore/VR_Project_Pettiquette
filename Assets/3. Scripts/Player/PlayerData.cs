using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerData : MonoBehaviour
{
    [Header("Player UI Datas")]
    public GameObject MenuCanvas;

    [Header("Player Audio Datas")]
    public GameObject AudioManager;

    [Header("Player Inventory Datas")]
    public GameObject InventoryManagerPrefab;

    [Header("Input")]
    public InputActionReference menuToggleAction;

    [Header("XR Interactor (Ray)")]
    public XRRayInteractor xrRayInteractor; // 인스펙터에 할당 필요

    private GameObject menuInstance, audioInstance, inventoryInstance;
    private Transform cam;

    private void Awake()
    {
        cam = Camera.main?.transform;
    }

    private void OnEnable()
    {
        if (menuToggleAction != null)
        {
            menuToggleAction.action.performed += OnMenuToggle;
            menuToggleAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (menuToggleAction != null)
        {
            menuToggleAction.action.performed -= OnMenuToggle;
            menuToggleAction.action.Disable();
        }
    }

    private void Start()
    {
        // 1. Menu UI 생성 및 초기화
        if (MenuCanvas != null)
        {
            menuInstance = Instantiate(MenuCanvas);
            menuInstance.SetActive(false);

            // [1] UIOnly 레이어 지정
            int uiLayer = LayerMask.NameToLayer("UIOnly");
            menuInstance.layer = uiLayer;
            foreach (Transform child in menuInstance.GetComponentsInChildren<Transform>(true))
                child.gameObject.layer = uiLayer;

            // [2] Collider 제거
            foreach (Collider col in menuInstance.GetComponentsInChildren<Collider>(true))
                Destroy(col);

            // [3] 카메라 설정
            SetupCanvasCamera();
        }

        // 2. XR Ray에서 UIOnly 레이어 무시
        if (xrRayInteractor != null)
        {
            xrRayInteractor.raycastMask &= ~(1 << LayerMask.NameToLayer("UIOnly"));
        }

        // 3. 오디오 매니저
        if (AudioManager != null)
            audioInstance = Instantiate(AudioManager);

        // 4. 인벤토리 매니저
        if (InventoryManager.Instance == null && InventoryManagerPrefab != null)
            inventoryInstance = Instantiate(InventoryManagerPrefab);
    }

    private void OnMenuToggle(InputAction.CallbackContext ctx)
    {
        if (menuInstance == null) return;

        bool isActive = menuInstance.activeSelf;
        menuInstance.SetActive(!isActive);

        if (!isActive)
        {
            SetupCanvasCamera();
            UIManager.Instance?.OpenMenu();
        }
        else
        {
            UIManager.Instance?.CloseMenu();
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
            canvas.worldCamera = Camera.main;

        if (cam == null)
            cam = Camera.main?.transform;
    }

    private void FollowCamera(Transform uiTransform)
    {
        if (cam == null || uiTransform == null) return;

        float forwardDistance = 3.0f;         // 카메라 전방 위치
        float verticalOffset = 0.6f;          // 위쪽 오프셋
        float minHeightAboveGround = 1.2f;    // 최소 바닥 높이

        Vector3 forward = cam.forward;
        Vector3 up = Vector3.up;
        Vector3 targetPosition = cam.position + forward.normalized * forwardDistance + up * verticalOffset;

        // 벽 충돌
        if (Physics.Raycast(cam.position, forward, out RaycastHit wallHit, forwardDistance + 0.2f, LayerMask.GetMask("Default")))
        {
            targetPosition = wallHit.point - forward.normalized * 0.2f;
        }

        // 바닥 충돌
        if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit groundHit, 5f, LayerMask.GetMask("Default")))
        {
            float groundY = groundHit.point.y;
            float minY = groundY + minHeightAboveGround;
            if (targetPosition.y < minY)
                targetPosition.y = minY;
        }

        // 최종 위치 및 회전 적용
        uiTransform.position = targetPosition;
        uiTransform.rotation = Quaternion.LookRotation(cam.forward, Vector3.up);
    }
}
