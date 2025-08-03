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

    [Header("Affinity Text")]
    public TextMeshProUGUI welshAffinityText;
    public TextMeshProUGUI shibaAffinityText;
    public TextMeshProUGUI dalmaAffinityText;

    private void Start()
    {
        CheckUnlockConditions();
    }

    public void CheckUnlockConditions()
    {
        // JSON에서 친밀도 불러오기
        float welshAffinity = PetAffinityManager.Instance.GetAffinity("welsh");
        float shibaAffinity = PetAffinityManager.Instance.GetAffinity("shiba");
        float dalmaAffinity = PetAffinityManager.Instance.GetAffinity("dalma");

        // 해금 기준
        float shibaUnlock = 50f;
        float dalmaUnlock = 100f;

        // 버튼 활성화
        welshSelectButton.interactable = true;
        shibaSelectButton.interactable = welshAffinity >= shibaUnlock;
        dalmaSelectButton.interactable = shibaAffinity >= dalmaUnlock;

        // 텍스트 표시 (조건 텍스트)
        shibaConditionText.text = welshAffinity >= shibaUnlock
            ? string.Empty
            : $"시바이누 해금 조건 => 웰시코기 친밀도 {shibaUnlock} 이상";

        dalmaConditionText.text = shibaAffinity >= dalmaUnlock
            ? string.Empty
            : $"달마시안 해금 조건 => 시바이누 친밀도 {dalmaUnlock} 이상";

        // ★ 친밀도 현재 값 텍스트
        welshAffinityText.text = $"웰시코기\n현재 친밀도 : {welshAffinity:F1}";
        shibaAffinityText.text = $"시바이누\n현재 친밀도 : {shibaAffinity:F1}";
        dalmaAffinityText.text = $"달마시안\n현재 친밀도 : {dalmaAffinity:F1}";

        // 버튼 클릭 이벤트 (중복 방지)
        welshSelectButton.onClick.RemoveAllListeners();
        shibaSelectButton.onClick.RemoveAllListeners();
        dalmaSelectButton.onClick.RemoveAllListeners();

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
