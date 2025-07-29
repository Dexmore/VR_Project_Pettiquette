using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectManager : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject selectCanvas;
    public GameObject gotoWalkCanvas;

    [Header("Select Button")]
    public Button welshSelectButton;
    public Button shibaSelectButton;
    public Button dalmaSelectButton;

    [Header("Condition Text")]
    public TextMeshProUGUI shibaConditionText;
    public TextMeshProUGUI dalmaConditionText;

    [Header("Select Animal")]
    public GameObject welshCorgi;
    public GameObject shibaInu;
    public GameObject dalmaTian;

    [Header("Animal SpawnPoint")]
    public Transform animalSpawnPoint;

    private void Start()
    {
        CheckUnlockConditions();
    }

    void CheckUnlockConditions()
    {
        // ģ�е� �ҷ�����
        float shibaAffinity = PlayerPrefs.GetFloat("shiba_affinity", 0f);
        float dalmaAffinity = PlayerPrefs.GetFloat("dalma_affinity", 0f);

        // �ر� ����
        float shibaUnlock = 50f;
        float dalmaUnlock = 100f;

        // ��ư Ȱ��ȭ
        welshSelectButton.interactable = true;
        shibaSelectButton.interactable = shibaAffinity >= shibaUnlock;
        dalmaSelectButton.interactable = dalmaAffinity >= dalmaUnlock;

        // �ؽ�Ʈ ǥ��
        shibaConditionText.text = shibaAffinity >= shibaUnlock
            ? string.Empty
            : $"�ù��̴� �ر� ���� => �����ڱ� ģ�е� {shibaUnlock} �̻�";

        dalmaConditionText.text = dalmaAffinity >= dalmaUnlock
            ? string.Empty
            : $"�޸��þ� �ر� ���� => �ù��̴� ģ�е� {dalmaUnlock} �̻�";

        welshSelectButton.onClick.AddListener(OnSelectWelsh);
        shibaSelectButton.onClick.AddListener(OnSelectShiba);
        dalmaSelectButton.onClick.AddListener(OnSelectDalma);
    }

    public void OnSelectWelsh()
    {
        PlayerPrefs.SetString("selected_pet", "welsh");
        selectCanvas.SetActive(false);
        gotoWalkCanvas.SetActive(true);

        Instantiate(welshCorgi, animalSpawnPoint);
    }

    public void OnSelectShiba()
    {
        PlayerPrefs.SetString("selected_pet", "shiba");
        selectCanvas.SetActive(false);
        gotoWalkCanvas.SetActive(true);

        Instantiate(shibaInu, animalSpawnPoint);
    }

    public void OnSelectDalma()
    {
        PlayerPrefs.SetString("selected_pet", "dalma");
        selectCanvas.SetActive(false);
        gotoWalkCanvas.SetActive(true);

        Instantiate(dalmaTian, animalSpawnPoint);
    }
}