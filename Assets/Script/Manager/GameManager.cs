using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public WaveManager waveManager;
    public EnemyManager enemyManager;

    private bool isGameOver = false;

    // GameManager를 유지할 특정 씬 목록
    private readonly string[] allowedScenes = { "Stage1", "Stage2", "Stage3" };

    private void Awake()
    {
        // 싱글턴 패턴 유지 (중복 생성 방지)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        string currentScene = SceneManager.GetActiveScene().name;

        // 특정 씬이 아니면 GameManager를 삭제 (메인메뉴에서는 유지)
        if (!IsAllowedScene(currentScene))
        {
            Debug.LogWarning($"[GameManager] {currentScene}에서는 GameManager가 필요하지 않으므로 삭제됨.");
            Destroy(gameObject);
            return;
        }


        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        Debug.Log($"[GameManager] 씬 로드됨: {sceneName}");

        // 특정 씬이 아닐 경우 자동 삭제
        if (!IsAllowedScene(sceneName))
        {
            Debug.LogWarning($"[GameManager] {sceneName}에서는 GameManager가 필요하지 않으므로 삭제됨.");
            Destroy(gameObject);
            return;
        }

        // 새로운 씬에서 WaveManager, EnemyManager 자동 할당
        waveManager = FindObjectOfType<WaveManager>();
        enemyManager = FindObjectOfType<EnemyManager>();

        // 씬 변경 후 웨이브 재로딩
        if (waveManager != null)
        {
            WaveManager.Instance.LoadWavesForStage(StageManager.Instance.currentStageNum, StageManager.Instance.currentSubStageNum);
        }
    }

    private void Start()
    {
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }
        if (waveManager == null)
        {
            Debug.LogError("[GameManager] WaveManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        // 진행 상태 초기화
        PlayerPrefs.SetInt("WaveProgress", 0);
        PlayerPrefs.Save();

        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        int currentSubStage = PlayerPrefs.GetInt("CurrentSubStage", 1);

        Debug.Log($"[GameManager] 현재 스테이지: {currentStage}, 서브 스테이지: {currentSubStage}");

        WaveManager.Instance.LoadWavesForStage(StageManager.Instance.currentStageNum, StageManager.Instance.currentSubStageNum);

        StartCoroutine(GameStartCountdown());
        StartCoroutine(DelayedStart());
    }

    private IEnumerator GameStartCountdown()
    {
        int countdown = 10;

        while (countdown > 0)
        {
            Debug.Log($"[GameManager] 웨이브 시작까지 {countdown}초...");
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        Debug.Log("[GameManager] 웨이브 시작!");
        waveManager.StartWave();
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f);

        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            WaveManager.Instance.LoadWavesForStage(StageManager.Instance.currentStageNum, StageManager.Instance.currentSubStageNum);
            Debug.Log("[GameManager] 웨이브 데이터 로드 완료!");
        }
        else
        {
            Debug.LogError("[GameManager] WaveManager를 찾을 수 없습니다!");
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("[GameManager] 게임 오버! 패배 처리.");
        PopupManager popupManager = FindObjectOfType<PopupManager>();
        if (popupManager != null)
        {
            popupManager.ShowDefeatPopup();
        }
    }

    public void ResetGameManager()
    {
        Debug.Log("[GameManager] 게임 매니저 초기화 실행!");

        StopAllCoroutines();
        isGameOver = false;

        // WaveManager 및 EnemyManager 초기화
        if (waveManager != null)
        {
            waveManager.ResetWaves();
        }
        if (enemyManager == null)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
        }

        Debug.Log("[GameManager] 게임 매니저 초기화 완료.");
    }

    // 특정 씬인지 확인하는 함수
    private bool IsAllowedScene(string sceneName)
    {
        foreach (string allowedScene in allowedScenes)
        {
            if (sceneName == allowedScene)
            {
                return true;
            }
        }
        return false;
    }
}
