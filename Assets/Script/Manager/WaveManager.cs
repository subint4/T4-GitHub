using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private int currentWaveIndex = 0; // 현재 웨이브 인덱스 (0부터 시작)
    private bool isSpawning = false;
    public Transform[] spawnPoints; // 적 스폰 위치 배열
    private List<Enemy> activeEnemies = new List<Enemy>(); // 현재 존재하는 적 리스트
    private int totalEnemies = 0;
    private int defeatedEnemies = 0;
    public GaugeManager progressBar; // 진행률 게이지 UI
    private List<WaveSO> currentWaveDataList; // 특정 스테이지의 전체 웨이브 리스트

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

    public void InitializeWave(int stageNum, int subStageNum)
    {
        currentWaveDataList = DataManager.Instance.WaveDataManager.GetWaveDataList(stageNum, subStageNum);

        if (currentWaveDataList == null || currentWaveDataList.Count == 0)
        {
            Debug.LogError($"[WaveManager] {stageNum}-{subStageNum} 웨이브 데이터를 찾을 수 없습니다!");
            return;
        }

        currentWaveIndex = 0; // 첫 번째 웨이브부터 시작
        Debug.Log($"[WaveManager] {stageNum}-{subStageNum} 웨이브 데이터 로드 완료!");
    }

    public void StartWave()
    {
        if (isSpawning || currentWaveDataList == null || currentWaveIndex >= currentWaveDataList.Count)
        {
            Debug.LogWarning("[WaveManager] 웨이브 시작 불가: 데이터 없음 또는 모든 웨이브 완료됨.");
            return;
        }

        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        WaveSO currentWaveData = currentWaveDataList[currentWaveIndex]; // 현재 웨이브 가져오기
        if (currentWaveData == null)
        {
            Debug.LogError("[WaveManager] 현재 웨이브 데이터가 없습니다!");
            isSpawning = false;
            yield break;
        }

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
            Debug.Log($"[WaveManager] 적 스폰 완료: {enemyData.Name} (ID: {enemyID}, 위치: {selectedSpawnPoint.position})");
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

            // 게이지바 업데이트
            if (progressBar != null)
            {
                progressBar.UpdateGauge(defeatedEnemies, totalEnemies);
            }

            Debug.Log($"[WaveManager] 적 처치 진행률: {defeatedEnemies} / {totalEnemies}");

            if (defeatedEnemies >= totalEnemies)
            {
                StageClear();
            }
        }
    }

    private void StageClear()
    {
        Debug.Log("[WaveManager] 웨이브 클리어!");
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
