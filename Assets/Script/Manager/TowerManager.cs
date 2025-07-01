using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TowerManager;

public class TowerManager : MonoBehaviour, IResettableManager
{
    public interface IResettableManager
    {
        void ResetManager();
    }
    public static TowerManager Instance { get; private set; }

    private Dictionary<int, GameObject> towerPrefabDictionary = new Dictionary<int, GameObject>();
    private int selectedTowerID = -1;
    private List<Tower> activeTowers = new List<Tower>();

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
        Debug.Log("TowerManager: 모든 타워 프리팹을 불러옵니다.");

        GameObject[] towerPrefabs = Resources.LoadAll<GameObject>("Prefabs/Towers");
        foreach (var prefab in towerPrefabs)
        {
            Tower towerComponent = prefab.GetComponent<Tower>();
            if (towerComponent == null)
            {
                Debug.LogError($"{prefab.name} 프리팹에서 Tower 컴포넌트를 찾을 수 없습니다!");
                continue;
            }

            TowerSO towerSO = DataManager.Instance.TowerDataManager.GetTowerData(towerComponent.TowerID);
            if (towerSO == null)
            {
                Debug.LogError($"{prefab.name} 프리팹에 해당하는 TowerSO (ID: {towerComponent.TowerID})를 찾을 수 없습니다!");
                continue;
            }

            towerPrefabDictionary[towerSO.ID] = prefab;
            Debug.Log($"Tower ID {towerSO.ID} - {prefab.name} 프리팹과 자동 연결됨.");
        }
    }

    public TowerSO GetTowerData(int towerID)
    {
        return DataManager.Instance.TowerDataManager.GetTowerData(towerID);
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

    public int GetTowerDeployCost(int towerID)
    {
        TowerSO towerData = GetTowerData(towerID);
        return towerData != null ? towerData.DeployCost : int.MaxValue;
    }

    public void SelectTower(int towerID)
    {
        if (DataManager.Instance.TowerDataManager.GetTowerData(towerID) != null)
        {
            selectedTowerID = towerID;
            Debug.Log($"[TowerManager] 타워 {towerID} 선택됨.");
        }
        else
        {
            Debug.LogError($"[TowerManager] 선택한 타워 {towerID}가 존재하지 않습니다!");
        }
    }

    public int GetSelectedTowerID()
    {
        return selectedTowerID;
    }

    public List<int> GetAllLevel1TowerIDs()
    {
        return DataManager.Instance.TowerDataManager.GetAllLevel1TowerIDs();
    }

    public void SpawnTower(Tiles tile)
    {
        if (selectedTowerID == -1)
        {
            Debug.LogError("[TowerManager] 타워가 선택되지 않았습니다!");
            return;
        }

        GameObject towerPrefab = GetTowerPrefab(selectedTowerID);
        if (towerPrefab == null)
        {
            Debug.LogError($"[TowerManager] ID {selectedTowerID}의 타워 프리팹을 찾을 수 없습니다.");
            return;
        }

        // 타일 위치에 타워 생성
        GameObject newTowerObj = Instantiate(towerPrefab, tile.transform.position, Quaternion.identity);
        Tower newTower = newTowerObj.GetComponent<Tower>();

        if (newTower != null)
        {
            newTower.Initialize(GetTowerData(selectedTowerID));
            activeTowers.Add(newTower);

            // 타일을 점유하도록 설정
            tile.PlaceTower(newTower);

            Debug.Log($"[TowerManager] 타워 배치 완료: ID {selectedTowerID}, 위치 {tile.transform.position}");
        }
        else
        {
            Debug.LogError("[TowerManager] 생성된 타워에 Tower 컴포넌트가 없습니다!");
            Destroy(newTowerObj);
        }
    }



    public void CancelTowerSelection()
    {
        selectedTowerID = -1;
        Debug.Log("[TowerManager] 타워 선택 취소됨");
    }

    public void RemoveTower(Tower tower)
    {
        if (activeTowers.Contains(tower))
        {
            activeTowers.Remove(tower);
            Destroy(tower.gameObject);
            Debug.Log($"[TowerManager] 타워 제거 완료: {tower.TowerID}");
        }
    }

    public int GetActiveTowerCount()
    {
        return activeTowers.Count;
    }
    public void ResetManager()
    {
        selectedTowerID = -1;

        foreach (Tower tower in activeTowers)
        {
            if (tower != null)
            {
                Destroy(tower.gameObject);
            }
        }
        activeTowers.Clear();

        Debug.Log("[TowerManager] 리셋 완료: 선택한 타워 및 배치된 타워 제거됨");
    }

}
