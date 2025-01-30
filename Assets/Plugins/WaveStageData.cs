using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveStageData
{
    public int key;      // 웨이브 단계 (기존 key)
    public string EnemyType;   // 적 유형 (예: "Zombie", "Skeleton")
    public int SpawnCount;     // 적 스폰 수
    public float SpawnRate;    // 적 스폰 간격
    public string EnemyPrefab; // 적 프리팹 이름
}

[System.Serializable]
public class WaveStageConfig
{
    public List<WaveStageData> WaveStages; // 웨이브 단계별 설정 리스트
}