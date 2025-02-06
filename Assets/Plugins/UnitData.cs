using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public int key;
    public string UnitName;
    public int Health;
    public int AttackPower;
    public float AttackSpeed;
    public int DeployCost;
    public int UpgradeCost;
}

[System.Serializable]
public class UnitConfig
{
    public List<UnitData> Units;
}