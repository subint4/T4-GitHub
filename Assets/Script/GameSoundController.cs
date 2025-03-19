using UnityEngine;
using UnityEngine.UI;

public class GameSoundController : MonoBehaviour
{
    public Button toggleButton;
    // 오디오 소스
    public AudioSource audioSource;

    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    //음소거 상태
    private bool isMuted = false;
    private Image buttonImage;

    void Start()
    {
        buttonImage = toggleButton.GetComponent<Image>();
        toggleButton.onClick.AddListener(ToggleSound);

        UpdateButtonImage();
    }

    public void ToggleSound()
    {
        // 음소거 상태 토글
        isMuted = !isMuted;

        // 오디오 음소거 설정
        audioSource.mute = isMuted;

        UpdateButtonImage();

        Debug.Log($"음소거 상태: {isMuted}, AudioSource Mute 상태: {audioSource.mute}");
    }

    void UpdateButtonImage()
    {
        if (buttonImage != null)
        {
            Debug.Log($"버튼 이미지 변경됨: {(isMuted ? "OFF" : "ON")}");
            buttonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }
}
