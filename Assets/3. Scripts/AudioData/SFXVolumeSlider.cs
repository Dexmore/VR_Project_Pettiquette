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
                Debug.Log("[SFXVolumeSlider] Text �� ã��");
            }
        }

        if(slider == null)
        {
            slider = GetComponentInChildren<Slider>();
            if (slider == null)
            {
                Debug.Log("[SFXVolumeSlider] Slider �� ã��");
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
            float saveVolume = AudioManager.Instance.GetSFXVolume(category); // ī�װ��� �ش��ϴ� ���� ������ ����
            slider.value = saveVolume;

            slider.onValueChanged.AddListener((value) =>
            {
                AudioManager.Instance.SetSFXVolume(category, value);
            });
        }
        else
        {
            Debug.Log($"[SFXVolumeSlider] {category} �� ã��");
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
