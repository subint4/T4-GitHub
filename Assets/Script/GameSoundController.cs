using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSoundController : MonoBehaviour
{
    public Button toggleButton;
    public Sprite soundOnImage;
    public Sprite soundOffImage;
    // 오디오 소스
    public AudioSource audioSource;

    //음소거 상태
    private bool isMuted = false;

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleSound);

        UpdateButtonImage();
    }

    void ToggleSound()
    {
        // 음소거 상태 토글
        isMuted = !isMuted;

        // 오디오 음소거 설정
        audioSource.mute = isMuted;

        // 버튼 이미지 업데이트
        UpdateButtonImage();

        Debug.Log($"음소거 상태: {isMuted}, AudioSource Mute 상태: {audioSource.mute}");
    }
    void UpdateButtonImage()
    {
        // 버튼의 이미지를 현재 음소거 상태에 따라 변경
        toggleButton.image.sprite = isMuted ? soundOnImage : soundOffImage;
    }
}
