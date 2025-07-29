using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CheckItemUImanager : MonoBehaviour
{
    [Header("Images")]
    public Image snackCheckImage;
    public Image shovelCheckImage;
    public Image muzzleCheckImage;
    public Image ballCheckImage;
    public Image collarCheckImage;
    public Image bowlCheckImage;

    [Header("Text")]
    public TextMeshProUGUI needText;

    [Header("Button")]
    public Button SceneMoveButton;
    void Start()
    {
        snackCheckImage.gameObject.SetActive(false);
        shovelCheckImage.gameObject.SetActive(false);
        muzzleCheckImage.gameObject.SetActive(false);
        ballCheckImage.gameObject.SetActive(false);
        collarCheckImage.gameObject.SetActive(false);
        bowlCheckImage.gameObject.SetActive(false);

        needText.text = string.Empty;

        SceneMoveButton.onClick.AddListener(GoToWalkScene);
    }

    public void CheckItem(string itemName)
    {
        switch (itemName)
        {
            case "Food":
                snackCheckImage.gameObject.SetActive(true);
                break;
            case "Shovel":
                shovelCheckImage.gameObject.SetActive(true);
                break;
            case "Muzzle":
                muzzleCheckImage.gameObject.SetActive(true);
                break;
            case "Ball":
                ballCheckImage.gameObject.SetActive(true);
                break;
            case "Collar":
                collarCheckImage.gameObject.SetActive(true);
                break;
            case "Bowl":
                bowlCheckImage.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning($"[CheckItemUImanager] 일치하는 항목 없음: {itemName}");
                break;
        }
    }

    public void GoToWalkScene()
    {
        // 부족한 아이템 리스트
        string missing = "";

        if (!snackCheckImage.gameObject.activeSelf) missing += "=> 간식\n";
        if (!shovelCheckImage.gameObject.activeSelf) missing += "=> 삽\n";
        if (!muzzleCheckImage.gameObject.activeSelf) missing += "=> 입마개\n";
        if (!ballCheckImage.gameObject.activeSelf) missing += "=> 공\n";
        if (!collarCheckImage.gameObject.activeSelf) missing += "=> 목줄\n";
        if (!bowlCheckImage.gameObject.activeSelf) missing += "=> 밥그릇\n";

        if (string.IsNullOrEmpty(missing))
        {
            SceneManager.LoadScene("3. Walk_Scene"); // 원하는 씬 이름으로 교체
        }
        else
        {
            needText.text = $"다음 아이템이 필요해요!{missing}";
        }
    }
}
