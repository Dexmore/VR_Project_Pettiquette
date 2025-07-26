using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleMenu : MonoBehaviour
{
    public InputActionReference toggleMenuAction;

    private void OnEnable()
    {
        toggleMenuAction.action.Enable();
        toggleMenuAction.action.performed += ToggleMenuActive;
    }

    private void OnDisable()
    {
        toggleMenuAction.action.performed -= ToggleMenuActive;
        toggleMenuAction.action.Disable();
    }

    private void ToggleMenuActive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool on = UIManager.Instance.MenuCanvas.activeSelf;
            UIManager.Instance.MenuCanvas.SetActive(!on);
        }
    }
}
