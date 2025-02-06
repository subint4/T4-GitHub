using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public TowerSO towerStats;
    private Tower selectedTower;
    public ResourceManager resourceManager;
    [HideInInspector]public int CurrentTier = 1;
    [HideInInspector]public int MaxTier = 3;
    [HideInInspector][Header("Tower Tiers")]
    public TowerStat baseStats;
    [HideInInspector]private TowerStat currentStats;
    [HideInInspector]private int upgradeCost;
    [HideInInspector]private UpgradeUI upgradeUI;

    private void Start()
    {
        if(towerStats == null || resourceManager == null)
        {
            return;
        }
        ApplyTowerData(towerStats);
        Debug.Log($"현재 재화 : {resourceManager.currentGold}");
    }

    public void SelectTower(Tower tower)
    {
        if(selectedTower ==tower)
        {
            DeselectTower();
            return;
        }
        selectedTower = tower;
        Debug.Log($"선택된 타워 : {selectedTower.towerStats.UnitName}");
    }
    public void UpgradeSelectedTower()
    {
        if(selectedTower == null)
        {
            Debug.Log("타워 선택 안됨");
            return;
        }
        if (CurrentTier >= MaxTier)
        {
            Debug.Log("최대 단계에 도달했습니다.");
            return;
        }
        TowerSO towerData = selectedTower.towerStats;
        int upgradeCost = towerData.UpgradeCost;
        if (resourceManager.currentGold >= upgradeCost)
        {
            resourceManager.SpendGold(upgradeCost);
            
            if(towerData.nextTierTower != null)
            {
                Debug.Log($"타워 업그레이드 : {towerData.UnitName}=>{towerData.nextTierTower.UnitName}");
                selectedTower.towerStats = towerData.nextTierTower;
                upgradeUI.HideUpgradeButton();
            }
        }
    }
    public void DeselectTower()
    {
        selectedTower = null;
        upgradeUI.HideUpgradeButton();
        Debug.Log("타워 선택 해제");
    }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.U))
            {
                UpgradeSelectedTower();
            }
        }
    public TowerStat GetCurrentStats()
    {
        return currentStats;
    }
    [System.Serializable]
    public class TowerStat
    {
        public int damage;
        public float attackSpeed;

    public TowerStat(int damage,  float attackSpeed)
    {
        this.damage = damage;
        this.attackSpeed = attackSpeed;
    }

    public TowerStat(TowerStat other)
    {
        this.damage = other.damage;
        this.attackSpeed = other.attackSpeed;
    }
}
    private void ApplyTowerData(TowerSO newTower)
    {
        // 새로운 SO 데이터를 반영
        currentStats = new TowerStat(newTower.AttackPower, newTower.AttackSpeed);
        upgradeCost = newTower.UpgradeCost;
    }
}