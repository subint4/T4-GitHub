using UnityEngine;
using System.Collections.Generic;

public class EnemyDataManager
{
    private Dictionary<int, EnemySO> enemyDataDictionary = new Dictionary<int, EnemySO>();

    public void LoadData()
    {
        EnemySO[] enemyDataList = Resources.LoadAll<EnemySO>("EnemySO");
        foreach (var enemy in enemyDataList)
        {
            if (enemy != null)
            {
                enemyDataDictionary[enemy.ID] = enemy;
            }
        }
        Debug.Log($"[EnemyDataManager] {enemyDataDictionary.Count}개의 Enemy 데이터 로드 완료");
    }

    public EnemySO GetEnemyData(int enemyID)
    {
        return enemyDataDictionary.TryGetValue(enemyID, out var data) ? data : null;
    }
}
