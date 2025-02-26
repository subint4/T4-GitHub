using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    public Transform[] spawnPoints;
    private List<Enemy> activeEnemies = new List<Enemy>();
    private int totalEnemies = 0;
    private int defeatedEnemies = 0;
    public GaugeManager progressBar;
    private List<WaveSO> currentWaveDataList = new List<WaveSO>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 현재 스테이지의 웨이브 데이터 불러오기
    /// </summary>
    public void LoadWavesForStage()
    {
        if (StageManager.Instance == null)
        {
            Debug.LogError("[WaveManager] StageManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        int stageNum = StageManager.Instance.currentStageNum;
        int subStageNum = StageManager.Instance.GetCurrentSubStageNum();

        List<int> waveIDs = DataManager.Instance.StageDataManager.GetWaveIDsForStage(stageNum, subStageNum);
        currentWaveDataList = DataManager.Instance.WaveDataManager.GetWaveDataList(waveIDs);

        if (currentWaveDataList.Count == 0)
        {
            Debug.LogError($"[WaveManager] {stageNum}-{subStageNum}의 웨이브 데이터가 없습니다!");
        }
        else
        {
            Debug.Log($"[WaveManager] {stageNum}-{subStageNum}에 대한 {currentWaveDataList.Count}개의 웨이브 로드 완료!");
        }
    }

    /// <summary>
    /// 웨이브 시작
    /// </summary>
    public void StartWave()
    {
        if (isSpawning || currentWaveDataList.Count == 0 || currentWaveIndex >= currentWaveDataList.Count)
        {
            Debug.LogWarning("[WaveManager] 웨이브 시작 불가: 데이터 없음 또는 모든 웨이브 완료됨.");
            return;
        }

        StartCoroutine(SpawnWave());
    }

    /// <summary>
    /// 웨이브의 적 생성
    /// </summary>
    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        WaveSO currentWaveData = currentWaveDataList[currentWaveIndex];

        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 시작!");

        totalEnemies = 0;
        defeatedEnemies = 0;
        activeEnemies.Clear();

        foreach (var spawnData in currentWaveData.spawnDataList)
        {
            totalEnemies += spawnData.count;

            for (int i = 0; i < spawnData.count; i++)
            {
                SpawnEnemy(spawnData.enemyID);
                yield return new WaitForSeconds(spawnData.SpawnDelay);
            }
        }

        // 게이지 초기화
        if (progressBar != null)
        {
            progressBar.UpdateGauge(0, totalEnemies);
        }

        yield return new WaitForSeconds(currentWaveData.interval);
        isSpawning = false;

        if (activeEnemies.Count == 0)
        {
            StartNextWave();
        }
    }

    /// <summary>
    /// 특정 적을 스폰
    /// </summary>
    private void SpawnEnemy(int enemyID)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] 스폰 위치가 설정되지 않았습니다!");
            return;
        }

        Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        EnemyManager.Instance.SpawnEnemy(enemyID, selectedSpawnPoint);
    }

    /// <summary>
    /// 적 처치 처리
    /// </summary>
    public void OnEnemyDefeated()
    {
        defeatedEnemies++;

        // 게이지 업데이트
        if (progressBar != null)
        {
            progressBar.UpdateGauge(defeatedEnemies, totalEnemies);
        }

        Debug.Log($"[WaveManager] 적 처치 진행률: {defeatedEnemies} / {totalEnemies}");

        // 모든 적이 처치되었을 때 다음 웨이브 시작
        if (defeatedEnemies >= totalEnemies)
        {
            OnAllEnemiesDefeated();
        }
    }

    /// <summary>
    /// 모든 적 처치 시 다음 웨이브 처리
    /// </summary>
    private void OnAllEnemiesDefeated()
    {
        Debug.Log("[WaveManager] 현재 웨이브의 모든 적이 처치됨!");

        if (currentWaveIndex >= currentWaveDataList.Count - 1)
        {
            Debug.Log("[WaveManager] 모든 웨이브 완료! 승리!");
            PopupManager.Instance.ShowVictoryPopup(); // 게임 승리 처리
        }
        else
        {
            StartNextWave();
        }
    }

    /// <summary>
    /// 다음 웨이브 시작
    /// </summary>
    private void StartNextWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex < currentWaveDataList.Count)
        {
            Debug.Log($"[WaveManager] 다음 웨이브 {currentWaveIndex + 1} 시작!");
            StartWave();
        }
        else
        {
            Debug.Log("[WaveManager] 모든 웨이브 완료!");
        }
    }
}
