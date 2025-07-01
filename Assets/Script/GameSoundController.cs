using UnityEngine;
using UnityEngine.UI;

public class GameSoundController : MonoBehaviour
{
    public Button toggleButton;
    public AudioSource audioSource;

    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

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
        isMuted = !isMuted;

        if (isMuted)
        {
            audioSource.Pause(); // 일시정지
        }
        else
        {
            audioSource.UnPause(); // 다시 재생
        }

        UpdateButtonImage();

        Debug.Log($"음소거 상태: {isMuted}, AudioSource.isPlaying 상태: {audioSource.isPlaying}");
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
