using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Affinity Display")]
    public GameObject affinityTextObject;
    public TextMeshProUGUI affinityText;

    private string selectedPetId;
    private float currentAffinity = -1f;
    private string currentSceneName = "";

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
        InventoryButton.onClick.AddListener(() =>
        {
            SwitchToUI(inventoryPrefab);
            PlayUISFX();
        });

        SettingButton.onClick.AddListener(() =>
        {
            SwitchToUI(settingPrefab);
            PlayUISFX();
        });

        HelpButton.onClick.AddListener(() =>
        {
            SwitchToUI(helpUIPrefab);
            PlayUISFX();
        });

        SaveButton.onClick.AddListener(() =>
        {
            Save();
            PlayUISFX();
        });

        BackButton.onClick.AddListener(() =>
        {
            CloseMenu();
            PlayUISFX();
        });

        selectedPetId = PlayerPrefs.GetString("selected_pet", "welsh");

        SceneManager.sceneLoaded += OnSceneLoaded;
        currentSceneName = SceneManager.GetActiveScene().name;

        UpdateAffinityText();
    }

    private void Update()
    {
        if (currentSceneName != "3. Walk_Scene") return;

        if (affinityTextObject == null || affinityText == null) return;

        float newAffinity = PetAffinityManager.Instance.GetAffinity(selectedPetId);
        if (Mathf.Abs(newAffinity - currentAffinity) > 0.01f)
        {
            currentAffinity = newAffinity;
            UpdateAffinityText();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;

        if (affinityTextObject != null)
        {
            affinityTextObject.SetActive(scene.name == "3. Walk_Scene");
        }
    }

    private void UpdateAffinityText()
    {
        if (affinityText != null)
            affinityText.text = $"현재 친밀도 : {currentAffinity:F1}";
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
        PlayUISFX();
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
        SaveManager.Instance?.SaveGame();
    }

    private void PlayUISFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SFXCategory.UI, Vector3.zero);
        }
    }
}