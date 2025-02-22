using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    private int currentWaveIndex = 1;
    private bool isSpawning = false;
    public Transform[] spawnPoints; // 스폰 위치 배열
    private List<Enemy> activeEnemies = new List<Enemy>(); // 스폰된 적 리스트
    private int totalEnemies = 0;
    private int defeatedEnemies = 0;
    public GaugeManager progressBar; // 진행률 게이지 UI

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

    public void StartWave()
    {
        if (isSpawning) return;
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        WaveSO currentWave = DataManager.Instance.WaveDataManager.GetWaveData(currentWaveIndex);
        if (currentWave == null)
        {
            Debug.Log("[WaveManager] 모든 웨이브 완료!");
            isSpawning = false;
            yield break;
        }

        Debug.Log($"[WaveManager] 웨이브 {currentWave.wave} 시작!");

        totalEnemies = 0;
        defeatedEnemies = 0;
        activeEnemies.Clear();

        foreach (var spawnData in currentWave.spawnDataList)
        {
            totalEnemies += spawnData.count;

            for (int i = 0; i < spawnData.count; i++)
            {
                SpawnEnemy(spawnData.enemyID);
                yield return new WaitForSeconds(spawnData.SpawnDelay);
            }
        }

        yield return new WaitForSeconds(currentWave.interval);

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
        Debug.Log("[WaveManager] 스테이지 클리어!");
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentWaveIndex++;
        StartWave();
    }
}
