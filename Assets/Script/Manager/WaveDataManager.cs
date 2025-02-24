using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveDataManager
{
    private Dictionary<int, WaveSO> waveDataDictionary = new Dictionary<int, WaveSO>();

    public void LoadWaveData()
    {
        WaveSO[] waveDataList = Resources.LoadAll<WaveSO>("WaveSO");

        foreach (var wave in waveDataList)
        {
            if (wave != null)
            {
                if (!waveDataDictionary.ContainsKey(wave.ID))
                {
                    waveDataDictionary[wave.ID] = wave;
                }
                else
                {
                    Debug.LogWarning($"[WaveDataManager] 중복된 웨이브 ID 발견: {wave.ID}");
                }
            }
        }

        Debug.Log("[WaveDataManager] 모든 웨이브 데이터 로드 완료.");
    }

    /// <summary>
    /// 특정 웨이브 ID에 해당하는 웨이브 데이터를 반환
    /// </summary>
    public WaveSO GetWaveData(int waveID)
    {
        return waveDataDictionary.TryGetValue(waveID, out var data) ? data : null;
    }

    /// <summary>
    /// 특정 웨이브 ID 목록을 기반으로 웨이브 데이터를 가져옴
    /// </summary>
    public List<WaveSO> GetWaveDataList(List<int> waveIDs)
    {
        List<WaveSO> waveList = new List<WaveSO>();

        foreach (var waveID in waveIDs)
        {
            if (waveDataDictionary.TryGetValue(waveID, out var data))
            {
                waveList.Add(data);
            }
            else
            {
                Debug.LogWarning($"[WaveDataManager] 웨이브 {waveID} 데이터를 찾을 수 없습니다.");
            }
        }

        return waveList;
    }
}
