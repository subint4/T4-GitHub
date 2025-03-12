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

    public int totalEnemiesPerStage = 0;
    public int currentWaveTotalEnemies = 0;
    public int defeatedEnemies = 0;
    public int currentWaveDefeatedEnemies = 0;

    private List<WaveData> currentWaveDataList = new List<WaveData>(); // 현재 스테이지의 웨이브 데이터 저장

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

    private void Start()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged += LoadWavesForStage;
        }
    }

    public void LoadWavesForStage(int stageNum, int subStageNum)
    {
        Debug.Log($"[WaveManager] LoadWavesForStage() 호출됨: {stageNum}-{subStageNum}");

        if (StageManager.Instance == null)
        {
            Debug.LogError("[WaveManager] StageManager를 찾을 수 없습니다!");
            return;
        }

        List<int> waveIDs = DataManager.Instance.StageDataManager.GetWaveIDsForStage(stageNum, subStageNum);

        if (waveIDs == null || waveIDs.Count == 0)
        {
            Debug.LogError($"[WaveManager] {stageNum}-{subStageNum}의 웨이브 데이터를 찾을 수 없습니다!");
            return;
        }

        currentWaveDataList = DataManager.Instance.WaveDataManager.GetWaveDataList(waveIDs);

        if (currentWaveDataList == null || currentWaveDataList.Count == 0)
        {
            Debug.LogError($"[WaveManager] {stageNum}-{subStageNum}의 웨이브 데이터를 가져오지 못했습니다!");
            return;
        }

        totalEnemiesPerStage = currentWaveDataList.Sum(wave => wave.count);
        Debug.Log($"[WaveManager] {stageNum}-{subStageNum} 스테이지의 총 적 수: {totalEnemiesPerStage}");

        // **현재 로드된 웨이브 데이터 출력**
        Debug.Log($"[WaveManager] {stageNum}-{subStageNum} 스테이지에서 로드된 웨이브 데이터 목록:");

        foreach (var wave in currentWaveDataList)
        {
            Debug.Log($"[CurrentWaveData] Wave: {wave.wave}, EnemyID: {wave.enemyID}, Count: {wave.count}, " +
                      $"SpawnDelay: {wave.SpawnDelay}, SpawnGroup: {wave.SpawnGroup}, Interval: {wave.interval}, " +
                      $"SpawnLaneID: {wave.SpawnLaneID}, StageNum: {wave.stageNum}, SubStageNum: {wave.subStageNum}");
        }
    }



    public void StartWave()
    {
        if (isSpawning || currentWaveDataList.Count == 0 || currentWaveIndex >= currentWaveDataList.Count)
        {
            Debug.LogWarning($"[WaveManager] 웨이브 시작 불가! (isSpawning: {isSpawning}, currentWaveIndex: {currentWaveIndex})");
            return;
        }

        currentWaveDefeatedEnemies = 0;
        activeEnemies.Clear();

        WaveData currentWaveData = currentWaveDataList[currentWaveIndex];

        if (currentWaveData == null)
        {
            Debug.LogError($"[WaveManager] 웨이브 {currentWaveIndex + 1}의 데이터가 유효하지 않음!");
            return;
        }

        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 시작! 적 ID: {currentWaveData.enemyID}, 총 적 수: {currentWaveData.count}");

        currentWaveTotalEnemies = currentWaveData.count;

        StartCoroutine(SpawnWaveCoroutine());
    }



    private IEnumerator SpawnWaveCoroutine()
    {
        isSpawning = true;

        // 현재 웨이브 데이터 가져오기
        WaveData waveData = currentWaveDataList[currentWaveIndex];

        Debug.Log($"[WaveManager] 웨이브 {waveData.wave}에서 EnemyID {waveData.enemyID} 스폰 시작");

        for (int i = 0; i < waveData.count; i++)
        {
            Debug.Log($"[WaveManager] EnemyID {waveData.enemyID} 스폰 - 남은 적 {waveData.count - i}");

            SpawnEnemy(waveData.enemyID);
            yield return new WaitForSeconds(waveData.SpawnDelay);
        }

        isSpawning = false;
    }





    private void SpawnEnemy(int enemyID)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] 스폰 위치가 설정되지 않았습니다!");
            return;
        }

        // 올바른 EnemyID가 전달되는지 확인
        Debug.Log($"[WaveManager] SpawnEnemy 호출됨 - EnemyID: {enemyID}");

        // 적 프리팹 가져오기
        GameObject enemyPrefab = EnemyManager.Instance.GetEnemyPrefab(enemyID);
        if (enemyPrefab == null)
        {
            Debug.LogError($"[WaveManager] 적 프리팹을 찾을 수 없음! ID: {enemyID}");
            return;
        }

        Transform spawnPoint;

        // 보스는 2번 또는 4번 스폰 포인트에서만 소환
        if (enemyPrefab.CompareTag("Boss"))
        {
            List<int> bossSpawnIndexes = new List<int> { 1, 3 }; // 1=2열, 3=4열 (0부터 시작하는 인덱스)
            int randomIndex = bossSpawnIndexes[Random.Range(0, bossSpawnIndexes.Count)];
            spawnPoint = spawnPoints[randomIndex];

            Debug.Log($"[WaveManager] 보스({enemyID})가 {randomIndex + 1}열에서 스폰됨!");
        }
        else
        {
            // 일반 적은 기존 방식대로 랜덤 스폰
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        // 적 생성 및 초기화
        GameObject newEnemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            newEnemy.Initialize(DataManager.Instance.EnemyDataManager.GetEnemyData(enemyID), EnemyType.Melee);
            activeEnemies.Add(newEnemy);
            Debug.Log($"[WaveManager] 적 스폰 완료 - ID {enemyID}, 위치 {spawnPoint.position}");
        }
        else
        {
            Debug.LogError("[WaveManager] 생성된 적에 Enemy 컴포넌트가 없습니다!");
        }
    }


    public void ClearEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy)) return;

        activeEnemies.Remove(enemy);
        currentWaveDefeatedEnemies = Mathf.Clamp(currentWaveDefeatedEnemies + 1, 0, currentWaveTotalEnemies);
        defeatedEnemies = Mathf.Clamp(defeatedEnemies + 1, 0, totalEnemiesPerStage);

        // **게이지 업데이트: 현재 웨이브 진행도 / 전체 적 수 기준으로 업데이트**
        GaugeManager gaugeManager = FindObjectOfType<GaugeManager>();
        if (gaugeManager != null)
        {
            gaugeManager.UpdateGauge(defeatedEnemies, totalEnemiesPerStage);
        }

        Debug.Log($"[WaveManager] 적 처치됨 {currentWaveDefeatedEnemies}/{currentWaveTotalEnemies}, 전체 처치: {defeatedEnemies}/{totalEnemiesPerStage}");

        // **웨이브가 종료되었는지 확인**
        if (currentWaveDefeatedEnemies >= currentWaveTotalEnemies)
        {
            Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 완료! 다음 웨이브 확인 중...");
            isSpawning = false;  // 스폰 중지 신호 추가
            CheckNextWave();
        }
    }

    private void CheckNextWave()
    {
        if (isSpawning)
        {
            Debug.LogWarning("[WaveManager] 웨이브 진행 중인데 CheckNextWave가 호출됨! 실행 중지");
            return;
        }

        if (currentWaveIndex + 1 < currentWaveDataList.Count)
        {
            StopAllCoroutines();  // 현재 실행 중인 모든 코루틴을 멈춘다.
            isSpawning = false;   // 새로운 웨이브 시작 전에 스폰 상태를 확실히 초기화

            float waveInterval = currentWaveDataList[currentWaveIndex].interval;
            Debug.Log($"[WaveManager] 다음 웨이브 {currentWaveIndex + 2} 시작까지 {waveInterval}초 대기...");

            currentWaveIndex++;
            StartCoroutine(StartNextWaveWithDelay(waveInterval));
        }
        else
        {
            Debug.Log("[WaveManager] 모든 웨이브 종료! 게임 클리어 판정.");
            OnAllWavesCompleted();
        }
    }


    private IEnumerator StartNextWaveWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartWave();
    }

    private void OnAllWavesCompleted()
    {
        Debug.Log("[WaveManager] 모든 웨이브가 종료되었습니다! 게임 승리 처리.");
        PopupManager popupManager = FindObjectOfType<PopupManager>();
        if (popupManager != null)
        {
            popupManager.ShowVictoryPopup();
        }
    }

    public void ResetWaves()
    {
        StopAllCoroutines();
        currentWaveIndex = 0;
        defeatedEnemies = 0;
        totalEnemiesPerStage = 0;
        currentWaveTotalEnemies = 0;
        currentWaveDefeatedEnemies = 0;
        activeEnemies.Clear();

        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            Destroy(enemy.gameObject);
        }

        Debug.Log("[WaveManager] 웨이브 데이터 초기화 완료.");
    }
}
