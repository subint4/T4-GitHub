using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerStat 
{
    public int TowerBaseHealth { get; private set; }
    public int TowerBaseDamage { get; private set; }
    public int TowerBaseDeployCost { get; private set; }
    public float TowerBaseAttackSpeed { get; private set; }

    public TowerStat()
    {
        TowerBaseHealth = 100;
        TowerBaseDamage = 10;
        TowerBaseDeployCost = 50;
        TowerBaseAttackSpeed = 1f;
    }
    public void Initialize(int health,int damage, int deployCost,float attackSpeed)
    {
        Debug.Log($"Initializing TowerStat with: Health={health}, Damage={damage}, DeployCost={deployCost}, AttackSpeed={attackSpeed}");
        TowerBaseHealth = health;
        TowerBaseDamage= damage;
        TowerBaseDeployCost= deployCost;
        TowerBaseAttackSpeed= attackSpeed;
    }
}
