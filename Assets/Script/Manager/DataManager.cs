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

            // Resources �������� �ڵ����� ������ �ε�
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
        Debug.Log("������ �Ŵ��� �ʱ�ȭ ����...");

        // �� ������ �ε�
        enemyDataDictionary.Clear();
        foreach (var enemy in enemyDataList)
        {
            if (enemy == null)
            {
                Debug.LogError("EnemySO ��Ͽ� null ���� �ֽ��ϴ�.");
                continue;
            }

            if (!enemyDataDictionary.ContainsKey(enemy.EnemyID))
            {
                enemyDataDictionary.Add(enemy.EnemyID, enemy);
                Debug.Log($"EnemySO �ε�: {enemy.UnitName} (ID: {enemy.EnemyID})");
            }
        }

        // Ÿ�� ������ �ε�
        towerDataDictionary.Clear();
        foreach (var tower in towerDataList)
        {
            if (tower == null)
            {
                Debug.LogError("TowerSO ��Ͽ� null ���� �ֽ��ϴ�.");
                continue;
            }

            if (!towerDataDictionary.ContainsKey(tower.UnitName))
            {
                towerDataDictionary.Add(tower.UnitName, tower);
                Debug.Log($"TowerSO �ε�: {tower.UnitName}");
            }
        }

        // ���̺� ������ �ε�
        waveDataDictionary.Clear();
        foreach (var wave in waveDataList)
        {
            if (wave == null)
            {
                Debug.LogError("WaveSO ��Ͽ� null ���� �ֽ��ϴ�.");
                continue;
            }

            if (!waveDataDictionary.ContainsKey(wave.waveCount))
            {
                waveDataDictionary.Add(wave.waveCount, wave);
                Debug.Log($"���̺� �ε� �Ϸ�: Wave {wave.waveCount} (�� ��: {wave.enemyCounts.Count})");
            }
        }

        // �� ������ �ڵ� �ε�
        enemyPrefabDictionary.Clear();
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy");

        if (loadedPrefabs.Length == 0)
        {
            Debug.LogError("EnemyPrefabs�� �ε���� �ʾҽ��ϴ�. Resources/Prefabs/Enemy ������ Ȯ���ϼ���.");
        }

        foreach (var prefab in loadedPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogError("EnemyPrefab ��Ͽ� null ���� �ֽ��ϴ�.");
                continue;
            }

            if (!enemyPrefabDictionary.ContainsKey(prefab.name))
            {
                enemyPrefabDictionary.Add(prefab.name, prefab);
                Debug.Log($"EnemyPrefab �ڵ� �ε�: {prefab.name}");
            }
        }

        Debug.Log($"���� ��ϵ� EnemyPrefab ����: {enemyPrefabDictionary.Count}");
    }

    // �� �����͸� EnemyID �������� ��������
    public EnemySO GetEnemyData(int enemyID)
    {
        if (!enemyDataDictionary.ContainsKey(enemyID))
        {
            Debug.LogError($"EnemyID {enemyID} �����͸� ã�� �� �����ϴ�.");
            return null;
        }

        return enemyDataDictionary[enemyID];
    }

    // Ÿ�� �����͸� UnitName �������� ��������
    public TowerSO GetTowerData(string unitName)
    {
        if (!towerDataDictionary.ContainsKey(unitName))
        {
            Debug.LogError($"Ÿ�� {unitName} �����͸� ã�� �� �����ϴ�.");
            return null;
        }

        return towerDataDictionary[unitName];
    }

    // ���̺� �����͸� waveCount �������� ��������
    public WaveSO GetWaveData(int waveCount)
    {
        if (!waveDataDictionary.ContainsKey(waveCount))
        {
            Debug.LogError($"���̺� {waveCount} �����͸� ã�� �� �����ϴ�. ��ϵ� ���̺� ����: {waveDataDictionary.Count}");
            foreach (var key in waveDataDictionary.Keys)
            {
                Debug.Log($"��ϵ� ���̺�: {key}");
            }
            return null;
        }

        return waveDataDictionary[waveCount];
    }
    public float GetWaveInterval(int waveIndex)
    {
        WaveSO wave = GetWaveData(waveIndex);
        return wave != null ? wave.timeBetweenWaves : 5f; // �⺻�� 5��
    }


    // �� �������� UnitName �������� ��������
    public GameObject GetEnemyPrefab(string unitName)
    {
        if (!enemyPrefabDictionary.ContainsKey(unitName))
        {
            Debug.LogError($"EnemyPrefab {unitName} �����͸� ã�� �� �����ϴ�. ��ϵ� ������ ����: {enemyPrefabDictionary.Count}");
            foreach (var key in enemyPrefabDictionary.Keys)
            {
                Debug.Log($"��ϵ� ������: {key}");
            }
            return null;
        }

        return enemyPrefabDictionary[unitName];
    }
}
