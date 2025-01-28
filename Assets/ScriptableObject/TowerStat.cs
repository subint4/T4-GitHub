using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerStat 
{
    public int baseHealth { get; private set; }
    public int baseDamage { get; private set; }
    public int baseDeployCost { get; private set; }
    public float baseAttackSpeed { get; private set; }

    public TowerStat()
    {
        baseHealth = 0;
        baseDamage = 0;
        baseDeployCost = 0;
        baseAttackSpeed = 1f;
    }
    public void Initialize(int health,int damage, int deployCost,float attackSpeed)
    {
        Debug.Log($"Initializing TowerStat with: Health={health}, Damage={damage}, DeployCost={deployCost}, AttackSpeed={attackSpeed}");
        baseHealth = health;
        baseDamage= damage;
        baseDeployCost= deployCost;
        baseAttackSpeed= attackSpeed;
    }
}
