using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSoundController : MonoBehaviour
{
    public Button toggleButton;
    // 오디오 소스
    public AudioSource audioSource;

    //음소거 상태
    private bool isMuted = false;

    void Start()
    {        
        toggleButton.onClick.AddListener(ToggleSound);
    }

    void ToggleSound()
    {
        // 음소거 상태 토글
        isMuted = !isMuted;

        // 오디오 음소거 설정
        audioSource.mute = isMuted;

        Debug.Log($"음소거 상태: {isMuted}, AudioSource Mute 상태: {audioSource.mute}");
    }
}
