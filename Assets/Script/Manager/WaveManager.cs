using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    public Transform[] spawnPoints;
    private List<Enemy> activeEnemies = new List<Enemy>();
    public int totalEnemies = 0;
    public int defeatedEnemies = 0;
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
            return;
        }
    }

    public void LoadWavesForStage()
    {
        if (StageManager.Instance == null)
        {
            Debug.LogError("[WaveManager] StageManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        DataManager.Instance.WaveDataManager.LoadWaveData();

        int stageNum = StageManager.Instance.currentStageNum;
        int subStageNum = StageManager.Instance.GetCurrentSubStageNum();

        Debug.Log($"[WaveManager] {stageNum}-{subStageNum} 스테이지 데이터 로드 시도 중...");

        List<int> waveIDs = DataManager.Instance.StageDataManager.GetWaveIDsForStage(stageNum, subStageNum);
        Debug.Log($"[WaveManager] 로드된 웨이브 ID 목록: {string.Join(", ", waveIDs)}");

        if (waveIDs == null || waveIDs.Count == 0)
        {
            Debug.LogError($"[WaveManager] {stageNum}-{subStageNum}에 대한 웨이브 데이터가 없음!");
            return;
        }

        currentWaveDataList = DataManager.Instance.WaveDataManager.GetWaveDataList(waveIDs);

        if (currentWaveDataList == null || currentWaveDataList.Count == 0)
        {
            Debug.LogError($"[WaveManager] {stageNum}-{subStageNum}의 웨이브 데이터를 가져오지 못했습니다!");
            return;
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

        WaveSO currentWaveData = currentWaveDataList[currentWaveIndex];

        if (currentWaveData == null || currentWaveData.spawnDataList == null || currentWaveData.spawnDataList.Count == 0)
        {
            Debug.LogError("[WaveManager] 웨이브 데이터가 비어 있음!");
            return;
        }

        foreach (var spawnData in currentWaveData.spawnDataList)
        {
            totalEnemies += spawnData.count;
        }

        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 시작! 총 적 수: {totalEnemies}");

        StartCoroutine(SpawnWave());
    }

    public void ResetWaves()
    {
        Debug.Log("[WaveManager] 웨이브 초기화 실행!");
        StopAllCoroutines();

        currentWaveIndex = 0;
        defeatedEnemies = 0;
        totalEnemies = 0;
        activeEnemies.Clear();

        Debug.Log("[WaveManager] 웨이브 데이터 초기화 완료.");
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

        isSpawning = false;
        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 적 생성 완료! 활성 적 수: {activeEnemies.Count}");
    }

    private void SpawnEnemy(int enemyID)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] 스폰 위치가 설정되지 않았습니다!");
            return;
        }

        Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
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
            Debug.LogError("[WaveManager] 생성된 적에 Enemy 컴포넌트가 없습니다!");
        }
    }

    public void Clear(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"[WaveManager] {enemy.name} 제거됨. 남은 활성 적 수: {activeEnemies.Count}");
        }

        defeatedEnemies++;
        Debug.Log($"[WaveManager] 적 처치됨 {defeatedEnemies}/{totalEnemies}");

        // `GaugeManager` 직접 참조하여 업데이트
        GaugeManager gaugeManager = FindObjectOfType<GaugeManager>();
        if (gaugeManager != null)
        {
            gaugeManager.UpdateGauge(defeatedEnemies, totalEnemies);
        }
        else
        {
            Debug.LogWarning("[WaveManager] GaugeManager를 찾을 수 없음. 게이지 업데이트 생략.");
        }

        if (defeatedEnemies >= totalEnemies)
        {
            Debug.Log("[WaveManager] 모든 적 처치 완료! 다음 웨이브 확인 중...");
            CheckNextWave();
        }
    }




    private void CheckNextWave()
    {
        if (currentWaveIndex + 1 < currentWaveDataList.Count)
        {
            Debug.Log("[WaveManager] 다음 웨이브 시작 준비 중...");
            currentWaveIndex++;
            StartWave();
        }
        else
        {
            Debug.Log("[WaveManager] 모든 웨이브 종료! 스테이지 완료 처리.");
            OnAllWavesCompleted();
        }
    }

    private void OnAllWavesCompleted()
    {
        Debug.Log("[WaveManager] 모든 웨이브가 종료되었습니다! 게임 승리 처리.");

        PopupManager popupManager = FindObjectOfType<PopupManager>();
        if (popupManager != null)
        {
            popupManager.ShowVictoryPopup();
        }
        // 추가적인 게임 클리어 로직 가능 (보상 지급, 씬 변경 등)
    }
}

