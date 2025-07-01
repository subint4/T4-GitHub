using System.Collections.Generic;
using UnityEngine;
public enum EnemyType { Melee, Ranged } // 근접/원거리 구분 추가

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game Data/Enemy")]

public class EnemySO : ScriptableObject
{
<<<<<<< Updated upstream
    public int ID;
    public EnemyType Type;
    public string Name;
    public float Health;
    public float MovementSpeed;
    public float AttackSpeed;
    public float AttackPower;
    public int SpawnCount;
    public int RewardMoney;

    public void LoadFromJson(EnemyData data)
    {
        ID = data.ID;
        Name = data.Name;
        Health = data.Health;
        MovementSpeed = data.MovementSpeed;
        AttackSpeed = data.AttackSpeed;
        AttackPower = data.AttackPower;
        SpawnCount = data.SpawnCount;
        RewardMoney = data.RewardMoney;
=======
    // UnitData 속성
    public string UnitName;          // 적 이름
    public int Health;               // 체력
    public int AttackPower;          // 공격력
    public float AttackSpeed;        // 공격 속도
    public float MovementSpeed;      // 이동 속도
    public int DeployCost;           // 배치 비용

    // EnemyData 전용 속성
    public int EnemyID;
    public int RewardMoney;          // 적 처치 시 보상 금액
    public string EnemyType;         // 적 유형 (예: "Zombie", "Skeleton")
    public int SpawnCount;           // 스폰 수
    public float SpawnRate;          // 스폰 간격
    public string EnemyPrefab;       // 적 프리팹 이름

    // JSON 데이터를 불러와 EnemySO에 데이터 적용
    public void LoadFromEnemyData(EnemyData enemyData)
    {
        // UnitData 속성 로드
        EnemyID = enemyData.EnemyID;
        UnitName = enemyData.UnitName;
        Health = enemyData.Health;
        AttackPower = enemyData.AttackPower;
        AttackSpeed = enemyData.AttackSpeed;
        MovementSpeed = enemyData.MovementSpeed;
        DeployCost = enemyData.DeployCost;

        // EnemyData 전용 속성 로드
        RewardMoney = enemyData.RewardMoney;
        EnemyType = enemyData.EnemyType;
        SpawnCount = enemyData.SpawnCount;
        SpawnRate = enemyData.SpawnRate;
>>>>>>> Stashed changes
    }
}
[System.Serializable]
public class EnemyConfig
{
    public List<EnemyData> Enemies;
}
[System.Serializable]
public class EnemyData
{
    public int ID;
    public string Name;
    public float Health;
    public float MovementSpeed;
    public float AttackSpeed;
    public float AttackPower;
    public int SpawnCount;
    public int RewardMoney;
}
