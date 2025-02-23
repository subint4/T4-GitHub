using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour, IPointerClickHandler
{
    public int towerID;
    private int towerCost;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button buttonComponent;

    private static List<int> assignedTowerIDs;
    private bool isSelected = false;

    // 현재 선택된 버튼을 추적하는 static 변수
    private static TowerButton selectedButton = null;

    private void Start()
    {
        AssignTowerIDByUnitDataOrder();
        UpdateCostText();

        if (buttonComponent == null)
        {
            buttonComponent = GetComponent<Button>();
        }
    }

    private void AssignTowerIDByUnitDataOrder()
    {
        if (TowerManager.Instance == null)
        {
            Debug.LogError("TowerManager가 존재하지 않습니다!");
            return;
        }

        if (assignedTowerIDs == null)
        {
            assignedTowerIDs = TowerManager.Instance.GetAllLevel1TowerIDs();
        }

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

        buttons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        int buttonIndex = buttons.IndexOf(this);
        if (buttonIndex >= 0 && buttonIndex < assignedTowerIDs.Count)
        {
            towerID = assignedTowerIDs[buttonIndex];
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

    public void CancelSelection()
    {
        isSelected = false;
        UpdateButtonVisual(false);
        selectedButton = null;
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

        // 현재 선택된 버튼이 있고, 자신이 아닐 경우 기존 버튼 해제
        if (selectedButton != null && selectedButton != this)
        {
            selectedButton.CancelSelection();
        }

        if (isSelected)
        {
            TowerManager.Instance.CancelTowerSelection();
            isSelected = false;
            selectedButton = null;
            Debug.Log($"[TowerButton] 타워 {towerID} 선택 취소됨");
        }
        else
        {
            // 여기에서 골드 차감을 제거 → 선택만 수행
            TowerManager.Instance.SelectTower(towerID);
            isSelected = true;
            selectedButton = this;
            Debug.Log($"[TowerButton] 타워 {towerID} 선택됨 (골드 차감 없음)");
        }

        UpdateButtonVisual(isSelected);
    }


    private void UpdateButtonVisual(bool selected)
    {
        if (buttonComponent != null)
        {
            buttonComponent.interactable = !selected; // 선택되면 비활성화, 취소되면 활성화
        }
    }

    public void DisableButton()
    {
        if (buttonComponent != null)
        {
            buttonComponent.interactable = false;
        }
    }
}
