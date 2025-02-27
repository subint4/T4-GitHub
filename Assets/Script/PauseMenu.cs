using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // 팝업 UI
    public GameObject pausePanel;
    // 게임 재개 버튼
    public Button resumeButton;
    // 메인화면 버튼
    public Button mainMenuButton;
    // 효과음 토글 버튼
    public Button ImpactToggleButton;
    // 홈 버튼 (추가)
    public Button homeButton;
#pragma warning disable CS0414
    private bool isPaused = false;
#pragma warning restore CS0414
    // 현재 효과음 상태
    private bool isSFXOn = true;
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
        // 시작 시 팝업창 비활성화
        pausePanel.SetActive(false);
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        ImpactToggleButton.onClick.AddListener(ToggleImpact);
        // 홈 버튼 클릭시 일시정지를 실행
        homeButton.onClick.AddListener(PauseGame);
    }

    // 게임 일시정지
    public void PauseGame()
    {
        isPaused = true;
        // 게임 멈춤
        Time.timeScale = 0f;
        // UI 창 표시
        pausePanel.SetActive(true);
    }

    // 게임 재개
    public void ResumeGame()
    {
        isPaused = false;
        // 시간 다시 흐르게
        Time.timeScale = 1f;
        // UI 창 숨기기
        pausePanel.SetActive(false);
    }

    // 메인화면으로 이동
    public void GoToMainMenu()
    {
        // 시간 흐르게하기
        Time.timeScale = 1f;
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audio in allAudio)
        {
            audio.mute = true;
        }
        // "MainMenu" 씬으로 이동
        SceneManager.LoadScene("MainMenu");
    }

    // 효과음 켰다 껐다
    public void ToggleImpact()
    {
        isSFXOn = !isSFXOn;
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();

        // 모든 AudioSource 찾기 (배경음악 제외)
        foreach (AudioSource audio in allAudio)
        {
            if (!audio.CompareTag("BGM")) // "BGM" 태그가 있는 오브젝트는 제외
                audio.mute = !isSFXOn;
        }

        Debug.Log("효과금 여기까지 오나?");
    }
}