using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthSystem : MonoBehaviour
{
    public GameObject mobObject;
    private MobManager mobManager;
    [SerializeField] private float healthChangeDelay = .1f;

    MobStatHandler MobStatHandler;
    TowerStatHandler TowerStatHandler;
    private float timeSincelastChange = float.MaxValue;

    public event Action OnDamage;

    public void Start()
    {
        mobManager = mobObject.GetComponent<MobManager>(); 
    }
    public float CurrentHealth {  get; private set; }
    public float MaxHealth => TowerStatHandler.CurrentStat.BaseHealth;

    private void Awake()
    {
        TowerStatHandler=GetComponent<TowerStatHandler>();
        MobStatHandler = GetComponent<MobStatHandler>();
    }

    public void InitHealth()
    {
        CurrentHealth = MaxHealth;
    }

    public bool ChangeHealth(float value)
    {
        if(value == 0 || timeSincelastChange < healthChangeDelay)
        {
            return false;
        }
        timeSincelastChange = 0;
        CurrentHealth += value;
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;

        if(value <0)
        {
            OnDamage?.Invoke();
        }
        if(CurrentHealth <= 0f)
        {
            Die();
        }
        return true;
    }
    void Die()
    {
        PlayerManager player = FindObjectOfType<PlayerManager>();        
        if (player != null)
        {
            {
                player.AddMoney(mobManager.RewardMoney);
            }
        }
        Destroy(gameObject);
    }
}
