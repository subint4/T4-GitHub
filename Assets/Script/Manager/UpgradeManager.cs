using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬이 변경되어도 유지
        }
        else
        {
            Debug.LogWarning("중복된 UpgradeManager가 감지됨. 기존 인스턴스를 유지하고, 새로운 것을 삭제합니다.");
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

        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager.Instance가 NULL입니다! 싱글톤 설정을 확인하세요.");
            return false;
        }

        if (tower.towerStats.NextLevelID <= 0)
        {
            Debug.LogError($"{tower.towerStats.Name}의 NextLevelID가 유효하지 않습니다! (NextLevelID: {tower.towerStats.NextLevelID})");
            return false;
        }

        TowerSO newTowerStats = TowerManager.Instance.GetTowerData(tower.towerStats.NextLevelID);
        if (newTowerStats == null)
        {
            Debug.Log($"{tower.towerStats.Name}은(는) 최대 레벨입니다.");
            return false;
        }

        int upgradeCost = newTowerStats.UpgradeCost;
        if (GoldManager.Instance != null && GoldManager.Instance.SpendGold(upgradeCost))
        {
            tower.UpgradeTower(newTowerStats);
            return true;
        }
        else
        {
            Debug.LogError("골드가 부족하거나 ResourceManager.Instance가 NULL입니다!");
            return false;
        }
    }

}
