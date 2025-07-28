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
        // 메뉴 UI 생성
        if (MenuCanvas != null)
        {
            menuInstance = Instantiate(MenuCanvas);
            menuInstance.SetActive(false);
            SetupCanvasCamera();
        }

        // 오디오 매니저
        if (AudioManager != null)
            audioInstance = Instantiate(AudioManager);

        // 인벤토리 매니저
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

        float distanceFromCamera = 1f;   // 카메라에서 거리

        Vector3 targetPos = cam.position + cam.forward * distanceFromCamera;
        uiTransform.position = targetPos;
        uiTransform.rotation = Quaternion.LookRotation(cam.forward, Vector3.up);
    }
}