using UnityEngine;
using UnityEngine.UI;

public class ImageSlider : MonoBehaviour
{
    public GameObject imagePanel; // 이미지 패널 (전체 화면)
    public Image displayImage; // 이미지가 표시될 UI Image 컴포넌트
    public Sprite[] spriteArray; // 이미지 슬라이드 배열

    public Button showButton; // 이미지 표시 버튼
    public Button hideButton; // 이미지 숨김 버튼
    public Button nextButton; // 다음 이미지 버튼
    public Button prevButton; // 이전 이미지 버튼

    private int currentIndex = 0; // 현재 이미지 인덱스

    private void Start()
    {
        // 초기 이미지 패널 숨김
        imagePanel.SetActive(false);

        // 버튼 이벤트 등록
        showButton.onClick.AddListener(ShowImagePanel);
        hideButton.onClick.AddListener(HideImagePanel);
        nextButton.onClick.AddListener(NextImage);
        prevButton.onClick.AddListener(PreviousImage);

        // 이미지가 존재하면 첫 번째 이미지 표시
        if (spriteArray.Length > 0)
        {
            displayImage.sprite = spriteArray[0];
        }
    }

    /// <summary>
    /// 이미지 패널을 활성화하고 첫 번째 이미지 표시
    /// </summary>
    private void ShowImagePanel()
    {
        imagePanel.SetActive(true);
        UpdateImage();
        Debug.Log("[ImageSlider] 이미지 패널 표시됨");
    }

    /// <summary>
    /// 이미지 패널을 숨김
    /// </summary>
    private void HideImagePanel()
    {
        imagePanel.SetActive(false);
        Debug.Log("[ImageSlider] 이미지 패널 숨김");
    }

    /// <summary>
    /// 다음 이미지 표시
    /// </summary>
    private void NextImage()
    {
        if (spriteArray.Length == 0) return;

        currentIndex++;
        if (currentIndex >= spriteArray.Length)
        {
            currentIndex = 0; // 처음으로 돌아감
        }
        UpdateImage();
    }

    /// <summary>
    /// 이전 이미지 표시
    /// </summary>
    private void PreviousImage()
    {
        if (spriteArray.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = spriteArray.Length - 1; // 마지막 이미지로 이동
        }
        UpdateImage();
    }

    /// <summary>
    /// 현재 이미지 업데이트
    /// </summary>
    private void UpdateImage()
    {
        displayImage.sprite = spriteArray[currentIndex];
        Debug.Log($"[ImageSlider] 이미지 변경: {currentIndex + 1}/{spriteArray.Length}");
    }
}
