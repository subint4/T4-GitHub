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
        // 메인에서 넘어왔을 때 전체 음소거가 남아 있을 수 있으니 해제
        AudioListener.pause = false;

        // 오디오 처음부터 재생
        audioSource.Stop();      // 혹시 남아있는 재생이 있으면 정지
        audioSource.time = 0f;   // 재생 위치 0으로
        audioSource.Play();      // 처음부터 재생

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
