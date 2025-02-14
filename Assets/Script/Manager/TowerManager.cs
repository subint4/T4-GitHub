using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    private int selectedTowerID = -1; // 현재 선택된 타워 ID (SO의 ID 값)
    private List<Tower> towers = new List<Tower>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // **타워 선택 (TowerSO의 ID 기반)**
    public void SelectTower(int towerID)
    {
        if (DataManager.GetTowerData(towerID) != null)
        {
            selectedTowerID = towerID;
            Debug.Log($"타워 {towerID} 선택됨.");
        }
        else
        {
            Debug.LogError($"선택한 타워 {towerID}가 존재하지 않습니다!");
        }
    }

    // **선택된 타워 배치 (타일에 배치)**
    public void PlaceSelectedTower(Tiles tile)
    {
        if (selectedTowerID == -1)
        {
            Debug.LogError("타워가 선택되지 않았습니다. 먼저 타워 버튼을 클릭하세요.");
            return;
        }

        SpawnTower(tile, selectedTowerID);
        selectedTowerID = -1; // 선택 해제
    }

    public void SpawnTower(Tiles tile, int towerID)
    {
        TowerSO towerData = DataManager.GetTowerData(towerID);
        if (towerData == null)
        {
            Debug.LogError($"TowerManager: 타워 데이터 ({towerID})를 찾을 수 없습니다!");
            return;
        }

        GameObject towerPrefab = DataManager.GetTowerPrefab(towerID); // ID 기반으로 프리팹 검색
        if (towerPrefab == null)
        {
            Debug.LogError($"TowerManager: TowerID {towerID}에 대한 프리팹을 찾을 수 없습니다!");
            return;
        }

        Vector3 spawnPosition = tile.transform.position;
        GameObject newTowerObj = Instantiate(towerPrefab, spawnPosition, Quaternion.identity);
        Tower newTower = newTowerObj.GetComponent<Tower>();

        if (newTower != null)
        {
            newTower.towerStats = towerData;
            newTower.currentTile = tile;
            tile.PlaceTower(newTower);
            Debug.Log($"타워 스폰 완료: {newTower.towerStats.Name} (ID: {towerID})");
        }
        else
        {
            Debug.LogError("TowerManager: 생성된 타워에 Tower 컴포넌트가 없습니다!");
        }
    }
}
