using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AllSoundManager : MonoBehaviour
{
    public Button muteButton; // 버튼 연결
    public Sprite muteSprite; // 음소거 상태 아이콘
    public Sprite unmuteSprite; // 소리 켜진 상태 아이콘
    private Image buttonImage; // 버튼 이미지 변경용
    private bool isMuted = false; // 현재 음소거 상태
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Stage"))
        {
            Debug.Log("[PauseMenu] 스테이지 씬에서 UI 활성화");
            gameObject.SetActive(true); // PauseMenu 강제 활성화
        }
    }
    void Start()
    {
        buttonImage = muteButton.GetComponent<Image>();

        // 버튼 클릭 이벤트 연결
        muteButton.onClick.AddListener(ToggleMute);

        // 초기 상태 설정
        UpdateButtonIcon();
    }

    void ToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.pause = isMuted; // Unity에서 전체 오디오 컨트롤

        UpdateButtonIcon();
    }

    void UpdateButtonIcon()
    {
        if (isMuted)
        {
            buttonImage.sprite = unmuteSprite; // 소리 아이콘으로 변경
        }
        else
        {
            buttonImage.sprite = muteSprite; // 음소거 아이콘으로 변경
        }
    }
}
