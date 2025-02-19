using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance { get; private set; }

    public GameObject upgradePanel;
    public Button upgradeButton;
    private Tower selectedTower;

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

    public void OpenUpgradeUI(Tower tower)
    {
        if (tower == null)
        {
            Debug.LogError("선택된 타워가 없습니다!");
            return;
        }

        selectedTower = tower;
        upgradePanel.SetActive(true);

        // 타워의 오른쪽 위에 UI 배치
        Vector3 newPosition = selectedTower.transform.position + new Vector3(1f, 1f, 0f);
        upgradePanel.transform.position = newPosition;

        Debug.Log($"업그레이드 UI가 {tower.name} 위에 나타남.");
    }

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
            Debug.Log($"{selectedTower.towerStats.Name} 업그레이드 성공!");
        }
        else
        {
            Debug.LogError("업그레이드 실패!");
        }
    }


    public void CloseUpgradeUI()
    {
        upgradePanel.SetActive(false);
    }
}
