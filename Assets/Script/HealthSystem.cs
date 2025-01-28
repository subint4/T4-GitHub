using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int rewardMoney = 50;
    private int currentHealth;

    [Header("Object Type")]
    public bool isEnemy = true; // 이 객체가 적인지 여부 (true: 적, false: 타워)

    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage}. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log($"{gameObject.name} died");
        if(isEnemy)
        {
            GiveReward();
        }
        
        Destroy(gameObject);
    }
    private void GiveReward()
    {
        PlayerSystem.Instance.AddMoney(rewardMoney);
        Debug.Log($"Player received {rewardMoney} money from {gameObject.name}.");
    }
}
