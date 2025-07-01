using DG.Tweening; // DOTween 사용
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public Button ReplayButton;
    public Button defeatMenuButton;

    [Header("Home Popup Buttons")]
    public Button homeConfirmButton;
    public Button homeCancelButton;

    private bool isGameOver = false;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[PopupManager] OnSceneLoaded 실행됨"); // 반드시 찍히는지 확인

        Debug.Log($"[PopupManager] 씬 변경 감지: {scene.name} - 팝업 초기화 실행");

        ReconnectButtons(); // 새로 생성된 버튼에 다시 연결
        HideAllPopups();
    }

    private void ReconnectButtons()
    {
        // 새 씬의 버튼들을 다시 찾아서 연결
        ReplayButton = GameObject.Find("ReplayButton")?.GetComponent<Button>();
        defeatMenuButton = GameObject.Find("DefeatMenuButton")?.GetComponent<Button>();

        if (ReplayButton != null)
        {
            ReplayButton.onClick.RemoveAllListeners();
            ReplayButton.onClick.AddListener(RestartCurrentStage);
            Debug.Log("[PopupManager] retryButton 연결 완료");
        }
        else
        {
            Debug.LogWarning("[PopupManager] retryButton 찾을 수 없음");
        }

        if (defeatMenuButton != null)
        {
            defeatMenuButton.onClick.RemoveAllListeners();
            defeatMenuButton.onClick.AddListener(ReturnToMainMenu);
            Debug.Log("[PopupManager] defeatMenuButton 연결 완료");
        }
        else
        {
            Debug.LogWarning("[PopupManager] defeatMenuButton 찾을 수 없음");
        }
    }

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
        HideAllPopups(); // **게임 시작 시 모든 팝업 숨김**

        Debug.Log("[PopupManager] 모든 팝업 숨김");

        if (nextStageButton != null)
            nextStageButton.onClick.AddListener(LoadNextStage);

        if (victoryMenuButton != null)
            victoryMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (ReplayButton != null)
            ReplayButton.onClick.AddListener(RestartCurrentStage);

        if (defeatMenuButton != null)
            defeatMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (homeConfirmButton != null)
            homeConfirmButton.onClick.AddListener(ReturnToMainMenu);

        if (homeCancelButton != null)
            homeCancelButton.onClick.AddListener(CloseHomePopup);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    /// <summary>
    /// **모든 팝업 숨기기**
    /// </summary>
    public void HideAllPopups()
    {
        if (victoryPopup != null) victoryPopup.SetActive(false);
        if (defeatPopup != null) defeatPopup.SetActive(false);
        if (homePopup != null) homePopup.SetActive(false);
        if (blockPanel != null) blockPanel.SetActive(false); // 터치 차단 패널도 비활성화

        isGameOver = false; // **게임 오버 상태 초기화**
        Time.timeScale = 1f; // 게임 속도 정상화
        Debug.Log("[PopupManager] 모든 팝업 숨김 및 게임 상태 초기화 완료.");
    }

    /// <summary>
    /// **승리 팝업 표시 및 화면 터치 차단**
    /// </summary>
    public void ShowVictoryPopup()
    {
        if (isGameOver) return;

        HideAllPopups(); // **기존 팝업 숨김 후 표시**
        isGameOver = true;

        if (victoryPopup != null)
        {
            victoryPopup.SetActive(true);
            EnableBlockPanel();
            Time.timeScale = 0f;
            Debug.Log("[PopupManager] 승리 팝업 표시!");
        }
    }

    /// <summary>
    /// **패배 팝업 표시 및 화면 터치 차단**
    /// </summary>
    public void ShowDefeatPopup()
    {
        if (isGameOver) return;

        HideAllPopups(); // **기존 팝업 숨김 후 표시**
        isGameOver = true;

        if (defeatPopup != null)
        {
            defeatPopup.SetActive(true);
            EnableBlockPanel();
            Time.timeScale = 0f;
            Debug.Log("[PopupManager] 패배 팝업 표시!");
        }
    }

    /// <summary>
    /// **홈 버튼 클릭 시 팝업 표시**
    /// </summary>
    public void ShowHomePopup()
    {
        HideAllPopups(); // **기존 팝업 숨김 후 표시**

        if (homePopup != null)
        {
            homePopup.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("[PopupManager] 홈 팝업 표시!");
        }
    }

    /// <summary>
    /// **홈 팝업에서 확인 시 메인 메뉴로 이동**
    /// </summary>
    private void ReturnToMainMenu()
    {
        HideAllPopups(); // **기존 팝업 숨김**
        Time.timeScale = 1f;
        DOTween.KillAll(); // 모든 DOTween 애니메이션 정리

        Debug.Log("[PopupManager] 메인 메뉴로 이동");
        
        Destroy(gameObject); // 중복 방지
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// **현재 스테이지 다시 시작**
    /// </summary>
    private void RestartCurrentStage()
    {
        HideAllPopups();
        Time.timeScale = 1f;

        GameManager.Instance.ResetAllManagers();

        WaveManager.Instance.StartWave(); // 다시 웨이브 시작
    }


    private IEnumerator RestartStageRoutine()
    {
        yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    /// <summary>
    /// **홈 팝업 닫기**
    /// </summary>
    private void CloseHomePopup()
    {
        if (homePopup != null)
        {
            homePopup.SetActive(false);
            Time.timeScale = 1f;
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

}
