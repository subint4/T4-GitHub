using System.Collections.Generic;
using UnityEngine;

public static class DataManager
{
    private static Dictionary<int, EnemySO> enemyDataDictionary = new Dictionary<int, EnemySO>();
    private static Dictionary<int, TowerSO> towerDataDictionary = new Dictionary<int, TowerSO>();
    private static Dictionary<int, WaveSO> waveDataDictionary = new Dictionary<int, WaveSO>();

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
    }

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

    private static void LoadTowerData()
    {
        TowerSO[] towerDataList = Resources.LoadAll<TowerSO>("TowerSO");
        foreach (var tower in towerDataList)
        {
            if (tower != null)
            {
                towerDataDictionary[tower.ID] = tower;
            }
        }
    }

    private static void LoadWaveData()
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

    public static EnemySO GetEnemyData(int enemyID)
    {
        return enemyDataDictionary.TryGetValue(enemyID, out var enemy) ? enemy : null;
    }

    public static TowerSO GetTowerData(int towerID)
    {
        return towerDataDictionary.TryGetValue(towerID, out var tower) ? tower : null;
    }
    public static List<TowerSO> GetAllTowerData()
    {
        return new List<TowerSO>(towerDataDictionary.Values);
    }


public static WaveSO GetWaveData(int waveID)
    {
        return waveDataDictionary.TryGetValue(waveID, out var wave) ? wave : null;
    }
}
