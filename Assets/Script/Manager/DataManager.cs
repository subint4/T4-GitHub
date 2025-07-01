<<<<<<< Updated upstream
using UnityEngine;

public class DataManager
{
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataManager();
                instance.Initialize();
            }
            return instance;
        }
    }

    public EnemyDataManager EnemyDataManager { get; private set; }
    public WaveDataManager WaveDataManager { get; private set; }
    public TowerDataManager TowerDataManager { get; private set; }
    public StageDataManager StageDataManager { get; private set; }

    private DataManager() { }

    private void Initialize()
    {
        try
        {
            Debug.Log("[DataManager] Îç∞Ïù¥ÌÑ∞ Îß§ÎãàÏ†Ä Ï¥àÍ∏∞Ìôî ÏãúÏûë...");

            // SO Í∏∞Î∞ò Îç∞Ïù¥ÌÑ∞ Î°úÎìú
            EnemyDataManager = new EnemyDataManager();
            TowerDataManager = new TowerDataManager();
            WaveDataManager = new WaveDataManager();

            EnemyDataManager.LoadData();
            TowerDataManager.LoadData();
            WaveDataManager.LoadWaveDataFromJSON();

            Debug.Log("[DataManager] SO Í∏∞Î∞ò Îç∞Ïù¥ÌÑ∞ Î°úÎìú ÏôÑÎ£å.");

            // JSON Í∏∞Î∞ò Îç∞Ïù¥ÌÑ∞ Î°úÎìú (StageDataManager)
            StageDataManager = new StageDataManager();
            StageDataManager.LoadStageData();

            Debug.Log("[DataManager] JSON Í∏∞Î∞ò Îç∞Ïù¥ÌÑ∞ Î°úÎìú ÏôÑÎ£å.");

            Debug.Log("[DataManager] Î™®Îì† Îç∞Ïù¥ÌÑ∞ Îß§ÎãàÏ†ÄÍ∞Ä Ï†ïÏÉÅÏ†ÅÏúºÎ°ú Ï¥àÍ∏∞ÌôîÎêòÏóàÏäµÎãàÎã§.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[DataManager] Ï¥àÍ∏∞Ìôî Ï§ë Ïò§Î•ò Î∞úÏÉù: {ex.Message}");
        }
=======
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

            // Resources ∆˙¥ıø°º≠ ¿⁄µø¿∏∑Œ µ•¿Ã≈Õ ∑ŒµÂ
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
        Debug.Log("µ•¿Ã≈Õ ∏≈¥œ¿˙ √ ±‚»≠ Ω√¿€...");

        // ¿˚ µ•¿Ã≈Õ ∑ŒµÂ
        enemyDataDictionary.Clear();
        foreach (var enemy in enemyDataList)
        {
            if (enemy == null)
            {
                Debug.LogError("EnemySO ∏Ò∑œø° null ∞™¿Ã ¿÷Ω¿¥œ¥Ÿ.");
                continue;
            }

            if (!enemyDataDictionary.ContainsKey(enemy.EnemyID))
            {
                enemyDataDictionary.Add(enemy.EnemyID, enemy);
                Debug.Log($"EnemySO ∑ŒµÂ: {enemy.UnitName} (ID: {enemy.EnemyID})");
            }
        }

        // ≈∏øˆ µ•¿Ã≈Õ ∑ŒµÂ
        towerDataDictionary.Clear();
        foreach (var tower in towerDataList)
        {
            if (tower == null)
            {
                Debug.LogError("TowerSO ∏Ò∑œø° null ∞™¿Ã ¿÷Ω¿¥œ¥Ÿ.");
                continue;
            }

            if (!towerDataDictionary.ContainsKey(tower.UnitName))
            {
                towerDataDictionary.Add(tower.UnitName, tower);
                Debug.Log($"TowerSO ∑ŒµÂ: {tower.UnitName}");
            }
        }

        // ø˛¿Ã∫Í µ•¿Ã≈Õ ∑ŒµÂ
        waveDataDictionary.Clear();
        foreach (var wave in waveDataList)
        {
            if (wave == null)
            {
                Debug.LogError("WaveSO ∏Ò∑œø° null ∞™¿Ã ¿÷Ω¿¥œ¥Ÿ.");
                continue;
            }

            if (!waveDataDictionary.ContainsKey(wave.waveCount))
            {
                waveDataDictionary.Add(wave.waveCount, wave);
                Debug.Log($"ø˛¿Ã∫Í ∑ŒµÂ øœ∑·: Wave {wave.waveCount} (¿˚ ºˆ: {wave.enemyCounts.Count})");
            }
        }

        // ¿˚ «¡∏Æ∆’ ¿⁄µø ∑ŒµÂ
        enemyPrefabDictionary.Clear();
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy");

        if (loadedPrefabs.Length == 0)
        {
            Debug.LogError("EnemyPrefabs∞° ∑ŒµÂµ«¡ˆ æ æ“Ω¿¥œ¥Ÿ. Resources/Prefabs/Enemy ∆˙¥ı∏¶ »Æ¿Œ«œººø‰.");
        }

        foreach (var prefab in loadedPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogError("EnemyPrefab ∏Ò∑œø° null ∞™¿Ã ¿÷Ω¿¥œ¥Ÿ.");
                continue;
            }

            if (!enemyPrefabDictionary.ContainsKey(prefab.name))
            {
                enemyPrefabDictionary.Add(prefab.name, prefab);
                Debug.Log($"EnemyPrefab ¿⁄µø ∑ŒµÂ: {prefab.name}");
            }
        }

        Debug.Log($"√÷¡æ µÓ∑œµ» EnemyPrefab ∞≥ºˆ: {enemyPrefabDictionary.Count}");
    }

    // ¿˚ µ•¿Ã≈Õ∏¶ EnemyID ±‚¡ÿ¿∏∑Œ ∞°¡Æø¿±‚
    public EnemySO GetEnemyData(int enemyID)
    {
        if (!enemyDataDictionary.ContainsKey(enemyID))
        {
            Debug.LogError($"EnemyID {enemyID} µ•¿Ã≈Õ∏¶ √£¿ª ºˆ æ¯Ω¿¥œ¥Ÿ.");
            return null;
        }

        return enemyDataDictionary[enemyID];
    }

    // ≈∏øˆ µ•¿Ã≈Õ∏¶ UnitName ±‚¡ÿ¿∏∑Œ ∞°¡Æø¿±‚
    public TowerSO GetTowerData(string unitName)
    {
        if (!towerDataDictionary.ContainsKey(unitName))
        {
            Debug.LogError($"≈∏øˆ {unitName} µ•¿Ã≈Õ∏¶ √£¿ª ºˆ æ¯Ω¿¥œ¥Ÿ.");
            return null;
        }

        return towerDataDictionary[unitName];
    }

    // ø˛¿Ã∫Í µ•¿Ã≈Õ∏¶ waveCount ±‚¡ÿ¿∏∑Œ ∞°¡Æø¿±‚
    public WaveSO GetWaveData(int waveCount)
    {
        if (!waveDataDictionary.ContainsKey(waveCount))
        {
            Debug.LogError($"ø˛¿Ã∫Í {waveCount} µ•¿Ã≈Õ∏¶ √£¿ª ºˆ æ¯Ω¿¥œ¥Ÿ. µÓ∑œµ» ø˛¿Ã∫Í ∞≥ºˆ: {waveDataDictionary.Count}");
            foreach (var key in waveDataDictionary.Keys)
            {
                Debug.Log($"µÓ∑œµ» ø˛¿Ã∫Í: {key}");
            }
            return null;
        }

        return waveDataDictionary[waveCount];
    }

    // ¿˚ «¡∏Æ∆’¿ª UnitName ±‚¡ÿ¿∏∑Œ ∞°¡Æø¿±‚
    public GameObject GetEnemyPrefab(string unitName)
    {
        if (!enemyPrefabDictionary.ContainsKey(unitName))
        {
            Debug.LogError($"EnemyPrefab {unitName} µ•¿Ã≈Õ∏¶ √£¿ª ºˆ æ¯Ω¿¥œ¥Ÿ. µÓ∑œµ» «¡∏Æ∆’ ∞≥ºˆ: {enemyPrefabDictionary.Count}");
            foreach (var key in enemyPrefabDictionary.Keys)
            {
                Debug.Log($"µÓ∑œµ» «¡∏Æ∆’: {key}");
            }
            return null;
        }

        return enemyPrefabDictionary[unitName];
>>>>>>> Stashed changes
    }
}
