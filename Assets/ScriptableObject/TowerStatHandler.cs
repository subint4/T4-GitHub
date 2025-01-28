using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStatHandler : MonoBehaviour
{
    [Header("Tower Data")]
    [SerializeField] private TowerSO towerData;

    public TowerStat CurrentStat { get; private set; } = new TowerStat();

    private void Start()
    {
        if (towerData == null)
        {
            Debug.LogError("TowerData is not assigned");
            return;
        }
        InitializedTower();
    }
    private void InitializedTower()
    {
        CurrentStat.Initialize(
            towerData.BaseStat.baseHealth,
            towerData.BaseStat.baseDamage,
            towerData.BaseStat.baseDeployCost,
            towerData.BaseStat.baseAttackSpeed
            );
        Debug.Log($"Tower '{towerData.TowerName}' initialized. Health: {CurrentStat.baseHealth}, Attack: {CurrentStat.baseDamage}");
    }
}