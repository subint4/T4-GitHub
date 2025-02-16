using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    private int currentWaveIndex = 1;
    private bool isSpawning = false;

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

        WaveSO currentWave = DataManager.GetWaveData(currentWaveIndex);
        if (currentWave == null)
        {
            Debug.Log("모든 웨이브 완료!");
            isSpawning = false;
            yield break;
        }

        Debug.Log($"웨이브 {currentWave.wave} 시작!");

        foreach (var spawnData in currentWave.spawnDataList)
        {
            int enemyID = spawnData.enemyID;
            int spawnCount = spawnData.count;
            float spawnDelay = Mathf.Max(spawnData.SpawnDelay, 0.1f); // 최소 딜레이 보장

            for (int i = 0; i < spawnCount; i++)
            {
                GameObject enemy = EnemyManager.Instance.SpawnEnemy(enemyID);
                yield return new WaitForSeconds(spawnDelay); // 개별 적 간격을 유지
            }
        }

        yield return new WaitForSeconds(currentWave.interval); // 웨이브 간 대기 시간

        currentWaveIndex++;
        isSpawning = false;
    }
}
