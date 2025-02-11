using System;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Default,
    Slow,
    Pierce,
    Stun
}

[CreateAssetMenu(fileName = "NewTowerData", menuName = "New Tower/Tower")]
public class TowerSO : ScriptableObject
{
    public int key;
    public string UnitName;
    public int Health;
    public int AttackPower;
    public float AttackSpeed;
    public int DeployCost;
    public int UpgradeCost;
    public float Range;
    public TowerType TowerType;

    // JSON 데이터를 불러와 `TowerSO`를 생성하는 메서드
    public void LoadFromUnitData(UnitData unitData)
    {
        key = unitData.key;
        UnitName = unitData.UnitName;
        Health = unitData.Health;
        AttackPower = unitData.AttackPower;
        AttackSpeed = unitData.AttackSpeed;
        DeployCost = unitData.DeployCost;
        UpgradeCost = unitData.UpgradeCost;

        if (unitData.TowerType != null)
        {
            string towerTypeString = unitData.TowerType.Trim();
            if (Enum.TryParse(towerTypeString, true, out TowerType parsedType))
            {
                TowerType = parsedType;
            }
            else
            {
                Debug.LogError($"{UnitName}: TowerType 변환 실패! 기본값(Default) 적용.");
                TowerType = TowerType.Default;
            }
        }
        else
        {
            TowerType = TowerType.Default;
        }
    }
}
