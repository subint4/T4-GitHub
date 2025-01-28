using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public TowerSO towerSO;
    public int CurrentMoney = 100;
    public int UpgradeCost;
    public int CurrentTier = 1;
    public int MaxTier = 3;
    [Header("Tower Tiers")]
    private TowerStat currentStats;

    private void Start()
    {
        currentStats = new TowerStat(towerSO.BaseStat.baseDamage, towerSO.BaseStat.baseHealth);
        UpgradeCost = towerSO.BaseStat.baseDeployCost; // TODO: 업그레이드시 1.5배 비용 증가
        Debug.Log($"타워 초기화 완료: Damage = {currentStats.damage}, Health = {currentStats.health}");

    }
    public void Upgrade()
    {
    /*if (CurrentTier >= MaxTier)
    //{
    //    Debug.Log("최대 단계에 도달했습니다.");
    //    return;
    //}
    //if (CurrentMoney >= UpgradeCost)
    //{
    //    CurrentMoney -= UpgradeCost;
    //    CurrentTier++;
    //    UpgradeCost = Mathf.CeilToInt(UpgradeCost * 1.5f);

    //    currentStats.damage = Mathf.CeilToInt(currentStats.damage * 1.5f);
    //    currentStats.health = Mathf.CeilToInt(currentStats.health * 1.5f);
    //    Debug.Log($"Upgraded Damage: {currentStats.damage}, Health: {currentStats.health}");
    //    Debug.Log($"업그레이드 완료. 현재 단계: {CurrentTier},남은 재화 : {CurrentMoney}");
    //}
    //else
    //{
    //    Debug.Log("돈이 모자랍니다.");
    }*/
            if(CurrentTier >= MaxTier)
        {
            Debug.Log("최대 단계입니다.");
            return;
        }
            if(PlayerSystem.Instance.currentMoney >= UpgradeCost)
        {
            PlayerSystem.Instance.AddMoney(-UpgradeCost);
        
            CurrentTier++;
            UpgradeCost = Mathf.CeilToInt(UpgradeCost * 1.5f);
            currentStats.damage = Mathf.CeilToInt(currentStats.damage * 1.5f);
            currentStats.health = Mathf.CeilToInt(currentStats.health * 1.5f);
        
            Debug.Log($"업그레이드 완료: Tier {CurrentTier},Damage = {currentStats.damage}, Health={currentStats.health}");
        }
        else
        {
            Debug.Log("재화가 모자랍니다.");
        }
    }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.U))
            {
                Upgrade();
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
        public int health;

    public TowerStat(int damage,  int health)
    {
        this.damage = damage;
        this.health = health;
    }

    public TowerStat(TowerStat other)
    {
        this.damage = other.damage;
        this.health = other.health;
    }
    }
}