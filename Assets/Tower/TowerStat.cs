using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStat 
{
    private int baseHealth;
    private float baseSpeed;
    private int baseAttack;
    private int baseDeployCost;

    private const int minHealth=0;
    private const int maxHealth=100;
    private const float minSpeed = 0.1f;
    private const float maxSpeed = 1.0f;
    private const int minAttack = 0;
    private const int maxAttack = 500;
    private const int minDeployCost = 10;
    private const int maxDeployCost = 100;



    public int BaseDeployCost
    {
        get => baseDeployCost;
        set => baseDeployCost = Mathf.Clamp(value,minDeployCost,maxDeployCost);
    }

    public int BaseHealth
    {
        get => baseHealth;
        set => baseHealth = Mathf.Clamp(value, minHealth, maxHealth);
    }
    public int BaseAttack
    {
        get => baseAttack;
        set => baseAttack = Mathf.Clamp(value, minAttack, maxAttack);
    }
    public float BaseSpeed
    {
        get => baseSpeed;
        set => baseSpeed = Mathf.Clamp(value, minSpeed, maxSpeed);
    }


    public void Initialize(int health,int attack,float speed,int deploycost,int upgradecost)
    {   
        baseHealth = health;
        baseSpeed = speed;
        baseAttack = attack;
        baseDeployCost = deploycost;

    }
}
