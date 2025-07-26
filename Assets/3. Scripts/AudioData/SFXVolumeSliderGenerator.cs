using UnityEngine;
using System.Collections;

public class SFXVolumeSliderGenerator : MonoBehaviour
{
    [Header("�����̴� ������")]
    public GameObject sfxSliderPrefab;

    [Header("�θ� ������Ʈ")]
    public Transform contentParent;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => AudioManager.Instance != null && AudioManager.Instance.sfxDatas.Count > 0);
        GenerateSliders();
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
