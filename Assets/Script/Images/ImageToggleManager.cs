using UnityEngine;
using UnityEngine.UI;

public class ImageToggleManager : MonoBehaviour
{
    public GameObject imagePanel; // 표시할 이미지 패널
    public Button showButton;     // 이미지를 표시할 버튼
    public Button hideButton;     // 이미지를 숨길 버튼

    private void Start()
    {
        // 초기에는 이미지 숨김
        imagePanel.SetActive(false);

        // 버튼 클릭 이벤트 등록
        showButton.onClick.AddListener(ShowImage);
        hideButton.onClick.AddListener(HideImage);
    }

    /// <summary>
    /// 이미지 패널을 활성화
    /// </summary>
    private void ShowImage()
    {
        imagePanel.SetActive(true);
        Debug.Log("[ImageToggleManager] 이미지 표시됨");
    }

    /// <summary>
    /// 이미지 패널을 비활성화
    /// </summary>
    private void HideImage()
    {
        imagePanel.SetActive(false);
        Debug.Log("[ImageToggleManager] 이미지 숨김");
    }
}
