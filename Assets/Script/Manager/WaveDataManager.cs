using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveDataManager
{
    private Dictionary<int, WaveSO> waveDataDictionary = new Dictionary<int, WaveSO>();

    public void LoadWaveData()
    {
        // 기존 데이터 초기화 (중복 방지)
        waveDataDictionary.Clear();

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
                    Debug.LogError($"[WaveDataManager] 중복된 웨이브 ID 발견: {wave.ID}, 기존 데이터 유지");
                }
            }
        }

        Debug.Log($"[WaveDataManager] {waveDataDictionary.Count}개의 웨이브 데이터 로드 완료.");
    }

    /// <summary>
    /// 특정 웨이브 ID에 해당하는 웨이브 데이터를 반환
    /// </summary>
    public WaveSO GetWaveData(int waveID)
    {
        return waveDataDictionary.TryGetValue(waveID, out var data) ? data : null;
    }

    /// <summary>
    /// 특정 스테이지에 필요한 여러 개의 웨이브 데이터를 순서대로 가져옴
    /// </summary>
    public List<WaveSO> GetWaveDataList(List<int> waveIDs)
    {
        List<WaveSO> waveList = new List<WaveSO>();

        if (waveIDs == null || waveIDs.Count == 0)
        {
            Debug.LogWarning("[WaveDataManager] 빈 웨이브 ID 리스트가 입력되었습니다.");
            return waveList;
        }

        // 웨이브 ID 정렬 (순서 보장)
        waveIDs.Sort();

        foreach (var waveID in waveIDs)
        {
            if (waveDataDictionary.TryGetValue(waveID, out var data))
            {
                waveList.Add(data);
            }
            else
            {
                Debug.LogError($"[WaveDataManager] 웨이브 {waveID} 데이터를 찾을 수 없습니다.");
            }
        }

        Debug.Log($"[WaveDataManager] {waveList.Count}개의 웨이브 데이터 반환됨.");
        return waveList;
    }
}
