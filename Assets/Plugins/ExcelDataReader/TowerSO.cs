using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "New Tower/Tower")]
public class TowerSO : ScriptableObject
{
    public int key;
    public string UnitName;
    public int Health;
    public int AttackPower;
    public float AttackSpeed;
    public int DeployCost;
    public float Range;

    // JSON 데이터를 불러와 `TowerSO`를 생성하는 메서드
    public void LoadFromUnitData(UnitData unitData)
    {
        key = unitData.key;
        UnitName = unitData.UnitName;
        Health = unitData.Health;
        AttackPower = unitData.AttackPower;
        AttackSpeed = unitData.AttackSpeed;
        DeployCost = unitData.DeployCost;
    }
}
