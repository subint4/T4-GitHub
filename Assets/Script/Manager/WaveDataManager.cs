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
        if (isDataLoaded)
        {
            Debug.Log("[WaveDataManager] 웨이브 데이터가 이미 로드되었습니다.");
            return;
        }

        string jsonContent = JsonLoader.LoadJsonFromResources("JsonData/WaveData");
        if (!string.IsNullOrEmpty(jsonContent))
        {
            ProcessWaveData(jsonContent);
            isDataLoaded = true;
        }
        else
        {
            Debug.LogError("[WaveDataManager] WaveData.json 파일을 찾을 수 없습니다!");
        }

        Debug.Log($"[WaveDataManager] 로드된 웨이브 데이터 키값: {string.Join(", ", waveDataCache.Keys)}");
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
            int key = wave.ID; // 또는 wave.WaveID로 변경해야 할 수도 있음
            if (!waveDataCache.ContainsKey(key))
            {
                waveDataCache[key] = wave;
            }
            else
            {
                Debug.LogWarning($"[WaveDataManager] 중복된 웨이브 ID 발견: {key}");
            }
        }

        Debug.Log($"[WaveDataManager] {waveDataCache.Count}개의 웨이브 데이터 로드 완료.");
    }


    public List<WaveSO> GetWaveDataList(List<int> waveIDs)
    {
        List<WaveSO> waveDataList = new List<WaveSO>();

        Debug.Log($"[WaveDataManager] 현재 캐시된 웨이브 데이터 키값: {string.Join(", ", waveDataCache.Keys)}");
        Debug.Log($"[WaveDataManager] 요청된 Wave IDs: {string.Join(", ", waveIDs)}");

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
