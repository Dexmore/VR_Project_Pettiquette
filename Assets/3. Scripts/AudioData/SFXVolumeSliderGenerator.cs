using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SFXVolumeSliderGenerator : MonoBehaviour
{
    [Header("�����̴� ������")]
    public GameObject sfxSliderPrefab;

    [Header("�θ� ������Ʈ")]
    public Transform contentParent;

    [Header("UI Button")]
    public Button button;

    [Header("���� ���")]
    public GameObject closeTarget;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => AudioManager.Instance != null && AudioManager.Instance.sfxDatas.Count > 0);
        GenerateSliders();

        button.onClick.AddListener(() =>
        {
            if(closeTarget != null)
            {
                closeTarget.gameObject.SetActive(false);
                UIManager.Instance.CloseAllSubCanvas();
            }
        });
    }

    void GenerateSliders()
    {
        foreach(var sfx in AudioManager.Instance.sfxDatas)
        {
            GameObject item = Instantiate(sfxSliderPrefab, contentParent);
            SFXVolumeSlider slider = item.GetComponent<SFXVolumeSlider>();
            slider.Initialize(sfx.category);
        }
    }
}
