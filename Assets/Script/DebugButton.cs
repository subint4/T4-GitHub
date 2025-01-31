using UnityEngine;
using UnityEngine.UI;

public class ButtonDebug : MonoBehaviour
{
    void Start()
    {        // RectTransform 위치 확인
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Debug.Log($"버튼 위치: {rectTransform.anchoredPosition}");
            Debug.Log($"버튼 크기: {rectTransform.sizeDelta}");
        }
        else
        {
            Debug.LogError("RectTransform을 찾을 수 없습니다!");
        }

        // 버튼 활성화 상태 확인
        if (gameObject.activeInHierarchy)
        {
            Debug.Log("버튼이 활성화되어 있습니다.");
        }
        else
        {
            Debug.LogWarning("버튼이 비활성화되어 있습니다.");
        }

        // Image 컴포넌트 확인
        Image buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            Debug.Log($"버튼 Alpha 값: {buttonImage.color.a}");
            Debug.Log($"Raycast Target 활성 상태: {buttonImage.raycastTarget}");
        }
        else
        {
            Debug.LogWarning("Image 컴포넌트가 없습니다!");
        }

        // Button 컴포넌트 확인
        Button button = GetComponent<Button>();
        if (button != null)
        {
            Debug.Log($"Button Interactable 상태: {button.interactable}");
        }
        else
        {
            Debug.LogWarning("Button 컴포넌트가 없습니다!");
            var image = GetComponent<Image>();
            if (image != null)
            {
                Debug.Log($"Image Alpha: {image.color.a}");
            }

            if (button != null)
            {
                Debug.Log($"Button Interactable: {button.interactable}");
            }
        }
    }
}