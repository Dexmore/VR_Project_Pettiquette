using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayUI : MonoBehaviour
{
    public Image spriteImage;
    public TextMeshProUGUI objectText;

    public void ShowUI(ItemData data)
    {
        if(spriteImage != null)
        {
            spriteImage.sprite = data.itemIcon;
        }

        if(objectText != null)
        {
            objectText.text = data.itemDescription;
        }
    }

    public void Clear()
    {
        if (spriteImage != null)
        {
            spriteImage.sprite = null;
        }

        if (objectText != null)
        {
            objectText.text = string.Empty;
        }
    }
}
