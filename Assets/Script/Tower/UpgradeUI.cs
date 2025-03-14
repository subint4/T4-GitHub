using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance { get; private set; }

    public GameObject upgradePanel;
    public Button upgradeButton;
    public TextMeshProUGUI upgradeCostText;
    private Tower selectedTower;
    private Canvas upgradeCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (upgradePanel == null) Debug.LogError("Upgrade Panel이 설정되지 않았습니다!");
        if (upgradeButton == null) Debug.LogError("Upgrade Button이 설정되지 않았습니다!");
        if (upgradeCostText == null) Debug.LogError("[UpgradeUI] UpgradeCostText가 설정되지 않았습니다!");

        upgradeCanvas = upgradePanel.GetComponent<Canvas>();
        if (upgradeCanvas == null)
        {
            Debug.LogError("[UpgradeUI] Canvas가 없습니다! UI 계층을 조정하려면 Canvas가 필요합니다.");
        }
    }

    private void Start()
    {
        upgradePanel.SetActive(false);
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(UpgradeSelectedTower);
        }
    }

    /// <summary>
    /// 타워 선택 시 업그레이드 UI 표시
    /// </summary>
    public void OpenUpgradeUI(Tower tower)
    {
        if (tower == null)
        {
            Debug.LogError("[UpgradeUI] 선택된 타워가 없습니다!");
            return;
        }

        // 같은 타워를 다시 클릭하면 UI 닫기
        if (selectedTower == tower && upgradePanel.activeSelf)
        {
            CloseUpgradeUI();
            return;
        }

        selectedTower = tower;
        upgradePanel.SetActive(true);

        // Canvas가 비활성화 상태일 경우 활성화
        if (upgradeCanvas != null)
        {
            upgradeCanvas.enabled = true;
            upgradeCanvas.sortingOrder = 50; // UI를 최상단에 표시
        }

        // UI를 타워의 우측 대각선(0.5f, 0.5f) 위치에 배치
        Vector3 newPosition = tower.transform.position + new Vector3(0.5f, 0.5f, 0f);

        // RectTransform을 사용하여 UI 배치
        RectTransform rectTransform = upgradePanel.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.position = newPosition;
        }

        UpdateUpgradeCostUI();

        Debug.Log($"[UpgradeUI] 업그레이드 UI가 {tower.name} 우측 대각선에 나타남. (위치: {newPosition})");
    }

    private TowerSO GetNextLevelStats()
    {
        if (selectedTower == null || selectedTower.towerStats == null)
        {
            Debug.LogError("[UpgradeUI] 선택된 타워 데이터가 없습니다! 다음 레벨을 가져올 수 없음.");
            return null;
        }

        int nextLevelID = selectedTower.towerStats.NextLevelID;
        return (nextLevelID > 0) ? DataManager.Instance.TowerDataManager.GetTowerData(nextLevelID) : null;
    }

    /// <summary>
    /// 업그레이드 비용 UI 업데이트
    /// </summary>
    private void UpdateUpgradeCostUI()
    {
        if (selectedTower == null || selectedTower.towerStats == null)
        {
            Debug.LogWarning("[UpgradeUI] 선택된 타워 데이터가 없습니다. UI 업데이트 중단.");
            upgradeButton.interactable = false;
            upgradeCostText.text = "업그레이드 불가";
            return;
        }
        TowerSO nextLevelStats = GetNextLevelStats();

        // 다음 레벨이 없으면 업그레이드 불가 처리
        if (nextLevelStats == null)
        {
            Debug.Log($"[UpgradeUI] {selectedTower.towerStats.Name}은(는) 최대 레벨입니다. 업그레이드 불가.");
            upgradeButton.interactable = false;
            upgradeCostText.text = "최대 레벨 도달";
            return;
        }

        // 다음 레벨의 업그레이드 비용을 UI에 표시
        int upgradeCost = nextLevelStats.UpgradeCost;
        upgradeCostText.text = $"{upgradeCost}";

        // 업그레이드 가능 여부에 따라 버튼 활성화
        upgradeButton.interactable = selectedTower.CanUpgrade();
    }


    /// <summary>
    /// 업그레이드 버튼 클릭 시 실행
    /// </summary>
    public void UpgradeSelectedTower()
    {
        if (selectedTower == null)
        {
            Debug.LogError("[UpgradeUI] 업그레이드할 타워가 선택되지 않았습니다.");
            return;
        }
        TowerSO nextLevelStats = GetNextLevelStats();

        int upgradeCost = nextLevelStats.UpgradeCost;

        // 골드 확인 및 차감
        if (!GoldManager.Instance.SpendGold(upgradeCost))
        {
            Debug.LogError("[UpgradeUI] 골드 부족! 업그레이드 실패.");
            return;
        }

        // 타워의 업그레이드 함수 직접 호출
        selectedTower.UpgradeTower();

        Debug.Log($"[UpgradeUI] {selectedTower.towerStats.Name} 업그레이드 성공!");

        OpenUpgradeUI(selectedTower); // 업그레이드 후 UI 갱신
    }

    /// <summary>
    /// UI 닫기 및 선택 해제
    /// </summary>
    public void CloseUpgradeUI()
    {
        upgradePanel.SetActive(false);
        selectedTower = null;
        Debug.Log("[UpgradeUI] 업그레이드 UI 닫힘.");
    }
}
