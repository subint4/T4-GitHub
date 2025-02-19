using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    private Dictionary<int, WaveSO> waveDataDictionary = new Dictionary<int, WaveSO>();
    private int currentWaveIndex = 1;
    private bool isSpawning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadWaveData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadWaveData()
    {
        WaveSO[] waveDataList = Resources.LoadAll<WaveSO>("WaveSO");
        foreach (var wave in waveDataList)
        {
            if (wave != null)
            {
                waveDataDictionary[wave.ID] = wave;
            }
        }
    }

    public WaveSO GetWaveData(int id)
    {
        return waveDataDictionary.TryGetValue(id, out var data) ? data : null;
    }

    public void StartWave()
    {
        if (isSpawning) return;
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        WaveSO currentWave = GetWaveData(currentWaveIndex);
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
