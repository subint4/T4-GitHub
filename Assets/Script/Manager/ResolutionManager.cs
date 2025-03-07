using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    void Start()
    {
        SetResolution();
    }

    void SetResolution()
    {
        int targetWidth = 1920; // 기준 해상도 너비
        int targetHeight = 1080; // 기준 해상도 높이
        bool fullscreen = true; // 전체 화면 사용 여부

        float targetAspect = (float)targetWidth / targetHeight;
        float currentAspect = (float)Screen.width / Screen.height;

        if (currentAspect >= targetAspect)
        {
            // 현재 화면 비율이 기준 비율보다 클 경우 (너비가 너무 길어진 경우)
            int newWidth = Mathf.RoundToInt(targetHeight * currentAspect);
            Screen.SetResolution(newWidth, targetHeight, fullscreen);
        }
        else
        {
            // 현재 화면 비율이 기준 비율보다 작을 경우 (세로가 너무 긴 경우)
            int newHeight = Mathf.RoundToInt(targetWidth / currentAspect);
            Screen.SetResolution(targetWidth, newHeight, fullscreen);
        }

        Debug.Log($"[ResolutionManager] 해상도 조정됨: {Screen.width}x{Screen.height}");
    }
}
