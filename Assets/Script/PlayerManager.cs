using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int CurrentMoney = 0;
    
    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        Debug.Log($"재화가 증가했습니다. 현재 재화 : {CurrentMoney}");
    }
}
