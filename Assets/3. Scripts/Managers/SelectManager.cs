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
        // 친밀도 불러오기
        float shibaAffinity = PlayerPrefs.GetFloat("shiba_affinity", 0f);
        float dalmaAffinity = PlayerPrefs.GetFloat("dalma_affinity", 0f);

        // 해금 기준
        float shibaUnlock = 50f;
        float dalmaUnlock = 100f;

        // 버튼 활성화
        welshSelectButton.interactable = true;
        shibaSelectButton.interactable = shibaAffinity >= shibaUnlock;
        dalmaSelectButton.interactable = dalmaAffinity >= dalmaUnlock;

        // 텍스트 표시
        shibaConditionText.text = shibaAffinity >= shibaUnlock
            ? string.Empty
            : $"시바이누 해금 조건 => 웰시코기 친밀도 {shibaUnlock} 이상";

        dalmaConditionText.text = dalmaAffinity >= dalmaUnlock
            ? string.Empty
            : $"달마시안 해금 조건 => 시바이누 친밀도 {dalmaUnlock} 이상";

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