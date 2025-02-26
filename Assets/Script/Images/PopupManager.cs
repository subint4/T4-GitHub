using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening; // DOTween 사용

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("UI Popups")]
    public GameObject victoryPopup;
    public GameObject defeatPopup;

    [Header("Victory Popup Buttons")]
    public Button nextStageButton;
    public Button victoryMenuButton;

    [Header("Defeat Popup Buttons")]
    public Button retryButton;
    public Button defeatMenuButton;

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
    /// **승리 팝업 표시**
    /// </summary>
    public void ShowVictoryPopup()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (victoryPopup != null)
        {
            victoryPopup.SetActive(true);
            Time.timeScale = 0f; // 게임 멈춤
            Debug.Log("[PopupManager] 승리 팝업 표시!");
        }
        else
        {
            Debug.LogError("[PopupManager] 승리 팝업이 설정되지 않았습니다!");
        }
    }

    /// <summary>
    /// **패배 팝업 표시**
    /// </summary>
    public void ShowDefeatPopup()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (defeatPopup != null)
        {
            defeatPopup.SetActive(true);
            Time.timeScale = 0f; // 게임 멈춤
            Debug.Log("[PopupManager] 패배 팝업 표시!");
        }
        else
        {
            Debug.LogError("[PopupManager] 패배 팝업이 설정되지 않았습니다!");
        }
    }

    /// <summary>
    /// **다음 스테이지로 이동**
    /// </summary>
    private void LoadNextStage()
    {
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
        Debug.Log("[PopupManager] 현재 스테이지 다시 시작!");

        Time.timeScale = 1f; // **게임 속도 정상화**
        DOTween.KillAll(); // **모든 DOTween 애니메이션 정리**

        // **기존 PopupManager 삭제 (씬 재시작 시 중복 방지)**
        Destroy(gameObject);

        // **씬 다시 로드**
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// **메인 메뉴로 이동**
    /// </summary>
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // **게임 속도 정상화**
        DOTween.KillAll(); // **모든 DOTween 애니메이션 정리**

        // **PopupManager 삭제 (중복 방지)**
        Destroy(gameObject);

        SceneManager.LoadScene("MainMenu");
    }
}
