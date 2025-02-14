using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TowerButton : MonoBehaviour, IPointerClickHandler
{
    public int towerID; // 선택할 타워의 ID (직접 지정 가능)

    private void Start()
    {
        AssignTowerIDByHierarchy(); // 버튼 순서를 Hierarchy 기준으로 할당
    }

    // Hierarchy 순서대로 Tower ID 할당
    private void AssignTowerIDByHierarchy()
    {
        List<int> sortedTowerIDs = DataManager.GetAllLevel1TowerIDs(); // 1레벨 타워 ID 정렬
        int index = transform.GetSiblingIndex(); // **Hierarchy에서 버튼의 순서 가져오기**

        if (index < sortedTowerIDs.Count)
        {
            towerID = sortedTowerIDs[index]; // **버튼 순서대로 ID 매칭**
            Debug.Log($"버튼 {gameObject.name}에 Tower ID {towerID} 할당 (Index: {index})");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} 버튼의 인덱스 {index}가 타워 ID 목록보다 큽니다.");
        }
    }

    // 버튼 클릭 시 실행
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TowerManager.Instance != null)
        {
            TowerManager.Instance.SelectTower(towerID);
            Debug.Log($"버튼 클릭됨: 타워 ID {towerID} 선택됨");
        }
        else
        {
            Debug.LogError("TowerManager가 존재하지 않음!");
        }
    }

}
