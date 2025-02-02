using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public string monsterName = ""; // 몬스터 이름 (CSV에 있는 이름과 일치해야 함)

    // 몬스터 처치 함수 (보상 지급 + 제거)
    public void KillMonster()
    {
        MoneyManager.instance.AddMonsterReward(monsterName);
        Destroy(gameObject);
    }
}
