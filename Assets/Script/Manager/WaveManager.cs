using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class WaveManager : MonoBehaviour
{
    public TextAsset jsonFile; // JSON 파일 (유니티 에디터에서 넣어줌)
    public Transform spawnPoint; // 적 생성 위치
    private List<WaveStageData> waves;
    private int currentWave = 0;

    private void Start()
    {
        LoadWaveData();
        StartCoroutine(SpawnWaves());
    }

    private void LoadWaveData()
    {
        if (jsonFile == null)
        {
            Debug.LogError("Error: JSON 파일을 찾을 수 없습니다.");
            return;
        }

        string jsonData = jsonFile.text;
        WaveStageConfig waveConfig = JsonConvert.DeserializeObject<WaveStageConfig>(jsonData);
        waves = waveConfig.WaveStages;
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWave < waves.Count)
        {
            Debug.Log($"웨이브 {currentWave + 1} 시작!");

            List<WaveStageData> currentWaveData = waves.FindAll(w => w.key == currentWave + 1);
            foreach (WaveStageData wave in currentWaveData)
            {
                StartCoroutine(SpawnEnemies(wave));
            }

            yield return new WaitForSeconds(5f); // 웨이브 간 대기 시간
            currentWave++;
        }

        Debug.Log("모든 웨이브 완료!");
    }

    private IEnumerator SpawnEnemies(WaveStageData wave)
    {
        GameObject enemyPrefab = Resources.Load<GameObject>(wave.EnemyPrefab);
        if (enemyPrefab == null)
        {
            Debug.LogError($"Error: {wave.EnemyPrefab} 프리팹을 찾을 수 없습니다.");
            yield break;
        }

        for (int i = 0; i < wave.SpawnCount; i++)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log($"{wave.EnemyType} 생성! ({i + 1}/{wave.SpawnCount})");
            yield return new WaitForSeconds(wave.SpawnRate);
        }
    }
}
