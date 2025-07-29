using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class AutoAssignEventCamera : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();

        if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
        {
            Camera mainCam = Camera.main;

            if (mainCam != null)
            {
                canvas.worldCamera = mainCam;
                Debug.Log("[AutoAssignEventCamera] Main Camera �ڵ� �Ҵ� �Ϸ�.");
            }
            else
            {
                Debug.LogWarning("[AutoAssignEventCamera] Main Camera�� ã�� �� �����ϴ�.");
            }
        }
    }
}
