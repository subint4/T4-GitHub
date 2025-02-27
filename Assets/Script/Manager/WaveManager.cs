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
            return;
        }

        // **씬이 로드될 때 무조건 웨이브 인덱스를 초기화**
        currentWaveIndex = 0;
        defeatedEnemies = 0;
        totalEnemies = 0;
        Debug.Log("[WaveManager] 씬 로드 시 웨이브 진행도 초기화 완료!");
    }

    //public void AssignSpawnPoints()
    //{
    //    spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint")
    //        .Select(go => go.transform)
    //        .ToArray();

    //    if (spawnPoints == null || spawnPoints.Length == 0)
    //    {
    //        Debug.LogError("[WaveManager] 스폰 포인트를 찾을 수 없습니다! 씬에서 'SpawnPoint' 태그가 있는 오브젝트를 확인하세요.");
    //    }
    //    else
    //    {
    //        Debug.Log($"[WaveManager] 스폰 포인트 {spawnPoints.Length}개 할당 완료!");
    //    }
    //}

    public void LoadWavesForStage()
    {
        if (StageManager.Instance == null)
        {
            Debug.LogError("[WaveManager] StageManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        int stageNum = StageManager.Instance.currentStageNum;
        int subStageNum = StageManager.Instance.GetCurrentSubStageNum();

        Debug.Log($"[WaveManager] {stageNum}-{subStageNum} 스테이지 데이터 로드 시도 중...");

        List<int> waveIDs = DataManager.Instance.StageDataManager.GetWaveIDsForStage(stageNum, subStageNum);
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

        Debug.Log($"[WaveManager] {stageNum}-{subStageNum} 웨이브 데이터 로드 완료! 총 웨이브 수: {currentWaveDataList.Count}");
    }


    /// <summary>
    /// 스테이지와 서브 스테이지 번호를 받아 웨이브 데이터를 로드
    /// </summary>
    public void LoadWavesForStage(int stageNum, int subStageNum)
    {
        if (DataManager.Instance == null || DataManager.Instance.StageDataManager == null || DataManager.Instance.WaveDataManager == null)
        {
            Debug.LogError("[WaveManager] 데이터 매니저를 찾을 수 없습니다!");
            return;
        }

        List<int> waveIDs = DataManager.Instance.StageDataManager.GetWaveIDsForStage(stageNum, subStageNum);
        currentWaveDataList = DataManager.Instance.WaveDataManager.GetWaveDataList(waveIDs);

        Debug.Log($"[WaveManager] {stageNum}-{subStageNum} 웨이브 데이터 로드 완료! 총 웨이브 수: {currentWaveDataList.Count}");

        if (currentWaveDataList.Count == 0)
        {
            Debug.LogError($"[WaveManager] {stageNum}-{subStageNum}의 웨이브 데이터가 없습니다!");
        }
    }
    private void LoadNextSubStage()
    {
        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        int currentSubStage = PlayerPrefs.GetInt("CurrentSubStage", 1);

        Debug.Log($"[SceneLoader] 현재 서브 스테이지: {currentStage}-{currentSubStage}");

        // 마지막 서브 스테이지 개수를 확인 (예: 5까지 있는 경우)
        int maxSubStage = 5; // 필요에 따라 조정

        if (currentSubStage >= maxSubStage)
        {
            Debug.Log("[SceneLoader] 마지막 서브 스테이지입니다. 다음 메인 스테이지로 이동해야 합니다.");
            return;
        }

        currentSubStage++;
        PlayerPrefs.SetInt("CurrentSubStage", currentSubStage);
        PlayerPrefs.Save();

        Debug.Log($"[SceneLoader] 다음 서브 스테이지 로드: {currentStage}-{currentSubStage}");
        ApplySubStageSettings();
    }
    private void ApplySubStageSettings()
    {
        int stageNum = PlayerPrefs.GetInt("CurrentStage", 1);
        int subStageNum = PlayerPrefs.GetInt("CurrentSubStage", 1);

        Debug.Log($"[SceneLoader] {stageNum}-{subStageNum} 설정 적용 시도...");

        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.LoadWavesForStage();
            Debug.Log($"[SceneLoader] {stageNum}-{subStageNum} 웨이브 데이터 로드 완료!");
        }
        else
        {
            Debug.LogWarning("[SceneLoader] WaveManager를 찾을 수 없음, 웨이브 데이터 적용 생략");
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
            Debug.Log($"[WaveManager] 다음 웨이브 {currentWaveIndex + 2} 시작 준비...");
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        currentWaveIndex++;
        Debug.Log($"[WaveManager] StartNextWave() 호출됨. 현재 웨이브 인덱스: {currentWaveIndex}, 총 웨이브 수: {currentWaveDataList.Count}");

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
