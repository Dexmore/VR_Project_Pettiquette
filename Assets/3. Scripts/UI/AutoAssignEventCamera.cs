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
                Debug.Log("[AutoAssignEventCamera] Main Camera 자동 할당 완료.");
            }
            else
            {
                Debug.LogWarning("[AutoAssignEventCamera] Main Camera를 찾을 수 없습니다.");
            }
        }
    }
}
