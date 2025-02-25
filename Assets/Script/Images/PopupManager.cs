using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("UI Popups")]
    public GameObject victoryPopup;  // 승리 UI 팝업
    public GameObject defeatPopup;   // 패배 UI 팝업

    [Header("Victory Popup Buttons")]
    public Button nextStageButton;   // 다음 스테이지 버튼
    public Button victoryMenuButton; // 승리 후 메인 메뉴 버튼

    [Header("Defeat Popup Buttons")]
    public Button retryButton;       // 재도전 버튼
    public Button defeatMenuButton;  // 패배 후 메인 메뉴 버튼

    private bool isGameOver = false; // 게임 종료 여부

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        if (nextStageButton != null)
            nextStageButton.onClick.AddListener(LoadNextStage);

        if (victoryMenuButton != null)
            victoryMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (retryButton != null)
            retryButton.onClick.AddListener(RestartCurrentStage);

        if (defeatMenuButton != null)
            defeatMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    /// <summary>
    /// **승리 시 팝업 표시**
    /// </summary>
    public void ShowVictoryPopup()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (victoryPopup != null)
        {
            victoryPopup.SetActive(true);
            Time.timeScale = 0f; // 게임 정지
            Debug.Log("[PopupManager] 승리 팝업 표시!");
        }
        else
        {
            Debug.LogError("[PopupManager] 승리 팝업이 설정되지 않았습니다!");
        }
    }

    /// <summary>
    /// **패배 시 팝업 표시**
    /// </summary>
    public void ShowDefeatPopup()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (defeatPopup != null)
        {
            defeatPopup.SetActive(true);
            Time.timeScale = 0f; // 게임 정지
            Debug.Log("[PopupManager] 패배 팝업 표시!");
        }
        else
        {
            Debug.LogError("[PopupManager] 패배 팝업이 설정되지 않았습니다!");
        }
    }

    /// <summary>
    /// **다음 스테이지로 이동 (승리 후)**
    /// </summary>
    private void LoadNextStage()
    {
        Time.timeScale = 1f; // 시간 정상화

        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        int currentSubStage = PlayerPrefs.GetInt("CurrentSubStage", 1);

        // 다음 스테이지로 변경
        int nextSubStage = currentSubStage + 1;
        int nextStage = currentStage;

        // 예제: 서브스테이지가 3개라면, 3에서 증가할 경우 다음 메인 스테이지로 이동
        if (nextSubStage > 3) // 최대 서브 스테이지 수를 설정
        {
            nextStage++;
            nextSubStage = 1;
        }

        // 새로운 스테이지 정보 저장
        PlayerPrefs.SetInt("CurrentStage", nextStage);
        PlayerPrefs.SetInt("CurrentSubStage", nextSubStage);

        Debug.Log($"[PopupManager] 다음 스테이지 로드: {nextStage}-{nextSubStage}");
        SceneManager.LoadScene("Stage1"); // 다음 스테이지 씬 이름 (Stage1 기본값)
    }

    /// <summary>
    /// **현재 스테이지 다시 시작 (패배 후)**
    /// </summary>
    private void RestartCurrentStage()
    {
        Time.timeScale = 1f; // 시간 정상화
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// **메인 메뉴로 이동**
    /// </summary>
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // 시간 정상화
        SceneManager.LoadScene("MainMenu");
    }
}
