using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;


public class WaveDataManager
{
    public static WaveDataManager Instance { get; private set; } = new WaveDataManager();

    private Dictionary<int, WaveSO> waveDataCache = new Dictionary<int, WaveSO>(); // SO 기반 캐싱
    private bool isDataLoaded = false;

    /// <summary>
    /// 웨이브 데이터를 ScriptableObject(SO)에서 로드
    /// </summary>
    public void LoadWaveData()
    {
        string jsonContent = JsonLoader.LoadJsonFromResources("JsonData/WaveData");
        if (!string.IsNullOrEmpty(jsonContent))
        {
            ProcessWaveData(jsonContent);
        }
        else
        {
            Debug.LogError("[WaveDataManager] WaveData.json 파일을 찾을 수 없습니다!");
        }
    }


    private void ProcessWaveData(string jsonContent)
    {
        WaveDataContainer waveDataContainer = JsonConvert.DeserializeObject<WaveDataContainer>(jsonContent);
        if (waveDataContainer == null || waveDataContainer.Data == null)
        {
            Debug.LogError("[WaveDataManager] JSON 데이터를 불러오지 못했습니다.");
            return;
        }

        foreach (var wave in waveDataContainer.Data)
        {
            if (!waveDataCache.ContainsKey(wave.ID))
            {
                waveDataCache[wave.ID] = wave;
                Debug.Log($"[WaveDataManager] 웨이브 ID {wave.ID} 캐싱 완료.");
            }
            else
            {
                Debug.LogWarning($"[WaveDataManager] 중복된 웨이브 ID 발견: {wave.ID}");
            }
        }

        Debug.Log($"[WaveDataManager] {waveDataCache.Count}개의 웨이브 데이터 로드 완료.");
    }

    public List<WaveSO> GetWaveDataList(List<int> waveIDs)
    {
        List<WaveSO> waveDataList = new List<WaveSO>();

        Debug.Log($"[WaveDataManager] 현재 캐시된 웨이브 데이터: {string.Join(", ", waveDataCache.Keys)}");

        foreach (var waveID in waveIDs)
        {
            if (waveDataCache.TryGetValue(waveID, out var waveData))
            {
                Debug.Log($"[WaveDataManager] 웨이브 ID {waveID} 로드 성공!");
                waveDataList.Add(waveData);
            }
            else
            {
                Debug.LogError($"[WaveDataManager] 웨이브 ID {waveID}를 찾을 수 없습니다!");
            }
        }

        return waveDataList;
    }

}


[Serializable]
public class WaveDataContainer
{
    public List<WaveSO> Data;
}

[Serializable]
public class SpawnData
{
    public int enemyID;
    public int count;
    public float SpawnDelay;
}
