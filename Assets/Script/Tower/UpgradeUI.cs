using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance { get; private set; }

    public GameObject upgradePanel;
    public Button upgradeButton;
    private Tower selectedTower;

    private Canvas upgradeCanvas; // 업그레이드 UI가 속한 Canvas 참조

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

        if (upgradePanel == null)
        {
            Debug.LogError("Upgrade Panel이 설정되지 않았습니다!");
        }

        if (upgradeButton == null)
        {
            Debug.LogError("Upgrade Button이 설정되지 않았습니다!");
        }

        // **업그레이드 UI의 Canvas 가져오기**
        upgradeCanvas = upgradePanel.GetComponent<Canvas>();
        if (upgradeCanvas == null)
        {
            Debug.LogError("[UpgradeUI] Canvas가 없습니다! UI 계층을 조정하려면 Canvas가 필요합니다.");
        }
    }

    private void Start()
    {
        upgradePanel.SetActive(false); // UI 초기 상태 비활성화

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(UpgradeSelectedTower);
        }
    }

    /// <summary>
    /// **타워 선택 시 업그레이드 UI 열기 (한 번 더 클릭하면 닫기)**
    /// </summary>
    public void OpenUpgradeUI(Tower tower)
    {
        if (tower == null)
        {
            Debug.LogError("선택된 타워가 없습니다!");
            return;
        }

        // **같은 타워를 다시 클릭하면 선택 취소**
        if (selectedTower == tower)
        {
            Debug.Log($"[UpgradeUI] {tower.name} 선택 해제됨.");
            CloseUpgradeUI();
            return;
        }

        selectedTower = tower;
        upgradePanel.SetActive(true);

        // **타워의 오른쪽 위에 UI 배치**
        Vector3 newPosition = selectedTower.transform.position + new Vector3(.5f, .5f, 0f);
        upgradePanel.transform.position = newPosition;

        // **업그레이드 UI를 다른 UI 아래로 이동**
        if (upgradeCanvas != null)
        {
            upgradeCanvas.sortingOrder = 1; // 다른 UI보다 아래에 배치
        }
        upgradePanel.transform.SetAsFirstSibling(); // UI 계층에서 맨 아래로 이동

        Debug.Log($"[UpgradeUI] 업그레이드 UI가 {tower.name} 위에 나타남.");
    }

    /// <summary>
    /// **업그레이드 버튼 클릭 시 타워 업그레이드 실행**
    /// </summary>
    public void UpgradeSelectedTower()
    {
        if (UpgradeManager.Instance == null)
        {
            Debug.LogError("UpgradeManager가 존재하지 않습니다. 씬에 추가되었는지 확인하세요.");
            return;
        }

        if (selectedTower == null)
        {
            Debug.LogError("업그레이드할 타워가 선택되지 않았습니다.");
            return;
        }

        bool success = UpgradeManager.Instance.UpgradeTower(selectedTower);
        if (success)
        {
            Debug.Log($"[UpgradeUI] {selectedTower.towerStats.Name} 업그레이드 성공!");
        }
        else
        {
            Debug.LogError("[UpgradeUI] 업그레이드 실패!");
        }
    }

    /// <summary>
    /// **UI 닫기 및 타워 선택 해제**
    /// </summary>
    public void CloseUpgradeUI()
    {
        upgradePanel.SetActive(false);
        selectedTower = null;
        Debug.Log("[UpgradeUI] 업그레이드 UI 닫힘.");
    }
}
