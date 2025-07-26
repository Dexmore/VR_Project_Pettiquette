using UnityEngine;
using System.Collections;

public class SFXVolumeSliderGenerator : MonoBehaviour
{
    [Header("슬라이더 프리팹")]
    public GameObject sfxSliderPrefab;

    [Header("부모 오브젝트")]
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
