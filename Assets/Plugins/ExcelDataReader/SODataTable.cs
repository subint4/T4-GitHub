using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class SODataTable
{
    public static void DisplaySOData(string dataType)
    {
        string path = "";

        switch (dataType)
        {
            case "EnemyData":
                path = "EnemyData"; // Resources 폴더 내 경로
                DisplayEnemyData(path);
                break;

            case "TowerData":
                path = "TowerData";
                DisplayTowerData(path);
                break;

            case "WaveData":
                path = "WaveData";
                DisplayWaveData(path);
                break;

            default:
                Debug.LogError($"잘못된 데이터 타입: {dataType}");
                return;
        }
    }

    private static void DisplayEnemyData(string path)
    {
        EnemySO[] allEnemies = Resources.LoadAll<EnemySO>(path);
        if (allEnemies.Length == 0)
        {
            Debug.LogError("적 데이터가 존재하지 않습니다!");
            return;
        }

        Debug.Log("==== [적 데이터] ====");
        foreach (EnemySO enemy in allEnemies)
        {
            Debug.Log($"ID: {enemy.ID} | Name: {enemy.Name} | HP: {enemy.Health} | Speed: {enemy.MovementSpeed} | ATK: {enemy.AttackPower}| ATKSpeed: {enemy.AttackSpeed} | Reward: {enemy.RewardMoney}");
        }
    }

    private static void DisplayTowerData(string path)
    {
        TowerSO[] allTowers = Resources.LoadAll<TowerSO>(path);
        if (allTowers.Length == 0)
        {
            Debug.LogError("타워 데이터가 존재하지 않습니다!");
            return;
        }

        Debug.Log("==== [타워 데이터] ====");
        foreach (TowerSO tower in allTowers)
        {
            Debug.Log($"ID: {tower.ID} | Name: {tower.Name} | HP: {tower.Health} | ATK: {tower.AttackPower} | DeployCost: {tower.DeployCost}");
        }
    }

    private static void DisplayWaveData(string path)
    {
        WaveSO[] allWaves = Resources.LoadAll<WaveSO>(path);
        if (allWaves.Length == 0)
        {
            Debug.LogError("웨이브 데이터가 존재하지 않습니다!");
            return;
        }

        Debug.Log("==== [웨이브 데이터] ====");
        foreach (WaveSO wave in allWaves)
        {
            Debug.Log($"Wave {wave.wave} | Stage: {wave.stagenum} | ID: {wave.ID}");

            foreach (var spawnData in wave.spawnDataList)
            {
                Debug.Log($"  Enemy ID: {spawnData.enemyID} | Count: {spawnData.count} | " +
                          $"SpawnGroup: {spawnData.SpawnGroup} | SpawnDelay: {spawnData.SpawnDelay} | " +
                          $"Interval: {spawnData.interval} | SpawnLaneID: {spawnData.SpawnLaneID}");
            }
        }
    }


}
