using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataManager
{
    private static Dictionary<int, EnemySO> enemyDataDictionary = new Dictionary<int, EnemySO>();
    private static Dictionary<int, TowerSO> towerDataDictionary = new Dictionary<int, TowerSO>();
    private static Dictionary<int, WaveSO> waveDataDictionary = new Dictionary<int, WaveSO>();
    private static Dictionary<int, GameObject> towerPrefabDictionary = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> enemyPrefabDictionary = new Dictionary<int, GameObject>();

    static DataManager()
    {
        InitializeData();
    }

    private static void InitializeData()
    {
        Debug.Log("DataManager: 데이터 초기화 시작...");

        LoadEnemyData();
        LoadTowerData();
        LoadWaveData();
        LoadTowerPrefabs();
        LoadEnemyPrefabs();
    }

    // **적 데이터 로드**
    private static void LoadEnemyData()
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

    //  타워 데이터 로드
    private static void LoadTowerData()
    {
        TowerSO[] towerDataList = Resources.LoadAll<TowerSO>("TowerSO");
        foreach (var tower in towerDataList)
        {
            if (tower != null)
                towerDataDictionary[tower.ID] = tower;
        }
    }

    //  웨이브 데이터 로드
    private static void LoadWaveData()
    {
        WaveSO[] waveDataList = Resources.LoadAll<WaveSO>("WaveSO");
        foreach (var wave in waveDataList)
        {
            if (wave != null)
                waveDataDictionary[wave.ID] = wave;
        }
    }

    //  타워 프리팹 로드
    private static void LoadTowerPrefabs()
    {
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/Tower");

        foreach (var prefab in loadedPrefabs)
        {
            Tower towerComponent = prefab.GetComponent<Tower>();
            if (towerComponent != null && towerComponent.towerStats != null)
            {
                int towerID = towerComponent.towerStats.ID;
                towerPrefabDictionary[towerID] = prefab;
                Debug.Log($"타워 프리팹 로드됨: {prefab.name} (ID: {towerID})");
            }
            else
            {
                Debug.LogError($"타워 프리팹 '{prefab.name}'에서 Tower 또는 towerStats를 찾을 수 없음!");
            }
        }
    }

    //  적 프리팹 로드
    private static void LoadEnemyPrefabs()
    {
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy");

        foreach (var prefab in loadedPrefabs)
        {
            Enemy enemyComponent = prefab.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                if (enemyComponent.enemyStats == null) // EnemySO가 없는 경우 자동 할당
                {
                    if (enemyDataDictionary.TryGetValue(enemyComponent.EnemyID, out EnemySO enemySO))
                    {
                        enemyComponent.enemyStats = enemySO;
                        Debug.Log($"프리팹 '{prefab.name}'에 EnemySO '{enemySO.ID}' 자동 할당 완료!");
                    }
                    else
                    {
                        Debug.LogError($"프리팹 '{prefab.name}'에서 EnemySO ID '{enemyComponent.EnemyID}'를 찾을 수 없습니다!");
                    }
                }

                enemyPrefabDictionary[enemyComponent.EnemyID] = prefab;
            }
            else
            {
                Debug.LogError($"적 프리팹 '{prefab.name}'에서 Enemy 컴포넌트를 찾을 수 없습니다!");
            }
        }
    }


    // **적 프리팹 가져오기**
    public static GameObject GetEnemyPrefab(int enemyID)
    {
        if (enemyPrefabDictionary.TryGetValue(enemyID, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogError($"Enemy ID '{enemyID}'에 해당하는 프리팹을 찾을 수 없습니다!");
        return null;
    }

    //  특정 ID의 타워 프리팹 반환
    public static GameObject GetTowerPrefab(int towerID)
    {
        if (towerPrefabDictionary.TryGetValue(towerID, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogError($"Tower ID '{towerID}'에 해당하는 프리팹을 찾을 수 없습니다!");
        return null;
    }

    //  특정 ID의 적 데이터 반환
    public static EnemySO GetEnemyData(int enemyID)
    {
        return enemyDataDictionary.TryGetValue(enemyID, out var enemy) ? enemy : null;
    }

    //  특정 ID의 타워 데이터 반환
    public static TowerSO GetTowerData(int towerID)
    {
        return towerDataDictionary.TryGetValue(towerID, out var tower) ? tower : null;
    }

    //  특정 ID의 웨이브 데이터 반환
    public static WaveSO GetWaveData(int waveID)
    {
        return waveDataDictionary.TryGetValue(waveID, out var wave) ? wave : null;
    }

    //  1레벨 타워 ID 목록 반환
    public static List<int> GetAllLevel1TowerIDs()
    {
        return towerDataDictionary.Values
            .Where(tower => tower.Level == 1)
            .OrderBy(tower => tower.ID)
            .Select(tower => tower.ID)
            .ToList();
    }
}
