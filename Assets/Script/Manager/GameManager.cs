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
        if (WaveManager.Instance == null)
        {
            Debug.LogError("[GameManager] WaveManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        // 현재 선택된 Stage 및 SubStage 정보 가져오기
        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        int currentSubStage = PlayerPrefs.GetInt("CurrentSubStage", 1);

        Debug.Log($"[GameManager] 현재 스테이지: {currentStage}, 서브 스테이지: {currentSubStage}");

        // 스테이지 데이터 가져오기
        StageData stageData = DataManager.Instance.StageDataManager.GetStageData(currentStage, currentSubStage);
        if (stageData != null)
        {
            StageManager.Instance.SetStageData(stageData);
        }
        else
        {
            Debug.LogError("[GameManager] 유효한 스테이지 데이터를 찾을 수 없습니다!");
        }

        // 웨이브 데이터 불러오기 (현재 스테이지와 서브 스테이지 기반)
        WaveManager.Instance.LoadWavesForStage();

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
        Debug.Log("[GameManager] 게임 오버! 패배 처리.");
        PopupManager.Instance.ShowDefeatPopup(); // 패배 팝업 표시
    }

}
