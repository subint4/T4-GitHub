using UnityEngine;
using UnityEngine.SceneManagement; // 씬 리로드를 위해 추가
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public WaveManager waveManager;
    public EnemyManager enemyManager;

    private bool isGameOver = false;

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

        // **자동으로 WaveManager와 EnemyManager 찾기**
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }
        if (enemyManager == null)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
        }
    }

    private void Start()
    {
        if (waveManager == null || enemyManager == null)
        {
            Debug.LogError("[GameManager] WaveManager 또는 EnemyManager를 찾을 수 없습니다! 씬에 존재하는지 확인하세요.");
            return;
        }

        // **현재 스테이지 정보를 PlayerPrefs에서 가져옴**
        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        int currentSubStage = PlayerPrefs.GetInt("CurrentSubStage", 1);

        Debug.Log($"[GameManager] 현재 스테이지: {currentStage}, 서브 스테이지: {currentSubStage}");

        // **웨이브 데이터 로드**
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.LoadWavesForStage(currentStage, currentSubStage);
        }
        else
        {
            Debug.LogError("[GameManager] WaveManager 인스턴스를 찾을 수 없습니다!");
        }

        Debug.Log("[GameManager] 게임 시작! 10초 후 웨이브 시작...");
        StartCoroutine(GameStartCountdown());
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
        waveManager.StartWave(); // 10초 후 웨이브 시작
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("[GameManager] 게임 오버! 화면을 클릭하면 다시 시작됩니다.");

        // **게임 멈추기**
        Time.timeScale = 0f; // 게임 전체 정지
    }

    private void Update()
    {
        if (isGameOver && Input.GetMouseButtonDown(0)) // 클릭 감지
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        Debug.Log("[GameManager] 게임 다시 시작!");

        // DOTween 정리
        DG.Tweening.DOTween.KillAll();

        // **시간 복구**
        Time.timeScale = 1f;

        // **씬 다시 로드**
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
