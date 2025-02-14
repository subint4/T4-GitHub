using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game Data/Enemy")]
public class EnemySO : ScriptableObject
{
    public int ID;
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
