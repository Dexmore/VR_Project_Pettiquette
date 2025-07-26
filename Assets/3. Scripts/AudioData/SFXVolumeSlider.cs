using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SFXVolumeSlider : MonoBehaviour
{
    [Header("UI Combine")]
    public TextMeshProUGUI text;
    public Slider slider;

    private SFXCategory category;
    private void Awake()
    {
        if (text == null)
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            if(text == null)
            {
                Debug.Log("[SFXVolumeSlider] Text 못 찾음");
            }
        }

        if(slider == null)
        {
            slider = GetComponentInChildren<Slider>();
            if (slider == null)
            {
                Debug.Log("[SFXVolumeSlider] Slider 못 찾음");
            }
        }
    }
    public void Initialize(SFXCategory category)
    {
        this.category = category;

        if(text != null)
        {
            text.text = category.ToString();
        }
        
        if(slider != null)
        {
            float saveVolume = AudioManager.Instance.GetSFXVolume(category); // 카테고리에 해당하는 볼륨 가지고 오기
            slider.value = saveVolume;

            slider.onValueChanged.AddListener((value) =>
            {
                AudioManager.Instance.SetSFXVolume(category, value);
            });
        }
        else
        {
            Debug.Log($"[SFXVolumeSlider] {category} 못 찾음");
        }
    }

    private void OnDestroy()
    {
        if(slider != null)
        {
            slider.onValueChanged.RemoveAllListeners();
        }
    }
}
