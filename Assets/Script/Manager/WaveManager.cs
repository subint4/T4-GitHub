using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

        WaveSO currentWave = DataManager.Instance.Wave.GetWaveData(currentWaveIndex);
        if (currentWave == null)
        {
            Debug.Log("모든 웨이브 완료!");
            isSpawning = false;
            yield break;
        }

        Debug.Log($"웨이브 {currentWave.wave} 시작!");

        foreach (var spawnData in currentWave.spawnDataList)
        {
            for (int i = 0; i < spawnData.count; i++)
            {
                EnemyManager.Instance.SpawnEnemy(spawnData.enemyID);
                yield return new WaitForSeconds(spawnData.SpawnDelay);
            }
        }

        yield return new WaitForSeconds(currentWave.interval);

        currentWaveIndex++;
        isSpawning = false;
        StartWave();
    }
}
