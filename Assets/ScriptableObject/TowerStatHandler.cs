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
        }
        InitializedTower();
    }
    private void InitializedTower()
    {
        CurrentStat.Initialize(
            towerData.BaseStat.TowerBaseHealth,
            towerData.BaseStat.TowerBaseDamage,
            towerData.BaseStat.TowerBaseDeployCost,
            towerData.BaseStat.TowerBaseAttackSpeed
            );
        Debug.Log($"Tower '{towerData.TowerName}' initialized. Health: {CurrentStat.TowerBaseHealth}, Attack: {CurrentStat.TowerBaseDamage}");
    }
}