using UnityEngine;
using System.Collections.Generic;

public class EnemyDataManager
{
    private Dictionary<int, EnemySO> enemyDataDictionary = new Dictionary<int, EnemySO>();

    public void LoadData()
    {
        Debug.Log("[EnemyDataManager] 적 데이터 로드 시작...");

        EnemySO[] enemyDataList = Resources.LoadAll<EnemySO>("EnemySO");
        foreach (var enemy in enemyDataList)
        {
            if (enemy != null)
            {
                enemyDataDictionary[enemy.ID] = enemy;
            }
        }

        Debug.Log($"[EnemyDataManager] 적 데이터 로드 완료. 총 {enemyDataDictionary.Count}개의 데이터 로드됨.");
    }

    public EnemySO GetEnemyData(int enemyID)
    {
        return enemyDataDictionary.TryGetValue(enemyID, out var data) ? data : null;
    }
}
