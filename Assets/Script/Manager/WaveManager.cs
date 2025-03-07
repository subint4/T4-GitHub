using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{

    public static WaveManager Instance { get; private set; }

    private int currentWaveIndex = 0; // **JSON이 0부터 시작하면 0, 1부터 시작하면 1로 변경**
    private bool isSpawning = false;
    public Transform[] spawnPoints;
    private List<Enemy> activeEnemies = new List<Enemy>();

    public int totalEnemiesPerStage = 0;
    public int currentWaveTotalEnemies = 0;
    public int defeatedEnemies = 0;
    public int currentWaveDefeatedEnemies = 0;

    private List<WaveData> allWaveDataList = new List<WaveData>(); // **모든 웨이브 데이터 로드**
    private List<WaveData> currentWaveDataList = new List<WaveData>(); // **현재 웨이브 데이터만 저장**


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
        Debug.Log("[WaveManager] LoadWavesForStage() 호출됨");

        if (StageManager.Instance == null)
        {
            Debug.LogError("[WaveManager] StageManager를 찾을 수 없습니다!");
            return;
        }

        DataManager.Instance.WaveDataManager.LoadWaveDataFromJSON();

        int stageNum = StageManager.Instance.currentStageNum;
        int subStageNum = StageManager.Instance.currentSubStageNum;

        // 현재 스테이지에 해당하는 웨이브 ID 가져오기
        List<int> waveIDs = DataManager.Instance.StageDataManager.GetWaveIDsForStage(stageNum, subStageNum);

        if (waveIDs == null || waveIDs.Count == 0)
        {
            Debug.LogError($"[WaveManager] {stageNum}-{subStageNum}의 웨이브 데이터를 찾을 수 없습니다!");
            return;
        }

        // `GetWaveDataList()` 사용하여 웨이브 데이터를 가져오기
        currentWaveDataList = DataManager.Instance.WaveDataManager.GetWaveDataList(waveIDs);

        if (currentWaveDataList == null || currentWaveDataList.Count == 0)
        {
            Debug.LogError($"[WaveManager] {stageNum}-{subStageNum}의 웨이브 데이터를 가져오지 못했습니다!");
            return;
        }

        totalEnemiesPerStage = 0;
        foreach (var wave in currentWaveDataList)
        {
            totalEnemiesPerStage += wave.count;
        }
        Debug.Log($"[WaveManager] {stageNum}-{subStageNum} 스테이지의 총 적 수: {totalEnemiesPerStage}");
    }


    private void LoadCurrentWave()
    {
        currentWaveDataList = allWaveDataList.FindAll(wave => wave.wave == currentWaveIndex + 1); // **현재 웨이브 데이터 필터링**

        if (currentWaveDataList == null || currentWaveDataList.Count == 0)
        {
            Debug.LogError($"[WaveManager] 웨이브 {currentWaveIndex + 1} 데이터를 찾을 수 없습니다!");
            return;
        }

        currentWaveTotalEnemies = 0;
        foreach (var wave in currentWaveDataList)
        {
            currentWaveTotalEnemies += wave.count;
        }

        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 로드 완료! 총 적 수: {currentWaveTotalEnemies}");
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

        currentWaveTotalEnemies = currentWaveData.count;
        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 시작! 총 적 수: {currentWaveTotalEnemies}");

        StartCoroutine(SpawnEnemyCoroutine(currentWaveData.enemyID, currentWaveData.count, currentWaveData.SpawnDelay));
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        foreach (var waveData in currentWaveDataList)
        {
            if (waveData == null)
            {
                Debug.LogError("[WaveManager] waveData가 null입니다!");
                continue;
            }

            for (int j = 0; j < waveData.count; j++)
            {
                SpawnEnemy(waveData.enemyID);
                yield return new WaitForSeconds(waveData.SpawnDelay);
            }
        }

        isSpawning = false;
        Debug.Log("[WaveManager] 웨이브 스폰 완료!");
    }
    private IEnumerator SpawnEnemies(WaveData waveData)
    {
        isSpawning = true;
        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 적 스폰 시작!");

        if (waveData == null)
        {
            Debug.LogError("[WaveManager] waveData가 null입니다!");
            isSpawning = false;  // 웨이브 종료를 위해 false 설정
            yield break;
        }

        if (waveData.count <= 0)
        {
            Debug.LogError($"[WaveManager] 웨이브 {currentWaveIndex + 1}의 적 수가 0! 즉시 종료됨.");
            isSpawning = false;  // 웨이브 종료
            CheckNextWave();
            yield break;
        }

        for (int i = 0; i < waveData.count; i++)
        {
            Debug.Log($"[WaveManager] 적 소환 시도 - ID: {waveData.enemyID}");
            SpawnEnemy(waveData.enemyID);
            yield return new WaitForSeconds(waveData.SpawnDelay);
        }

        isSpawning = false;
        Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 적 생성 완료! 활성 적 수: {activeEnemies.Count}");
    }

    private void SpawnEnemy(int enemyID)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] 스폰 위치가 설정되지 않았습니다!");
            return;
        }

        // 적 프리팹 가져오기
        GameObject enemyPrefab = EnemyManager.Instance.GetEnemyPrefab(enemyID);
        if (enemyPrefab == null)
        {
            Debug.LogError($"[WaveManager] 적 프리팹을 찾을 수 없음! ID: {enemyID}");
            return;
        }

        Transform selectedSpawnPoint;

        // 유니티 `Tag`를 사용하여 보스인지 판별
        if (enemyPrefab.CompareTag("Boss"))
        {
            // 보스는 2번 또는 4번 스폰 포인트에서만 소환
            List<int> bossSpawnIndexes = new List<int> { 1, 3 }; // 0부터 시작하는 인덱스 (1=2열, 3=4열)
            int randomIndex = bossSpawnIndexes[Random.Range(0, bossSpawnIndexes.Count)];
            selectedSpawnPoint = spawnPoints[randomIndex];
            Debug.Log($"[WaveManager] 보스({enemyID})가 {randomIndex + 1}열에서 스폰됨!");
        }
        else
        {
            // 일반 적은 랜덤 스폰
            selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        // 적 생성 및 초기화
        GameObject newEnemyObj = Instantiate(enemyPrefab, selectedSpawnPoint.position, Quaternion.identity);
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            newEnemy.Initialize(DataManager.Instance.EnemyDataManager.GetEnemyData(enemyID), EnemyType.Melee);
            activeEnemies.Add(newEnemy);
            Debug.Log($"[WaveManager] 적 스폰 완료 - ID {enemyID}, 위치 {selectedSpawnPoint.position}");
        }
        else
        {
            Debug.LogError("[WaveManager] 생성된 적에 Enemy 컴포넌트가 없습니다!");
        }
    }
    private IEnumerator SpawnEnemyCoroutine(int enemyID, int count, float delay)
    {
        for (int j = 0; j < count; j++)
        {
            SpawnEnemy(enemyID);
            yield return new WaitForSeconds(delay);
        }
    }


    public void ResetWaves()
    {
        Debug.Log("[WaveManager] 웨이브 초기화 실행!");

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

        GaugeManager gaugeManager = FindObjectOfType<GaugeManager>();
        if (gaugeManager != null)
        {
            gaugeManager.UpdateGauge(0, 1);
        }

        Debug.Log("[WaveManager] 웨이브 데이터 초기화 완료.");
    }

    public void Clear(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        currentWaveDefeatedEnemies++;
        defeatedEnemies++;

        Debug.Log($"[WaveManager] 적 처치됨 {currentWaveDefeatedEnemies}/{currentWaveTotalEnemies}, 전체 처치: {defeatedEnemies}/{totalEnemiesPerStage}");

        GaugeManager gaugeManager = FindObjectOfType<GaugeManager>();
        if (gaugeManager != null)
        {
            gaugeManager.UpdateGauge(defeatedEnemies, totalEnemiesPerStage);
        }

        // **현재 웨이브의 모든 적이 처치되었는지 확인**
        if (currentWaveDefeatedEnemies >= currentWaveTotalEnemies)
        {
            Debug.Log($"[WaveManager] 웨이브 {currentWaveIndex + 1} 완료! 다음 웨이브 확인 중...");

            // **isSpawning을 false로 설정하여 다음 웨이브 시작 가능하도록 변경**
            isSpawning = false;
            CheckNextWave();
        }

        // **전체 스테이지의 적이 모두 처치되었는지 확인**
        if (defeatedEnemies >= totalEnemiesPerStage)
        {
            Debug.Log("[WaveManager] 모든 웨이브의 적 처치 완료! 게임 승리 판정.");
            OnAllWavesCompleted();
        }
    }


    private void CheckNextWave()
    {
        Debug.Log($"[WaveManager] CheckNextWave() 호출됨! 현재 웨이브 인덱스: {currentWaveIndex}, 총 웨이브 개수: {currentWaveDataList.Count}");

        if (isSpawning)
        {
            Debug.LogWarning("[WaveManager] 웨이브 진행 중인데 CheckNextWave가 호출됨! 실행 중지");
            return;
        }

        if (currentWaveIndex + 1 < currentWaveDataList.Count)
        {
            float waveInterval = currentWaveDataList[currentWaveIndex].interval; // interval 값 가져오기
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
}