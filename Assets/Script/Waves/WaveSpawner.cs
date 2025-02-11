using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnRows;
    private int currentWaveIndex = 1;
    private bool isSpawning = false;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        if (isSpawning) yield break;
        isSpawning = true;

        WaveSO currentWave = DataManager.Instance.GetWaveData(currentWaveIndex);
        if (currentWave == null)
        {
            Debug.Log("모든 웨이브 완료!");
            yield break;
        }

        Debug.Log($"웨이브 {currentWave.waveCount} 시작! 적 수: {currentWave.GetTotalEnemies()}");

        if (currentWave.enemyCounts.Count == 0)
        {
            Debug.LogError($"Wave {currentWave.waveCount}의 enemyCounts가 비어 있습니다.");
        }

        foreach (var enemyEntry in currentWave.enemyCounts)
        {
            Debug.Log($"적 ID: {enemyEntry.Key}, 스폰 개수: {enemyEntry.Value}");

            int enemyID = enemyEntry.Key;
            int spawnCount = enemyEntry.Value;

            for (int i = 0; i < spawnCount; i++)
            {
                GameObject enemy = SpawnEnemy(enemyID);
                if (enemy != null)
                {
                    activeEnemies.Add(enemy);
                }
                yield return new WaitForSeconds(1.0f);
            }
        }

        yield return new WaitUntil(() => activeEnemies.Count == 0);
        isSpawning = false;

        // **웨이브 간격 반영**
        float waveInterval = DataManager.Instance.GetWaveInterval(currentWaveIndex);
        Debug.Log($"웨이브 {currentWaveIndex} 완료! 다음 웨이브까지 {waveInterval}초 대기...");
        yield return new WaitForSeconds(waveInterval);

        // 다음 웨이브 시작
        if (DataManager.Instance.GetWaveData(currentWaveIndex + 1) != null)
        {
            currentWaveIndex++;
            StartCoroutine(SpawnWave());
        }
        else
        {
            Debug.Log("마지막 웨이브 완료! 더 이상 웨이브 없음.");
        }
    }

    private GameObject SpawnEnemy(int enemyID)
    {
        if (spawnRows.Length == 0)
        {
            Debug.LogError("스폰 위치가 없습니다.");
            return null;
        }

        int randomIndex = Random.Range(0, spawnRows.Length);
        Transform spawnPoint = spawnRows[randomIndex];

        Debug.Log($"적 스폰 시도: EnemyID {enemyID}");

        EnemySO enemyData = DataManager.Instance.GetEnemyData(enemyID);
        if (enemyData == null)
        {
            Debug.LogError($"EnemyID {enemyID} 데이터를 찾을 수 없습니다. DataManager를 확인하세요.");
            return null;
        }

        GameObject enemyPrefab = DataManager.Instance.GetEnemyPrefab(enemyData.UnitName);
        if (enemyPrefab == null)
        {
            Debug.LogError($"EnemyID {enemyID}에 대한 프리팹이 존재하지 않습니다.");
            return null;
        }

        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // **enemyStats를 올바르게 설정**
        Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.enemyStats = enemyData;
            Debug.Log($"Enemy {enemyData.UnitName}가 {spawnPoint.position}에 스폰됨! EnemyID: {enemyData.EnemyID}");
        }
        else
        {
            Debug.LogError($"Enemy 프리팹에 Enemy 스크립트가 없습니다! {enemyPrefab.name}");
        }

        return enemyInstance;
    }

}
