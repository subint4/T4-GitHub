using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Health,
    Speed,
    Attack,
    TowerTier,
    UpgradeCost
}

public enum StatModType
{
    flat,   // 고정값증감
    percent // 비율로 증감
}

[Serializable]

public class StatModifier
{
    public StatType statType;
    public StatModType modType;
    public float value;

    public StatModifier(StatType statType, StatModType modType, float value)
    {
        this.statType = statType;
        this.modType = modType;
        this.value = value;
    }
}
