using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private List<Enemy> activeEnemies = new List<Enemy>();

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

    /// <summary>
    /// 적을 스폰하는 함수
    /// </summary>
    public GameObject SpawnEnemy(int enemyID, Transform spawnPoint)
    {
        EnemySO enemyData = DataManager.GetEnemyData(enemyID);
        if (enemyData == null)
        {
            Debug.LogError($"EnemyManager: EnemySO({enemyID})를 찾을 수 없습니다!");
            return null;
        }

        GameObject enemyPrefab = DataManager.GetEnemyPrefab(enemyID);
        if (enemyPrefab == null)
        {
            Debug.LogError($"EnemyManager: EnemyID {enemyID}에 대한 프리팹을 찾을 수 없습니다!");
            return null;
        }

        GameObject newEnemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            newEnemy.Initialize(enemyData);  // 적 데이터 전달
            return newEnemyObj;  // GameObject 반환하도록 수정
        }
        else
        {
            Debug.LogError("EnemyManager: 생성된 적에 Enemy 컴포넌트가 없습니다!");
            return null;
        }
    }

    /// <summary>
    /// 적이 죽었을 때 리스트에서 제거
    /// </summary>
    public void RemoveEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }
}
