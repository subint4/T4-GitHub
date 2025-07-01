<<<<<<< Updated upstream
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWaveData", menuName = "Wave/WaveSO")]
public class WaveSO : ScriptableObject
{
    public int ID;
    public int stagenum;
    public int substagenum;
    public int wave;
    public float interval;
    public List<WaveSpawnData> spawnDataList = new List<WaveSpawnData>();

    public void LoadFromJson(List<WaveSpawnData> waveDataList)
    {
        if (waveDataList == null || waveDataList.Count == 0)
        {
            Debug.LogError("WaveSO: JSON 데이터가 비어 있습니다.");
            return;
        }

        ID = waveDataList[0].ID;
        stagenum = waveDataList[0].stagenum;
        wave = waveDataList[0].wave;
        interval = waveDataList[0].interval;
        spawnDataList = new List<WaveSpawnData>(waveDataList);
    }

}
[Serializable]
public class WaveSpawnData
{
    public int ID;
    public int stagenum;
    public int wave;
    public int enemyID;
    public int count;
    public int SpawnGroup;
    public float SpawnDelay;
    public float interval;
    public int SpawnLaneID;
}
=======
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/WaveSO", order = 2)]
public class WaveSO : ScriptableObject
{
    public int waveCount;
    public float spawnRate;

    // 이제 Unity에서 직렬화 가능
    public SerializableDictionary<int, int> enemyCounts = new SerializableDictionary<int, int>();

    public int GetTotalEnemies()
    {
        int totalEnemies = 0;
        foreach (var enemy in enemyCounts)
        {
            totalEnemies += enemy.Value;
        }
        return totalEnemies;
    }
}
>>>>>>> Stashed changes
