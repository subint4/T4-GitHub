//using UnityEngine;

//public class UpgradeManager : MonoBehaviour
//{
//    public static UpgradeManager Instance { get; private set; }

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);  // 씬이 변경되어도 유지
//        }
//        else
//        {
//            Debug.LogWarning("중복된 UpgradeManager가 감지됨. 기존 인스턴스를 유지하고, 새로운 것을 삭제합니다.");
//            Destroy(gameObject);
//        }
//    }

//    /// <summary>
//    /// 선택한 타워를 업그레이드 시도
//    /// </summary>
//    public bool UpgradeTower(Tower tower)
//    {
//        if (tower == null)
//        {
//            Debug.LogError("업그레이드할 타워가 없습니다!");
//            return false;
//        }

//        if (tower.towerStats == null)
//        {
//            Debug.LogError("타워에 설정된 TowerSO가 없습니다!");
//            return false;
//        }
//        if (tower.towerStats.NextLevelID <= 0)
//        {
//            Debug.LogWarning($"{tower.towerStats.Name}은(는) 최종 업그레이드 상태입니다. (NextLevelID: {tower.towerStats.NextLevelID})");
//            return false;
//        }

//        // 다음 업그레이드 타워 데이터 가져오기
//        TowerSO newTowerStats = DataManager.Instance.TowerDataManager.GetTowerData(tower.towerStats.NextLevelID);
//        if (newTowerStats == null)
//        {
//            Debug.LogError($"[UpgradeManager] Tower ID {tower.towerStats.NextLevelID}의 데이터를 찾을 수 없습니다!");
//            return false;
//        }

//        int upgradeCost = newTowerStats.UpgradeCost;
//        if (GoldManager.Instance != null && GoldManager.Instance.SpendGold(upgradeCost))
//        {
//            tower.UpgradeTower();
//            return true;
//        }
//        else
//        {
//            Debug.LogError("골드가 부족하거나 GoldManager.Instance가 NULL입니다!");
//            return false;
//        }
//    }
//}
