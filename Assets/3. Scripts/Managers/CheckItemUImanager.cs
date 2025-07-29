using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

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
        needText.text = string.Empty;

        SceneMoveButton.onClick.AddListener(GoToWalkScene);

        RefreshCheckUI(); // 인벤토리 상태 반영
    }

    public void RefreshCheckUI()
    {
        var items = InventoryManager.Instance?.Items;
        if (items == null) return;

        snackCheckImage.gameObject.SetActive(items.Any(i => i.itemName == "Food"));
        shovelCheckImage.gameObject.SetActive(items.Any(i => i.itemName == "Shovel"));
        muzzleCheckImage.gameObject.SetActive(items.Any(i => i.itemName == "Muzzle"));
        ballCheckImage.gameObject.SetActive(items.Any(i => i.itemName == "Ball"));
        collarCheckImage.gameObject.SetActive(items.Any(i => i.itemName == "Collar"));
        bowlCheckImage.gameObject.SetActive(items.Any(i => i.itemName == "Bowl"));
    }

    public void GoToWalkScene()
    {
        RefreshCheckUI(); // 이동하기 전 다시 상태 확인

        string missing = "";

        if (!snackCheckImage.gameObject.activeSelf) missing += "=> 간식\n";
        if (!shovelCheckImage.gameObject.activeSelf) missing += "=> 삽\n";
        if (!muzzleCheckImage.gameObject.activeSelf) missing += "=> 입마개\n";
        if (!ballCheckImage.gameObject.activeSelf) missing += "=> 공\n";
        if (!collarCheckImage.gameObject.activeSelf) missing += "=> 목줄\n";
        if (!bowlCheckImage.gameObject.activeSelf) missing += "=> 밥그릇\n";

        if (string.IsNullOrEmpty(missing))
        {
            SceneManager.LoadScene("3. Walk_Scene");
        }
        else
        {
            needText.text = $"다음 아이템이 필요해요!\n{missing}";
        }
    }
}