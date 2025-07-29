using UnityEngine;
using UnityEngine.UI;

public class CheckItemUImanager : MonoBehaviour
{
    [Header("Images")]
    public Image snackCheckImage;
    public Image shovelCheckImage;
    public Image muzzleCheckImage;
    public Image ballCheckImage;
    public Image collarCheckImage;
    public Image bowlCheckImage;

    void Start()
    {
        snackCheckImage.gameObject.SetActive(false);
        shovelCheckImage.gameObject.SetActive(false);
        muzzleCheckImage.gameObject.SetActive(false);
        ballCheckImage.gameObject.SetActive(false);
        collarCheckImage.gameObject.SetActive(false);
        bowlCheckImage.gameObject.SetActive(false);
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
}
