using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TowerDataManager
{
    private Dictionary<int, TowerSO> towerDataDictionary = new Dictionary<int, TowerSO>();
    private const string TowerDataPath = "TowerSO/Tower_"; // SO 경로 기본값

    public void LoadData()
    {
        Debug.Log("[TowerDataManager] 타워 데이터 매니저 초기화 완료.");
    }

    public TowerSO GetTowerData(int towerID)
    {
        if (!towerDataDictionary.TryGetValue(towerID, out var tower))
        {
            tower = Resources.Load<TowerSO>($"{TowerDataPath}{towerID}");
            if (tower != null)
            {
                towerDataDictionary[towerID] = tower;
                Debug.Log($"[TowerDataManager] Tower ID {towerID} 로드 완료.");
            }
            else
            {
                Debug.LogError($"[TowerDataManager] Tower ID {towerID} 데이터를 찾을 수 없습니다!");
                return null;
            }
        }
        return tower;
    }

    public List<int> GetAllLevel1TowerIDs()
    {
        // 만약 로드된 데이터가 없다면 기본적으로 1레벨 타워를 로드
        if (towerDataDictionary.Count == 0)
        {
            Debug.LogWarning("[TowerDataManager] 타워 데이터가 비어 있음. 레벨 1 타워를 자동 로드.");
            LoadAllLevel1Towers();
        }

        return towerDataDictionary.Values
            .Where(tower => tower.Level == 1)
            .OrderBy(tower => tower.ID)
            .Select(tower => tower.ID)
            .ToList();
    }

    private void LoadAllLevel1Towers()
    {
        for (int i = 1; i <= 50; i++) // 1~50 ID 범위에서 Level 1 타워 로드
        {
            TowerSO tower = Resources.Load<TowerSO>($"{TowerDataPath}{i}");
            if (tower != null && tower.Level == 1)
            {
                towerDataDictionary[i] = tower;
            }
        }
        Debug.Log("[TowerDataManager] 모든 레벨 1 타워 로드 완료.");
    }
}
