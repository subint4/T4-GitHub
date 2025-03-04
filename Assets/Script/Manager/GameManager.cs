using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public WaveManager waveManager;
    public EnemyManager enemyManager;

    private bool isGameOver = false;

    private void Awake()
    {
        // `DontDestroyOnLoad()` 제거: 씬이 전환될 때 `GameManager`를 새로 생성
        if (Instance == null)
        {
            Instance = this;
            // 씬 전환 시 새로 생성되기 때문에 DontDestroyOnLoad()를 호출하지 않음
        }
        else
        {
            // 다른 씬에 남아 있는 GameManager 인스턴스가 있다면 파괴
            Destroy(gameObject);
            return;
        }

        // 자동으로 WaveManager와 EnemyManager 찾기
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }
        if (enemyManager == null)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
        }

        // 씬이 로드될 때마다 GameManager 인스턴스를 확인하여, 중복된 인스턴스는 삭제하도록 설정
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] 씬 로드됨: {scene.name}");

        // 씬 로드 후에 GameManager가 싱글톤 인스턴스로만 존재하도록 함
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (WaveManager.Instance == null)
        {
            Debug.LogError("[GameManager] WaveManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        // PlayerPrefs에서 저장된 진행도 초기화
        PlayerPrefs.SetInt("WaveProgress", 0);
        PlayerPrefs.Save();

        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        int currentSubStage = PlayerPrefs.GetInt("CurrentSubStage", 1);

        Debug.Log($"[GameManager] 현재 스테이지: {currentStage}, 서브 스테이지: {currentSubStage}");

        // 웨이브 데이터 리로드
        WaveManager.Instance.LoadWavesForStage();

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
        waveManager.StartWave(); // 10초 후 웨이브 시작
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f); // 0.2초 대기

        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.LoadWavesForStage();
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
        PopupManager.Instance.ShowDefeatPopup(); // 패배 팝업 표시
    }
}
