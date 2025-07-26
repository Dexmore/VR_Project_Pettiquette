using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Menu Canvas")]
    public GameObject MenuCanvas;

    [Header("UI Prefabs")]
    public GameObject inventoryPrefab;
    public GameObject settingPrefab;
    public GameObject HelpUIPrefab;

    private Dictionary<string, GameObject> uiInstance = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            InitializeUI();
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void InitializeUI()
    {
        RegisterUI("Inventory", inventoryPrefab);
        RegisterUI("Setting", settingPrefab);
        RegisterUI("Help", HelpUIPrefab);
    }

    void RegisterUI(string uiName, GameObject prefab)
    {
        if (prefab == null) return;

        GameObject instance = Instantiate(prefab);
        instance.name = uiName + "Canvas";
        instance.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ �� ���߿� ��ư ���� ����Ͽ� Ȱ��ȭ
        uiInstance[uiName] = instance;
    }

    private void Start()
    {
        SetUpMenuButtons();
    }

    void SetUpMenuButtons()
    {
        if (MenuCanvas == null) return;

        Button Inventory = MenuCanvas.transform.Find("InventoryButton")?.GetComponent<Button>();
        Button Setting = MenuCanvas.transform.Find("SettingButton")?.GetComponent<Button>();
        Button Help = MenuCanvas.transform.Find("HelpButton")?.GetComponent<Button>();
        Button Save = MenuCanvas.transform.Find("SaveButton")?.GetComponent<Button>();
        Button Back = MenuCanvas.transform.Find("BackButton")?.GetComponent<Button>();

        if(Inventory != null)
        {
            Inventory.onClick.AddListener(() => ShowUI("Inventory"));
        }

        if(Setting != null)
        {
            Setting.onClick.AddListener(() => ShowUI("Setting"));
        }

        if(Help != null)
        {
            Help.onClick.AddListener(() => ShowUI("Help"));
        }

        if(Save != null)
        {
            Save.onClick.AddListener(() =>
            {
                // ���� ���� ���⿡ ��������
            });
        }

        if(Back != null)
        {
            Back.onClick.AddListener(() =>
            {
                HideUI("Inventory");
                HideUI("Setting");
                HideUI("Help");

                MenuCanvas.SetActive(false);
            });
        } // �޴� ��Ȱ��ȭ ��ư
    }
    public void ShowUI(string uiName)
    {
        if (uiInstance.ContainsKey(uiName))
        {
            MenuCanvas.SetActive(false);
            uiInstance[uiName].SetActive(true);
        }
    }

    public void HideUI(string uiName)
    {
        if (uiInstance.ContainsKey(uiName))
        {
            uiInstance[uiName].SetActive(false);
        }
    }
    
    public GameObject GetUI(string uiName)
    {
        return uiInstance.ContainsKey(uiName) ? uiInstance[uiName] : null;
    }
}
