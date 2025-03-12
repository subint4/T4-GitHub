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

    private void Awake()
    {
        if (buttonComponent == null)
        {
            buttonComponent = GetComponent<Button>();
        }
    }

    private void Start()
    {
        AssignTowerIDByOrder();
        UpdateCostText();
    }

    private void AssignTowerIDByOrder()
    {
        if (TowerManager.Instance == null)
        {
            Debug.LogError("[TowerButton] TowerManager가 존재하지 않습니다!");
            return;
        }

        // **ID 리스트를 한 번만 로드**
        if (assignedTowerIDs == null)
        {
            assignedTowerIDs = TowerManager.Instance.GetAllLevel1TowerIDs();
        }

        // **현재 버튼이 속한 부모 안의 모든 버튼을 가져와 정렬**
        Transform parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("[TowerButton] 부모 Transform이 존재하지 않습니다!");
            return;
        }

        TowerButton[] buttons = parentTransform.GetComponentsInChildren<TowerButton>();
        System.Array.Sort(buttons, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        int buttonIndex = System.Array.IndexOf(buttons, this);
        if (buttonIndex >= 0 && buttonIndex < assignedTowerIDs.Count)
        {
            towerID = assignedTowerIDs[buttonIndex]; // 정렬된 순서대로 ID 배정
            towerCost = TowerManager.Instance.GetTowerDeployCost(towerID);
            Debug.Log($"[TowerButton] 버튼 {gameObject.name}에 Tower ID {towerID} 할당됨 (Index: {buttonIndex})");
        }
        else
        {
            Debug.LogWarning($"[TowerButton] {gameObject.name} 버튼의 Index({buttonIndex})가 ID 목록보다 큽니다.");
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
            TowerManager.Instance.SelectTower(towerID);
            isSelected = true;
            selectedButton = this;
            Debug.Log($"[TowerButton] 타워 {towerID} 선택됨");
        }

        UpdateButtonVisual(isSelected);
    }

    private void UpdateButtonVisual(bool selected)
    {
        if (buttonComponent != null)
        {
            buttonComponent.interactable = !selected;
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
