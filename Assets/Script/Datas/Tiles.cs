using UnityEngine;
using UnityEngine.EventSystems;

public class Tiles : MonoBehaviour, IPointerClickHandler
{
    public bool isOccupied = false;
    public Tower currentTower = null;
    public int tileIndex; // 타일의 고유 인덱스 (1부터 시작)
    
    public DamageItemManager item;
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
    void OnMouseDown()
    {
        // 아이템 선택 중이라면 타워 클릭 로직 무시
        if (DamageItemManager.Instance.IsItemSelected)
        {
            DamageItemManager.Instance.UseItemOnTile(transform.position);
            return;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TowerManager.Instance == null)
        {
            Debug.LogError("[Tiles] TowerManager가 존재하지 않습니다! 씬에서 활성화되어 있는지 확인하세요.");
            return;
        }

        int selectedTowerID = TowerManager.Instance.GetSelectedTowerID();
        if (selectedTowerID == -1)
        {
            Debug.LogWarning("[Tiles] 선택된 타워가 없습니다! 먼저 타워를 선택하세요.");
            return;
        }

        if (isOccupied)
        {
            Debug.LogWarning($"[Tiles] 타워 배치 불가: {transform.position} (이미 점유됨)");
            return;
        }

        int towerCost = TowerManager.Instance.GetTowerDeployCost(selectedTowerID);
        if (!GoldManager.Instance.SpendGold(towerCost))
        {
            Debug.LogError($"[Tiles] 골드 부족! (필요: {towerCost}, 보유: {GoldManager.Instance.GetGold()})");
            return;
        }

        // **타워 프리팹 가져오기**
        GameObject towerPrefab = TowerManager.Instance.GetTowerPrefab(selectedTowerID)?.gameObject;
        if (towerPrefab == null)
        {
            Debug.LogError($"[Tiles] 선택된 타워 ID {selectedTowerID}에 해당하는 프리팹을 찾을 수 없습니다.");
            return;
        }

        // **타워 배치**
        GameObject newTowerObj = Instantiate(towerPrefab, transform.position, Quaternion.identity);
        Tower newTower = newTowerObj.GetComponent<Tower>();

        if (newTower == null)
        {
            Debug.LogError("[Tiles] 생성된 오브젝트에서 Tower 컴포넌트를 찾을 수 없습니다! 프리팹에 Tower 컴포넌트가 있는지 확인하세요.");
            return;
        }

        PlaceTower(newTower);
    }


    public void PlaceTower(Tower tower)
    {
        isOccupied = true;
        currentTower = tower;
        tower.currentTile = this;
        Debug.Log($"[Tiles] 타워 배치 완료: {transform.position}");
    }
}
