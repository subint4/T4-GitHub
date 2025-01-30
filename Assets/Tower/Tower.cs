using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerSO towerStats;

    private int Health;

    private void Start()
    {
        if(towerStats != null)
        {
            Health = towerStats.Health;
            Debug.Log($"타워 초기화 완료. 체력 :{Health}");
        }
        else
        {
            Debug.LogError("타워 스탯이 연결되지 않았습니다.");
        }
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log($"타워가 {damage}를 받았습니다. 현재 체력 : {Health}");

        if(Health<=0)
        {
            DestroyTower();
        }
    }
    private void DestroyTower()
    {
        Debug.Log("타워가 파괴되었습니다.");
        Destroy(gameObject);
    }
}
