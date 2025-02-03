using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class SODataTable
{
    public static void DisplaySOData(ExcelConverterEditor.DataType dataType)
    {
        string path = "";

        switch (dataType)
        {
            case ExcelConverterEditor.DataType.EnemyData:
                path = "Assets/Resources/EnemyData/";
                DisplayEnemyData(path);
                break;

            case ExcelConverterEditor.DataType.UnitData:
                path = "Assets/Resources/TowerData/";
                DisplayTowerData(path);
                break;

            case ExcelConverterEditor.DataType.WaveData:
                path = "Assets/Resources/WaveData/";
                DisplayWaveData(path);
                break;
        }
    }

    private static void DisplayEnemyData(string path)
    {
        EnemySO[] allEnemies = Resources.LoadAll<EnemySO>(path);
        foreach (EnemySO enemy in allEnemies)
        {
            Debug.Log($"{enemy.EnemyType} | SpawnCount: {enemy.SpawnCount} | Speed: {enemy.MovementSpeed} | Reward: {enemy.RewardMoney}");
        }
    }

    private static void DisplayTowerData(string path)
    {
        TowerSO[] allTowers = Resources.LoadAll<TowerSO>(path);
        foreach (TowerSO tower in allTowers)
        {
            Debug.Log($"{tower.UnitName} | HP: {tower.Health} | ATK: {tower.AttackPower} | Range: {tower.Range}");
        }
    }

    private static void DisplayWaveData(string path)
    {
        WaveStageData[] allWaves = Resources.LoadAll<WaveStageData>(path);
        foreach (WaveStageData wave in allWaves)
        {
            Debug.Log($"{wave.EnemyType} | Count: {wave.SpawnCount} | Rate: {wave.SpawnRate}");
        }
    }
}
