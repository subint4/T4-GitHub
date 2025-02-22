using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class TowerButton : MonoBehaviour, IPointerClickHandler
{
    public int towerID; // 자동으로 할당될 타워 ID
    private int towerCost;
    [SerializeField] private TMP_Text costText;

    private static List<int> assignedTowerIDs; // 중복 방지를 위한 리스트

    private void Start()
    {
        AssignTowerIDByUnitDataOrder(); // 버튼에 ID 배정
        UpdateCostText();
    }

    private void AssignTowerIDByUnitDataOrder()
    {
        if (TowerManager.Instance == null)
        {
            Debug.LogError("TowerManager가 존재하지 않습니다!");
            return;
        }

        // 최초 1회만 Level 1 타워 ID 가져와서 중복 없이 리스트 저장
        if (assignedTowerIDs == null)
        {
            assignedTowerIDs = TowerManager.Instance.GetAllLevel1TowerIDs();
        }

        // 부모 컨테이너에서 모든 버튼을 가져와 정렬 (왼쪽 → 오른쪽, 위 → 아래)
        Transform parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("부모 Transform이 존재하지 않습니다!");
            return;
        }

        List<TowerButton> buttons = new List<TowerButton>();
        foreach (Transform child in parentTransform)
        {
            TowerButton button = child.GetComponent<TowerButton>();
            if (button != null)
            {
                buttons.Add(button);
            }
        }

        // **Hierarchy에서 왼쪽 -> 오른쪽, 위 -> 아래 순서로 정렬**
        buttons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        // 현재 버튼의 Index 찾기
        int buttonIndex = buttons.IndexOf(this);

        if (buttonIndex >= 0 && buttonIndex < assignedTowerIDs.Count)
        {
            towerID = assignedTowerIDs[buttonIndex]; // 리스트에서 Index 순서대로 가져오기
            towerCost = TowerManager.Instance.GetTowerDeployCost(towerID);
            Debug.Log($"버튼 {gameObject.name}에 Tower ID {towerID} 할당 (Index: {buttonIndex})");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} 버튼의 인덱스 {buttonIndex}가 Level 1 타워 목록보다 큽니다.");
        }
    }
    public int GetTowerID()
    {
        return towerID;
    }


    private void UpdateCostText()
    {
        if (costText != null)
        {
            costText.text = towerCost.ToString();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (TowerManager.Instance == null)
        {
            Debug.LogError("[TowerButton] TowerManager가 존재하지 않습니다!");
            return;
        }

        if (towerCost == int.MaxValue)
        {
            Debug.LogError($"[TowerButton] Tower ID {towerID}의 비용 정보가 없습니다! 배치 불가");
            return;
        }

        if (GoldManager.Instance.SpendGold(towerCost))
        {
            TowerManager.Instance.SelectTower(towerID);
            Debug.Log($"[TowerButton] 타워 {towerID} 선택됨, 비용 {towerCost} 차감됨");
        }
        else
        {
            Debug.Log("[TowerButton] 골드 부족!");
        }
    }
}
