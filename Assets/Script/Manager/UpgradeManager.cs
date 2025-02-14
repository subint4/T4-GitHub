using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool UpgradeTower(Tower tower)
    {
        if (tower == null)
        {
            Debug.LogError("업그레이드할 타워가 없습니다!");
            return false;
        }

        if (tower.towerStats == null)
        {
            Debug.LogError("타워에 설정된 TowerSO가 없습니다!");
            return false;
        }

        TowerSO newTowerStats = DataManager.GetTowerData(tower.towerStats.NextLevelID);
        if (newTowerStats == null)
        {
            Debug.Log($"{tower.towerStats.Name}은(는) 최대 레벨입니다.");
            return false;
        }

        int upgradeCost = newTowerStats.UpgradeCost;
        if (ResourceManager.Instance.SpendGold(upgradeCost))
        {
            tower.UpgradeTower(newTowerStats);
            return true;
        }
        else
        {
            Debug.LogError("골드가 부족합니다!");
            return false;
        }
    }
}
