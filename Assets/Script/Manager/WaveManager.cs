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
    /// 스테이지에 맞는 웨이브 데이터 불러오기
    /// </summary>
    public void LoadWavesForStage(int stageNum, int subStageNum)
    {
        List<int> waveIDs = DataManager.Instance.StageDataManager.GetWaveIDsForStage(stageNum, subStageNum);
        currentWaveDataList.Clear();

        Debug.Log($"[WaveManager] {stageNum}-{subStageNum}에 대한 웨이브 데이터 로드 시작... (waveIDs: {string.Join(",", waveIDs)})");

        foreach (var waveID in waveIDs)
        {
            WaveSO waveData = DataManager.Instance.WaveDataManager.GetWaveData(waveID);
            if (waveData != null)
            {
                currentWaveDataList.Add(waveData);
            }
            else
            {
                Debug.LogError($"[WaveManager] 웨이브 {waveID} 데이터를 찾을 수 없습니다!");
            }
        }

        Debug.Log($"[WaveManager] {stageNum}-{subStageNum}에 대한 {currentWaveDataList.Count}개의 웨이브 로드 완료!");
    }

    /// <summary>
    /// 현재 웨이브 데이터 초기화
    /// </summary>
    public void ResetWaves()
    {
        currentWaveIndex = 0;
        isSpawning = false;
        totalEnemies = 0;
        defeatedEnemies = 0;
        activeEnemies.Clear();
        currentWaveDataList.Clear();
    }

    public void StartWave()
    {
        if (isSpawning || currentWaveDataList.Count == 0 || currentWaveIndex >= currentWaveDataList.Count)
        {
            Debug.LogWarning("[WaveManager] 웨이브 시작 불가: 데이터 없음 또는 모든 웨이브 완료됨.");
            return;
        }

        StartCoroutine(SpawnWave());
    }

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

        yield return new WaitForSeconds(currentWaveData.interval);
        isSpawning = false;

        if (activeEnemies.Count == 0)
        {
            StartNextWave();
        }
    }

    private void SpawnEnemy(int enemyID)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] 스폰 위치가 설정되지 않았습니다!");
            return;
        }

        Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        EnemySO enemyData = DataManager.Instance.EnemyDataManager.GetEnemyData(enemyID);
        if (enemyData == null)
        {
            Debug.LogError($"[WaveManager] Enemy ID {enemyID}에 대한 데이터를 찾을 수 없습니다!");
            return;
        }

        GameObject enemyPrefab = EnemyManager.Instance.GetEnemyPrefab(enemyID);
        if (enemyPrefab == null)
        {
            Debug.LogError($"[WaveManager] Enemy ID {enemyID}에 대한 프리팹을 찾을 수 없습니다!");
            return;
        }

        GameObject newEnemyObj = Instantiate(enemyPrefab, selectedSpawnPoint.position, Quaternion.identity);
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            newEnemy.Initialize(enemyData, enemyData.Type);
            activeEnemies.Add(newEnemy);
        }
        else
        {
            Debug.LogError("[WaveManager] 생성된 적에 Enemy 컴포넌트가 없습니다!");
            Destroy(newEnemyObj);
        }
    }

    public void OnEnemyDefeated(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            defeatedEnemies++;

            // 게이지 업데이트
            if (progressBar != null)
            {
                progressBar.UpdateGauge(defeatedEnemies, totalEnemies);
            }

            Debug.Log($"[WaveManager] 적 처치 진행률: {defeatedEnemies} / {totalEnemies}");

            // 모든 적 처치 시 다음 웨이브 시작
            if (defeatedEnemies >= totalEnemies)
            {
                OnAllEnemiesDefeated();
            }
        }
    }

    /// <summary>
    /// 모든 적이 처치되었을 때 다음 웨이브 실행
    /// </summary>
    private void OnAllEnemiesDefeated()
    {
        Debug.Log("[WaveManager] 현재 웨이브의 모든 적이 처치됨!");
        StartNextWave();
    }

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
