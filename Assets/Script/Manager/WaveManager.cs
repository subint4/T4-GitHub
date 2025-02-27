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

    public void StartWave()
    {
        if (isSpawning || currentWaveDataList.Count == 0 || currentWaveIndex >= currentWaveDataList.Count)
        {
            Debug.LogWarning("[WaveManager] 웨이브 시작 불가: 데이터 없음 또는 모든 웨이브 완료됨.");
            return;
        }

        defeatedEnemies = 0;
        totalEnemies = 0;
        activeEnemies.Clear();

        // 현재 웨이브에서 소환될 적의 총 개수를 다시 계산
        foreach (var spawnData in currentWaveDataList[currentWaveIndex].spawnDataList)
        {
            totalEnemies += spawnData.count;
        }

        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 시작! 총 적 수: {totalEnemies}");
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        WaveSO currentWaveData = currentWaveDataList[currentWaveIndex];

        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 시작!");

        foreach (var spawnData in currentWaveData.spawnDataList)
        {
            for (int i = 0; i < spawnData.count; i++)
            {
                SpawnEnemy(spawnData.enemyID);
                yield return new WaitForSeconds(spawnData.SpawnDelay);
            }
        }

        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 적 생성 완료! 활성 적 수: {activeEnemies.Count}");
        isSpawning = false;
    }

    private void SpawnEnemy(int enemyID)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] 스폰 위치가 설정되지 않았습니다!");
            return;
        }

        Transform selectedSpawnPoint;

        if (enemyID % 4 == 0)
        {
            List<Transform> validSpawnPoints = new List<Transform>();
            if (spawnPoints.Length >= 2) validSpawnPoints.Add(spawnPoints[1]);
            if (spawnPoints.Length >= 4) validSpawnPoints.Add(spawnPoints[3]);

            selectedSpawnPoint = validSpawnPoints.Count > 0
                ? validSpawnPoints[Random.Range(0, validSpawnPoints.Count)]
                : spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        else
        {
            selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        GameObject enemyPrefab = EnemyManager.Instance.GetEnemyPrefab(enemyID);
        if (enemyPrefab == null)
        {
            Debug.LogError($"[WaveManager] 적 프리팹을 찾을 수 없음! ID: {enemyID}");
            return;
        }

        GameObject newEnemyObj = Instantiate(enemyPrefab, selectedSpawnPoint.position, Quaternion.identity);
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            newEnemy.Initialize(DataManager.Instance.EnemyDataManager.GetEnemyData(enemyID), EnemyType.Melee);
            activeEnemies.Add(newEnemy);
            Debug.Log($"[WaveManager] 적 스폰 완료: ID {enemyID} 위치 {selectedSpawnPoint.position}, 현재 활성 적 수: {activeEnemies.Count}");
        }
        else
        {
            Debug.LogError($"[WaveManager] 생성된 적에 Enemy 컴포넌트가 없습니다!");
        }
    }

    public void OnEnemyDefeated(Enemy enemy)
    {
        if (enemy == null) return;

        defeatedEnemies++;
        RemoveActiveEnemy(enemy);

        Debug.Log($"[WaveManager] 적 처치 진행률: {defeatedEnemies} / {totalEnemies}");

        if (progressBar != null)
        {
            progressBar.UpdateGauge(defeatedEnemies, totalEnemies);
        }

        if (activeEnemies.Count == 0 && defeatedEnemies >= totalEnemies)
        {
            Debug.Log("[WaveManager] 모든 적 처치 완료! OnAllEnemiesDefeated() 호출됨.");
            OnAllEnemiesDefeated();
        }
    }

    public void RemoveActiveEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"[WaveManager] 적 제거됨: {enemy.gameObject.name}, 남은 적: {activeEnemies.Count}");
        }

        if (activeEnemies.Count == 0 && defeatedEnemies >= totalEnemies)
        {
            Debug.Log("[WaveManager] 모든 적 처치 완료! OnAllEnemiesDefeated() 호출됨.");
            OnAllEnemiesDefeated();
        }
    }

    private void OnAllEnemiesDefeated()
    {
        Debug.Log("[WaveManager] 현재 웨이브의 모든 적이 처치됨!");

        if (currentWaveIndex >= currentWaveDataList.Count - 1)
        {
            Debug.Log("[WaveManager] 모든 웨이브 완료! 스테이지 클리어!");
            PopupManager.Instance.ShowVictoryPopup();
        }
        else
        {
            Debug.Log($"[WaveManager] 다음 웨이브 {currentWaveIndex + 1} 시작!");
            StartNextWave();
        }
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
            Debug.Log("[WaveManager] 모든 웨이브가 완료되었습니다.");
        }
    }
}
