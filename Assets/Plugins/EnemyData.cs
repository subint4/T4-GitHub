using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemyData : UnitData
{
    public int RewardMoney;    // 적 처치 시 보상 금액
    public string EnemyType;   // 적의 유형 (예: "Zombie", "Skeleton")
    public int SpawnCount;     // 적의 스폰 수
    public float MovementSpeed;  // 이동속도
    public float SpawnRate;    // 적의 스폰 간격

}

[System.Serializable]
public class EnemyConfig
{
    public List<EnemyData> Enemies;
}