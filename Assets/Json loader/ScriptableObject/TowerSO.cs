using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTower", menuName = "Tower/Tower Data")]

public class TowerSO : ScriptableObject
{
    public string TowerName;
    public int baseDamage;
    public float AttackCoolDown;
    public int UpgradeCost;
    public int DeployCost;

    public LayerMask target;

    public float KnockbackPower;
    public float KnockbackCooldown;

    public TowerStatHandler[] modifier;

}
