using UnityEngine;
using UnityEngine.EventSystems;

public class Tiles : MonoBehaviour, IPointerClickHandler
{
    public bool isOccupied = false;
    public Tower currentTower = null;
    public int tileIndex; // 타일의 고유 인덱스 (1부터 시작)

    private void Start()
    {
        AssignTileIndex(); // 타일 인덱스 자동 배치
    }

    private void AssignTileIndex()
    {
        Transform parentTransform = transform.parent;

        if (parentTransform == null)
        {
            Debug.LogError("[Tiles] 부모 Transform을 찾을 수 없습니다!");
            return;
        }

        // 부모의 모든 자식(Tiles)을 가져와 순서대로 정렬
        Tiles[] allTiles = parentTransform.GetComponentsInChildren<Tiles>();

        // Hierarchy 순서대로 정렬
        System.Array.Sort(allTiles, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        // 정렬된 순서대로 1부터 인덱스 할당
        for (int i = 0; i < allTiles.Length; i++)
        {
            allTiles[i].tileIndex = i + 1; // 1부터 시작
            Debug.Log($"[Tiles] 타일 인덱스 설정됨 - Name: {allTiles[i].gameObject.name}, Index: {allTiles[i].tileIndex}");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // **아이템이 선택된 경우 먼저 체크**
        if (!string.IsNullOrEmpty(DamageItemManager.Instance.GetSelectedItemName()))
        {
            Debug.Log($"[Tiles] 아이템 사용 시도 - 타일 {tileIndex}에서 {DamageItemManager.Instance.GetSelectedItemName()} 실행");
            DamageItemManager.Instance.UseItemOnTile(transform.position);
            return;
        }

        // **아이템이 선택되지 않은 경우 타워 배치**
        if (TowerManager.Instance == null)
        {
            Debug.LogError("[Tiles] TowerManager가 존재하지 않습니다!");
            return;
        }

        int selectedTowerID = TowerManager.Instance.GetSelectedTowerID();
        if (selectedTowerID == -1)
        {
            Debug.LogWarning("[Tiles] 선택된 타워가 없습니다!");
            return;
        }

        if (isOccupied)
        {
            Debug.LogWarning($"[Tiles] 타워 배치 불가: {transform.position} (isOccupied: {isOccupied})");
            return;
        }

        int towerCost = TowerManager.Instance.GetTowerDeployCost(selectedTowerID);

        // 타워 배치 전 골드 차감 (차감 실패 시 배치 취소)
        if (!GoldManager.Instance.SpendGold(towerCost))
        {
            Debug.LogError($"[Tiles] 골드 부족! (필요: {towerCost}, 보유: {GoldManager.Instance.GetGold()})");
            return;
        }

        Debug.Log($"[Tiles] 타워 배치 시도: ID {selectedTowerID}, 위치 {transform.position}");

        // 타워 배치는 `TowerManager`에서 수행
        TowerManager.Instance.SpawnTower(this);
    }

    public void PlaceTower(Tower tower)
    {
        isOccupied = true;
        currentTower = tower;
        tower.currentTile = this;
        Debug.Log($"[Tiles] 타워 배치 완료: {transform.position}");
    }
}
