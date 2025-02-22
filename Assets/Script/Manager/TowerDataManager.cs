using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TowerDataManager
{
    private Dictionary<int, TowerSO> towerDataDictionary = new Dictionary<int, TowerSO>();

    public void LoadData()
    {
        TowerSO[] towerDataList = Resources.LoadAll<TowerSO>("TowerSO");
        foreach (var tower in towerDataList)
        {
            if (tower != null)
            {
                towerDataDictionary[tower.ID] = tower;
            }
        }
        Debug.Log($"TowerDataManager: {towerDataDictionary.Count}개의 타워 데이터 로드 완료");
    }

    public TowerSO GetTowerData(int towerID)
    {
        return towerDataDictionary.TryGetValue(towerID, out var data) ? data : null;
    }

    public List<int> GetAllLevel1TowerIDs()
    {
        return towerDataDictionary.Values
            .Where(tower => tower.Level == 1)
            .OrderBy(tower => tower.ID)
            .Select(tower => tower.ID)
            .ToList();
    }
}
