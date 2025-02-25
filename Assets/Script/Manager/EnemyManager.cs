using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    private Dictionary<int, GameObject> enemyPrefabDictionary = new Dictionary<int, GameObject>();
    private List<Enemy> activeEnemies = new List<Enemy>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeEnemies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEnemies()
    {
        Debug.Log("[EnemyManager] 모든 적 프리팹을 불러옵니다.");

        GameObject[] enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy");
        foreach (var prefab in enemyPrefabs)
        {
            Enemy enemyComponent = prefab.GetComponent<Enemy>();
            if (enemyComponent == null)
            {
                Debug.LogError($"[EnemyManager] {prefab.name} 프리팹에서 Enemy 컴포넌트를 찾을 수 없습니다!");
                continue;
            }

            EnemySO enemySO = DataManager.Instance.EnemyDataManager.GetEnemyData(enemyComponent.EnemyID);
            if (enemySO == null)
            {
                Debug.LogError($"[EnemyManager] {prefab.name} 프리팹에 해당하는 EnemySO (ID: {enemyComponent.EnemyID})를 찾을 수 없습니다!");
                continue;
            }

            enemyPrefabDictionary[enemyComponent.EnemyID] = prefab;
            Debug.Log($"[EnemyManager] Enemy ID {enemyComponent.EnemyID} - {prefab.name} 프리팹과 자동 연결됨.");
        }
    }

    public GameObject GetEnemyPrefab(int enemyID)
    {
        if (enemyPrefabDictionary.TryGetValue(enemyID, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogError($"[EnemyManager] Enemy ID {enemyID}에 대한 프리팹을 찾을 수 없습니다!");
        return null;
    }

    public void SpawnEnemy(int enemyID, Transform spawnPoint)
    {
        GameObject enemyPrefab = GetEnemyPrefab(enemyID);
        if (enemyPrefab == null)
        {
            return;
        }

        GameObject newEnemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            newEnemy.Initialize(DataManager.Instance.EnemyDataManager.GetEnemyData(enemyID), EnemyType.Melee);
            activeEnemies.Add(newEnemy);
            Debug.Log($"[EnemyManager] 적 스폰 완료: ID {enemyID} 위치 {spawnPoint.position}");

        }
        else
        {
            Debug.LogError("[EnemyManager] 생성된 적에 Enemy 컴포넌트가 없습니다!");
        }
    }
    public void RemoveEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"[EnemyManager] 적 제거: {enemy.EnemyID}");

            // 적이 제거될 때 게이지 업데이트
            WaveManager.Instance.OnEnemyDefeated();
        }
    }
}
