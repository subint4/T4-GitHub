using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] private Transform[] spawnRows;
    private int currentWaveIndex = 1;
    private bool isSpawning = false;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private float waveStartCountdown = 10f; // 게임 시작 전 카운트다운 시간
    private float waveIntervalCountdown = 5f; // 웨이브 간 대기시간

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

    public static WaveManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("WaveManager");
            Instance = obj.AddComponent<WaveManager>();
            DontDestroyOnLoad(obj);
        }
        return Instance;
    }

    /// <summary>
    /// 게임 시작 전 카운트다운 실행 (GameManager에서 호출)
    /// </summary>
    public void StartGameCountdown()
    {
        StartCoroutine(GameStartCountdownCoroutine());
    }

    private IEnumerator GameStartCountdownCoroutine()
    {
        while (waveStartCountdown > 0)
        {
            Debug.Log($"게임 시작까지 {waveStartCountdown:F0}초...");
            yield return new WaitForSeconds(1f);
            waveStartCountdown--;
        }

        Debug.Log("웨이브 시작!");
        StartWave(); // 웨이브 시작
    }

    /// <summary>
    /// 웨이브 시작
    /// </summary>
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
            yield break;
        }

        Debug.Log($"웨이브 {currentWave.wave} 시작! 적 수: {currentWave.spawnDataList.Count}");

        foreach (var spawnData in currentWave.spawnDataList) // 기존 EnemyCounts → spawnDataList 사용
        {
            int enemyID = spawnData.enemyID;
            int spawnCount = spawnData.count;

            for (int i = 0; i < spawnCount; i++)
            {
                GameObject enemy = EnemyManager.Instance.SpawnEnemy(enemyID, GetRandomSpawnPoint());
                if (enemy != null)
                {
                    activeEnemies.Add(enemy);
                }
                yield return new WaitForSeconds(spawnData.SpawnDelay); // 개별 딜레이 적용
            }
        }

        yield return new WaitUntil(() => activeEnemies.Count == 0);
        isSpawning = false;

        yield return StartCoroutine(WaveIntervalCountdown());

        if (DataManager.GetWaveData(currentWaveIndex + 1) != null)
        {
            currentWaveIndex++;
            StartWave();
        }
        else
        {
            Debug.Log("마지막 웨이브 완료!");
        }
    }

    private IEnumerator WaveIntervalCountdown()
    {
        float waveInterval = 5f;
        waveIntervalCountdown = waveInterval;

        while (waveIntervalCountdown > 0)
        {
            Debug.Log($"다음 웨이브 시작까지 {waveIntervalCountdown:F0}초...");
            yield return new WaitForSeconds(1f);
            waveIntervalCountdown--;
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnRows.Length > 0 ? spawnRows[Random.Range(0, spawnRows.Length)] : null;
    }
}
