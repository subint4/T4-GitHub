using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    private Dictionary<int, GameObject> enemyPrefabDictionary = new Dictionary<int, GameObject>();
    private Dictionary<int, EnemySO> enemyDataDictionary = new Dictionary<int, EnemySO>();
    private List<Enemy> activeEnemies = new List<Enemy>();
    public Transform[] spawnPoints;

    public GaugeManager progressBar;
    public int totalEnemies = 10;
    private int defeatedEnemies = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadEnemyData();
            InitializeEnemies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadEnemyData()
    {
        EnemySO[] enemyDataList = Resources.LoadAll<EnemySO>("EnemySO");
        foreach (var enemy in enemyDataList)
        {
            if (enemy != null)
            {
                enemyDataDictionary[enemy.ID] = enemy;
            }
        }
    }

    private void InitializeEnemies()
    {
        Debug.Log("EnemyManager: 모든 적 프리팹과 SO 데이터를 불러옵니다.");

        GameObject[] enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy");
        foreach (var prefab in enemyPrefabs)
        {
            Enemy enemyComponent = prefab.GetComponent<Enemy>();
            if (enemyComponent == null)
            {
                Debug.LogError($"{prefab.name} 프리팹에서 Enemy 컴포넌트를 찾을 수 없습니다!");
                continue;
            }

            if (!enemyDataDictionary.TryGetValue(enemyComponent.EnemyID, out EnemySO enemySO))
            {
                Debug.LogError($"{prefab.name} 프리팹에 해당하는 EnemySO (ID: {enemyComponent.EnemyID})를 찾을 수 없습니다!");
                continue;
            }

            enemyComponent.enemyStats = enemySO;
            enemyPrefabDictionary[enemyComponent.EnemyID] = prefab;
            Debug.Log($"Enemy ID {enemyComponent.EnemyID} - {prefab.name} 프리팹과 자동 연결됨.");
        }
    }

    public EnemySO GetEnemyData(int id)
    {
        return enemyDataDictionary.TryGetValue(id, out var data) ? data : null;
    }

    public GameObject SpawnEnemy(int enemyID)
    {
        if (!enemyDataDictionary.ContainsKey(enemyID) || !enemyPrefabDictionary.ContainsKey(enemyID))
        {
            Debug.LogError($"EnemyManager: Enemy ID {enemyID}에 대한 데이터 또는 프리팹을 찾을 수 없습니다!");
            return null;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemyManager: 스폰 위치가 설정되지 않았습니다!");
            return null;
        }

        Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = enemyPrefabDictionary[enemyID];
        EnemySO enemyData = enemyDataDictionary[enemyID];

        GameObject newEnemyObj = Instantiate(enemyPrefab, selectedSpawnPoint.position, Quaternion.identity);
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            newEnemy.Initialize(enemyData);
            activeEnemies.Add(newEnemy);
            return newEnemyObj;
        }
        else
        {
            Debug.LogError("EnemyManager: 생성된 적에 Enemy 컴포넌트가 없습니다!");
            return null;
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }
    public void OnEnemyDefeated()
    {
        defeatedEnemies++;

        // 게이지바 업데이트
        if (progressBar != null)
        {
            progressBar.UpdateGauge(defeatedEnemies, totalEnemies);
        }

        Debug.Log($"몬스터 처치 진행률: {defeatedEnemies} / {totalEnemies}");

        if (defeatedEnemies >= totalEnemies)
        {
            StageClear();
        }
    }

    private void StageClear()
    {
        Debug.Log("스테이지 클리어!");
    }
}
