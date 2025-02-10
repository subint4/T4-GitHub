using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [SerializeField] private List<EnemySO> enemyDataList = new List<EnemySO>();
    [SerializeField] private List<TowerSO> towerDataList = new List<TowerSO>();
    [SerializeField] private List<WaveSO> waveDataList = new List<WaveSO>();

    private Dictionary<int, EnemySO> enemyDataDictionary = new Dictionary<int, EnemySO>();
    private Dictionary<int, WaveSO> waveDataDictionary = new Dictionary<int, WaveSO>();
    private Dictionary<string, TowerSO> towerDataDictionary = new Dictionary<string, TowerSO>();
    private Dictionary<string, GameObject> enemyPrefabDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Resources 폴더에서 자동으로 데이터 로드
            enemyDataList = new List<EnemySO>(Resources.LoadAll<EnemySO>("EnemySO"));
            towerDataList = new List<TowerSO>(Resources.LoadAll<TowerSO>("TowerSO"));
            waveDataList = new List<WaveSO>(Resources.LoadAll<WaveSO>("WaveSO"));

            InitializeData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeData()
    {
        Debug.Log("데이터 매니저 초기화 시작...");

        // 적 데이터 로드
        enemyDataDictionary.Clear();
        foreach (var enemy in enemyDataList)
        {
            if (enemy == null)
            {
                Debug.LogError("EnemySO 목록에 null 값이 있습니다.");
                continue;
            }

            if (!enemyDataDictionary.ContainsKey(enemy.EnemyID))
            {
                enemyDataDictionary.Add(enemy.EnemyID, enemy);
                Debug.Log($"EnemySO 로드: {enemy.UnitName} (ID: {enemy.EnemyID})");
            }
        }

        // 타워 데이터 로드
        towerDataDictionary.Clear();
        foreach (var tower in towerDataList)
        {
            if (tower == null)
            {
                Debug.LogError("TowerSO 목록에 null 값이 있습니다.");
                continue;
            }

            if (!towerDataDictionary.ContainsKey(tower.UnitName))
            {
                towerDataDictionary.Add(tower.UnitName, tower);
                Debug.Log($"TowerSO 로드: {tower.UnitName}");
            }
        }

        // 웨이브 데이터 로드
        waveDataDictionary.Clear();
        foreach (var wave in waveDataList)
        {
            if (wave == null)
            {
                Debug.LogError("WaveSO 목록에 null 값이 있습니다.");
                continue;
            }

            if (!waveDataDictionary.ContainsKey(wave.waveCount))
            {
                waveDataDictionary.Add(wave.waveCount, wave);
                Debug.Log($"웨이브 로드 완료: Wave {wave.waveCount} (적 수: {wave.enemyCounts.Count})");
            }
        }

        // 적 프리팹 자동 로드
        enemyPrefabDictionary.Clear();
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy");

        if (loadedPrefabs.Length == 0)
        {
            Debug.LogError("EnemyPrefabs가 로드되지 않았습니다. Resources/Prefabs/Enemy 폴더를 확인하세요.");
        }

        foreach (var prefab in loadedPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogError("EnemyPrefab 목록에 null 값이 있습니다.");
                continue;
            }

            if (!enemyPrefabDictionary.ContainsKey(prefab.name))
            {
                enemyPrefabDictionary.Add(prefab.name, prefab);
                Debug.Log($"EnemyPrefab 자동 로드: {prefab.name}");
            }
        }

        Debug.Log($"최종 등록된 EnemyPrefab 개수: {enemyPrefabDictionary.Count}");
    }

    // 적 데이터를 EnemyID 기준으로 가져오기
    public EnemySO GetEnemyData(int enemyID)
    {
        if (!enemyDataDictionary.ContainsKey(enemyID))
        {
            Debug.LogError($"EnemyID {enemyID} 데이터를 찾을 수 없습니다.");
            return null;
        }

        return enemyDataDictionary[enemyID];
    }

    // 타워 데이터를 UnitName 기준으로 가져오기
    public TowerSO GetTowerData(string unitName)
    {
        if (!towerDataDictionary.ContainsKey(unitName))
        {
            Debug.LogError($"타워 {unitName} 데이터를 찾을 수 없습니다.");
            return null;
        }

        return towerDataDictionary[unitName];
    }

    // 웨이브 데이터를 waveCount 기준으로 가져오기
    public WaveSO GetWaveData(int waveCount)
    {
        if (!waveDataDictionary.ContainsKey(waveCount))
        {
            Debug.LogError($"웨이브 {waveCount} 데이터를 찾을 수 없습니다. 등록된 웨이브 개수: {waveDataDictionary.Count}");
            foreach (var key in waveDataDictionary.Keys)
            {
                Debug.Log($"등록된 웨이브: {key}");
            }
            return null;
        }

        return waveDataDictionary[waveCount];
    }
    public float GetWaveInterval(int waveIndex)
    {
        WaveSO wave = GetWaveData(waveIndex);
        return wave != null ? wave.timeBetweenWaves : 5f; // 기본값 5초
    }

    // 적 프리팹을 UnitName 기준으로 가져오기
    public GameObject GetEnemyPrefab(string unitName)
    {
        if (!enemyPrefabDictionary.ContainsKey(unitName))
        {
            Debug.LogError($"EnemyPrefab {unitName} 데이터를 찾을 수 없습니다. 등록된 프리팹 개수: {enemyPrefabDictionary.Count}");
            foreach (var key in enemyPrefabDictionary.Keys)
            {
                Debug.Log($"등록된 프리팹: {key}");
            }
            return null;
        }

        return enemyPrefabDictionary[unitName];
    }
}
