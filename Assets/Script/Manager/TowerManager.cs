using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    private Dictionary<int, GameObject> towerPrefabDictionary = new Dictionary<int, GameObject>();
    private Dictionary<int, TowerSO> towerDataDictionary = new Dictionary<int, TowerSO>();
    private int selectedTowerID = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeTowers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTowers()
    {
        Debug.Log("TowerManager: 모든 타워 프리팹과 SO 데이터를 불러옵니다.");

        TowerSO[] towerDataList = Resources.LoadAll<TowerSO>("TowerSO");
        foreach (var towerData in towerDataList)
        {
            towerDataDictionary[towerData.ID] = towerData;
        }

        GameObject[] towerPrefabs = Resources.LoadAll<GameObject>("Prefabs/Towers");
        foreach (var prefab in towerPrefabs)
        {
            Tower towerComponent = prefab.GetComponent<Tower>();
            if (towerComponent == null)
            {
                Debug.LogError($"{prefab.name} 프리팹에서 Tower 컴포넌트를 찾을 수 없습니다!");
                continue;
            }

            if (!towerDataDictionary.TryGetValue(towerComponent.TowerID, out TowerSO towerSO))
            {
                Debug.LogError($"{prefab.name} 프리팹에 해당하는 TowerSO (ID: {towerComponent.TowerID})를 찾을 수 없습니다!");
                continue;
            }

            towerComponent.Initialize(towerSO); // 자동 연결
            towerPrefabDictionary[towerComponent.TowerID] = prefab;
            Debug.Log($"Tower ID {towerComponent.TowerID} - {prefab.name} 프리팹과 자동 연결됨.");
        }
    }

    public GameObject GetTowerPrefab(int towerID)
    {
        if (towerPrefabDictionary.TryGetValue(towerID, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogError($"TowerManager: Tower ID {towerID}에 대한 프리팹을 찾을 수 없습니다!");
        return null;
    }

    public TowerSO GetTowerData(int towerID)
    {
        return towerDataDictionary.TryGetValue(towerID, out TowerSO towerData) ? towerData : null;
    }

    public List<int> GetAllLevel1TowerIDs()
    {
        return towerDataDictionary.Values
            .Where(tower => tower.Level == 1)
            .OrderBy(tower => tower.ID)
            .Select(tower => tower.ID)
            .ToList();
    }

    public void SelectTower(int towerID)
    {
        if (towerDataDictionary.ContainsKey(towerID))
        {
            selectedTowerID = towerID;
            Debug.Log($"타워 {towerID} 선택됨.");
        }
        else
        {
            Debug.LogError($"선택한 타워 {towerID}가 존재하지 않습니다!");
        }
    }

    public void PlaceSelectedTower(Tiles tile)
    {
        if (selectedTowerID == -1)
        {
            Debug.LogError("타워가 선택되지 않았습니다. 먼저 타워 버튼을 클릭하세요.");
            return;
        }

        SpawnTower(tile, selectedTowerID);
        selectedTowerID = -1;
    }

    public void SpawnTower(Tiles tile, int towerID)
    {
        if (!towerDataDictionary.ContainsKey(towerID) || !towerPrefabDictionary.ContainsKey(towerID))
        {
            Debug.LogError($"TowerManager: Tower ID {towerID}에 대한 데이터 또는 프리팹을 찾을 수 없습니다!");
            return;
        }

        GameObject towerPrefab = towerPrefabDictionary[towerID];
        TowerSO towerData = towerDataDictionary[towerID];

        Vector3 spawnPosition = tile.transform.position;
        GameObject newTowerObj = Instantiate(towerPrefab, spawnPosition, Quaternion.identity);
        Tower newTower = newTowerObj.GetComponent<Tower>();

        if (newTower != null)
        {
            newTower.Initialize(towerData);
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
