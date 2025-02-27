using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening; // DOTween 사용

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("UI Popups")]
    public GameObject victoryPopup;
    public GameObject defeatPopup;
    public GameObject homePopup; // 홈 이동 확인 팝업
    public GameObject blockPanel; // 터치 방지용 블로킹 패널 추가

    [Header("Victory Popup Buttons")]
    public Button nextStageButton;
    public Button victoryMenuButton;

    [Header("Defeat Popup Buttons")]
    public Button retryButton;
    public Button defeatMenuButton;

    [Header("Home Popup Buttons")]
    public Button homeConfirmButton;
    public Button homeCancelButton;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (victoryPopup != null) victoryPopup.SetActive(false);
        if (defeatPopup != null) defeatPopup.SetActive(false);
        if (homePopup != null) homePopup.SetActive(false);
        if (blockPanel != null) blockPanel.SetActive(false); // 블로킹 패널 초기 비활성화

        if (nextStageButton != null)
            nextStageButton.onClick.AddListener(LoadNextStage);

        if (victoryMenuButton != null)
            victoryMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (retryButton != null)
            retryButton.onClick.AddListener(RestartCurrentStage);

        if (defeatMenuButton != null)
            defeatMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (homeConfirmButton != null)
            homeConfirmButton.onClick.AddListener(ReturnToMainMenu);

        if (homeCancelButton != null)
            homeCancelButton.onClick.AddListener(CloseHomePopup);
    }

    /// <summary>
    /// **승리 팝업 표시 및 화면 터치 차단**
    /// </summary>
    public void ShowVictoryPopup()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (victoryPopup != null)
        {
            victoryPopup.SetActive(true);
            EnableBlockPanel(); // 화면 터치 차단
            Time.timeScale = 0f; // 게임 멈춤
            Debug.Log("[PopupManager] 승리 팝업 표시!");
        }
        else
        {
            Debug.LogError("[PopupManager] 승리 팝업이 설정되지 않았습니다!");
        }
    }
    /// <summary>
    /// **다음 스테이지로 이동**
    /// </summary>
    private void LoadNextStage()
    {
        DisableBlockPanel(); // 터치 차단 해제
        Time.timeScale = 1f; // 시간 정상화
        DOTween.KillAll(); // DOTween 애니메이션 제거

        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        int currentSubStage = PlayerPrefs.GetInt("CurrentSubStage", 1);

        int nextSubStage = currentSubStage + 1;
        int nextStage = currentStage;

        if (nextSubStage > 3) // 서브 스테이지 개수에 맞게 수정
        {
            nextStage++;
            nextSubStage = 1;
        }

        PlayerPrefs.SetInt("CurrentStage", nextStage);
        PlayerPrefs.SetInt("CurrentSubStage", nextSubStage);

        Debug.Log($"[PopupManager] 다음 스테이지 로드: {nextStage}-{nextSubStage}");
        SceneManager.LoadScene($"Stage{nextStage}");
    }

    /// <summary>
    /// **현재 스테이지 다시 시작 (패배 후)**
    /// </summary>
    private void RestartCurrentStage()
    {
        DisableBlockPanel(); // 터치 차단 해제
        Debug.Log("[PopupManager] 현재 스테이지 다시 시작!");

        Time.timeScale = 1f; // 게임 속도 정상화
        DOTween.KillAll(); // 모든 DOTween 애니메이션 정리

        Destroy(gameObject); // 기존 PopupManager 삭제 (씬 재시작 시 중복 방지)

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// **패배 팝업 표시 및 화면 터치 차단**
    /// </summary>
    public void ShowDefeatPopup()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (defeatPopup != null)
        {
            defeatPopup.SetActive(true);
            EnableBlockPanel(); // 화면 터치 차단
            Time.timeScale = 0f; // 게임 멈춤
            Debug.Log("[PopupManager] 패배 팝업 표시!");
        }
        else
        {
            Debug.LogError("[PopupManager] 패배 팝업이 설정되지 않았습니다!");
        }
    }

    /// <summary>
    /// **홈 버튼 클릭 시 팝업 표시**
    /// </summary>
    public void ShowHomePopup()
    {
        if (homePopup != null)
        {
            homePopup.SetActive(true);
            Time.timeScale = 0f; // 게임 정지
            Debug.Log("[PopupManager] 홈 팝업 표시!");
        }
    }

    /// <summary>
    /// **홈 팝업에서 확인 시 메인 메뉴로 이동**
    /// </summary>
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // 게임 속도 정상화
        DOTween.KillAll(); // 모든 DOTween 애니메이션 정리

        // 스테이지 진행 정보 초기화
        PlayerPrefs.SetInt("CurrentStage", 1);
        PlayerPrefs.SetInt("CurrentSubStage", 1);
        PlayerPrefs.Save();

        Debug.Log("[PopupManager] 메인 메뉴로 이동 - 스테이지 진행 정보 초기화 완료");

        Destroy(gameObject); // 중복 방지
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// **홈 팝업 닫기**
    /// </summary>
    private void CloseHomePopup()
    {
        if (homePopup != null)
        {
            homePopup.SetActive(false);
            Time.timeScale = 1f; // 게임 재개
        }
    }

    /// <summary>
    /// **터치 입력 차단을 위한 블로킹 패널 활성화**
    /// </summary>
    private void EnableBlockPanel()
    {
        if (blockPanel != null)
        {
            blockPanel.SetActive(true);
        }
    }

    /// <summary>
    /// **터치 차단 패널 비활성화**
    /// </summary>
    private void DisableBlockPanel()
    {
        if (blockPanel != null)
        {
            blockPanel.SetActive(false);
        }
    }
}
