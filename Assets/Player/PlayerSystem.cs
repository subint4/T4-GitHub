using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public static PlayerSystem instance;

    public int Money = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AddMoney(int amount)
    {
        Money += amount;
        Debug.Log($"보상금 {amount} 획득 현재 보유 금액 : {Money}");
    }
}
