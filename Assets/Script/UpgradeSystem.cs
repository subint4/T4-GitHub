using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public int CurrentMoney = 100;
    public int UpgradeCost = 50;
    public int CurrentTier = 1;
    public int MaxTier = 3;
    [Header("Tower Tiers")]
    public TowerStat baseStats;
    private TowerStat currentStats;

    private void Start()
    {
        currentStats = new TowerStat(baseStats);
        Debug.Log($"현재 재화 : {CurrentMoney}, 업그레이드 단계 : {CurrentTier}");
    }
    public void Upgrade()
    {
        if (CurrentTier >= MaxTier)
        {
            Debug.Log("최대 단계에 도달했습니다.");
            return;
        }
        if (CurrentMoney >= UpgradeCost)
        {
            CurrentMoney -= UpgradeCost;
            CurrentTier++;
            UpgradeCost = Mathf.CeilToInt(UpgradeCost * 1.5f);

            currentStats.damage = Mathf.CeilToInt(currentStats.damage * 1.5f);
            currentStats.attackSpeed *= 1.5f;
            Debug.Log($"Upgraded Damage: {currentStats.damage}, AttackSpeed: {currentStats.attackSpeed}");
            Debug.Log($"업그레이드 완료. 현재 단계: {CurrentTier},남은 재화 : {CurrentMoney}");
        }
        else
        {
            Debug.Log("돈이 모자랍니다.");
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
}