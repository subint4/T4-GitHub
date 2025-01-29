using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem 
{
    private int maxHealth;
    private int currentHealth;
    private Action onDeath;

    [Header("Object Type")]
    public bool isEnemy = true; // 이 객체가 적인지 여부 (true: 적, false: 타워)

    public HealthSystem(EnemySO enemyData = null, TowerSO towerData = null,Action onDeath = null)
    {
        if (enemyData != null)
        {
            this.maxHealth = enemyData.baseHealth;
            this.currentHealth = enemyData.baseHealth;
        }
        else if (towerData != null)
        {
            this.maxHealth = towerData.BaseStat.baseHealth;
            this.currentHealth = towerData.BaseStat.baseHealth;
        }
        else
        {
            Debug.LogError("EnemySO 또는 TowerSO가 연결되지 않았습니다!");
        }
        this.onDeath = onDeath;
    }
    public int GetHealth()
    {
        return currentHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"체력 감소 : {damage}. 현재 체력 : {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        onDeath?.Invoke();
    }

}
