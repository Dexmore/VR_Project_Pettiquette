using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Parent")]
    public GameObject uiParent;

    [Header("UI Prefabs")]
    public GameObject menuPrefab;
    public GameObject inventoryPrefab;
    public GameObject settingPrefab;
    public GameObject helpUIPrefab;

    [Header("UI Buttons")]
    public Button InventoryButton;
    public Button SettingButton;
    public Button HelpButton;
    public Button SaveButton;
    public Button BackButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InventoryButton.onClick.AddListener(() => SwitchToUI(inventoryPrefab));
        SettingButton.onClick.AddListener(() => SwitchToUI(settingPrefab));
        HelpButton.onClick.AddListener(() => SwitchToUI(helpUIPrefab));
        SaveButton.onClick.AddListener(Save);
        BackButton.onClick.AddListener(CloseMenu);
    }

    public void ToggleMenu()
    {
        if (!uiParent.activeSelf)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        uiParent.SetActive(true);
        menuPrefab.SetActive(true);
        CloseAllSubCanvas();
    }

    public void CloseMenu()
    {
        CloseAllSubCanvas();
        uiParent.SetActive(false);
    }

    public void SwitchToUI(GameObject targetUI)
    {
        // ¸Þ´º´Â ²ô°í, ÇØ´ç UI¸¸ ÄÔ
        menuPrefab.SetActive(false);
        CloseAllSubCanvas();
        targetUI.SetActive(true);
    }

    public void CloseAllSubCanvas()
    {
        inventoryPrefab.SetActive(false);
        settingPrefab.SetActive(false);
        helpUIPrefab.SetActive(false);
    }

    public void Save()
    {
        Debug.Log("Game Saved!");
    }
}
