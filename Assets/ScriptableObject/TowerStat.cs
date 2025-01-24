using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerStat 
{
    public int TowerBaseHealth;
    public int TowerBaseDamage;
    public int TowerBaseDeployCost;
    public float TowerBaseAttackSpeed;

    public void Initialize(int health,int damage,int deployCost,float attackSpeed)
    {
        TowerBaseHealth = health;
        TowerBaseDamage = damage;
        TowerBaseDeployCost = deployCost;
        TowerBaseAttackSpeed = attackSpeed;
    }
}
