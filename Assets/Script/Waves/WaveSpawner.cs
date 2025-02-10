using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Wave_Data
{
    public int key;
    public string EnemyType;
    public int SpawnCount;
    public float SpawnRate;
    public string EnemyPrefab;
}
[System.Serializable]
public class WaveConfig
{
    public List<Wave_Data> WaveStages;
}
public class WaveSpawner : MonoBehaviour
{
    [SerializeField]private List<Wave_Data> waves = new List<Wave_Data>();
    [SerializeField]private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnRows;
    //게이지 매니저
    [SerializeField] private GaugeManager gaugeManager;

    private bool isSpawning = false;
    private int currentWaveIndex = 0;
    //남은 적 목록
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        LoadWaveData();
    }

    private void Start()
    {
        Debug.Log("Start() 실행됨!");

        if (waves.Count == 0)
        {
            Debug.LogError("웨이브 데이터가 비어 있음! JSON 파일을 확인하세요.");
        }

        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("enemyPrefabs 배열이 비어 있음! Unity Inspector에서 프리팹 추가하세요.");
        }
        if (spawnRows.Length == 0)
        {
            Debug.LogError("spawnRows 배열이 비어 있음! Unity Inspector에서 스폰 위치 추가하세요.");
        }

        if (!isSpawning)StartCoroutine(SpawnWave());
    }
    private IEnumerator SpawnWave()
    {
        if(isSpawning) yield break;
        isSpawning = true;
        Debug.Log("SpawnWave() 실행됨!");

        Wave_Data wave = waves[currentWaveIndex];
        //웨이브 시작시 게이지 초기화
        gaugeManager.InitializeGauge(wave.SpawnCount);

        for (int i = 0; i < wave.SpawnCount; i++)
        {
            GameObject enemy = SpawnEnemy(wave);
            if (enemy != null)
            {
                //살아 있는 적 리스트에 추가
                activeEnemies.Add(enemy);
            }
            yield return new WaitForSeconds(wave.SpawnRate); // 적 스폰 간격 유지
        }
        Debug.Log($"웨이브 {currentWaveIndex + 1} 종료 대기...");
        //모든 몬스터 죽을때 까지 대기
        yield return new WaitUntil(() => activeEnemies.Count == 0);

        Debug.Log("웨이브 종료!");
        isSpawning = false; // 웨이브가 끝났으므로 다음 웨이브 실행 가능
        currentWaveIndex++;

        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWave()); // 다음 웨이브 실행
        }
        else
        {
            Debug.Log("모든 웨이브가 완료되었습니다!");
        }
    }
    private void LoadWaveData()
    {
        string filePath = Path.Combine(Application.dataPath, "wave_data.json");

        if(!File.Exists(filePath))
        {
            Debug.LogError($"오류 : {filePath}파일을 찾을수 없습니다.");
            return;
        }
        string jsonContent = File.ReadAllText(filePath);
        WaveConfig config = JsonConvert.DeserializeObject<WaveConfig>(jsonContent);

        if(config != null && config.WaveStages.Count > 0)
        {
            waves = config.WaveStages;
            Debug.Log($"웨이브 데이터 {waves.Count}개 로드 완료!");

        }
        else
        {
            Debug.LogError("Json값의 웨이브데이터가 비어있습니다.");
        }
    }
    private GameObject SpawnEnemy(Wave_Data wave)
    {
        if (enemyPrefabs.Length == 0 || spawnRows.Length == 0)
        {
            Debug.LogError("스폰할 적 프리팹 또는 위치가 없습니다.");
            return null;
        }
        int randomIndex = Random.Range(0, spawnRows.Length);
        Transform spawnPoint = spawnRows[randomIndex];

        Debug.Log($"스폰 위치 선택됨: {spawnPoint.position} (index: {randomIndex}");

        GameObject enemyPrefab = GetEnemyPrefabByType(wave.EnemyPrefab);
        if (enemyPrefab == null) return null;

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().OnEnemyDeath += UpdateEnemyCount;
        return enemy;
    }

    private GameObject GetEnemyPrefabByType(string enemyPrefabName)
    {
        foreach(GameObject enemyPrefab in enemyPrefabs)
        {
            if(enemyPrefab.name == enemyPrefabName)
            {
                return enemyPrefab;
            }
        }
        Debug.LogError($"적 프리팹을 찾을 수 없습니다: {enemyPrefabName}");

        return null;
    }
    
    private void UpdateEnemyCount(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        gaugeManager.UpdateGage();
    }
}
