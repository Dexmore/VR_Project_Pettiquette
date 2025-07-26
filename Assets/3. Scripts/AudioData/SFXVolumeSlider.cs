using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class UI_TnS
{
    public SFXCategory category;
    public TextMeshProUGUI text;
    public Slider slider;
}

public class SFXVolumeSlider : MonoBehaviour
{
    [Header("UI Combine")]
    public List<UI_TnS> uiCombine;
    void Start()
    {
        foreach(var ui in uiCombine)
        {
            if(ui.text != null)
            {
                ui.text.text = ui.category.ToString();
            }

            float savedVolume = AudioManager.Instance.GetSFXVolume(ui.category);
            ui.slider.SetValueWithoutNotify(savedVolume);

            ui.slider.onValueChanged.AddListener((value) =>
            {
                AudioManager.Instance.SetSFXVolume(ui.category, value);
            });
        }
    }

    private void OnDestroy()
    {
        foreach(var ui in uiCombine)
        {
            ui.slider.onValueChanged.RemoveAllListeners();
        }
    }
}
