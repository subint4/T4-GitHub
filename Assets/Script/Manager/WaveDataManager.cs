using UnityEngine;
using System.Collections.Generic;

public class WaveDataManager
{
    private Dictionary<(int, int), List<WaveSO>> waveDataDictionary = new Dictionary<(int, int), List<WaveSO>>();

    public void LoadWaveData()
    {
        WaveSO[] waveDataList = Resources.LoadAll<WaveSO>("WaveSO");

        foreach (var wave in waveDataList)
        {
            if (wave != null)
            {
                var key = (wave.stagenum, wave.substagenum);

                if (!waveDataDictionary.ContainsKey(key))
                {
                    waveDataDictionary[key] = new List<WaveSO>();
                }

                waveDataDictionary[key].Add(wave);
            }
        }

        Debug.Log("[WaveDataManager] 웨이브 데이터 로드 완료.");
    }

    public List<WaveSO> GetWaveDataList(int stageNum, int subStageNum)
    {
        var key = (stageNum, subStageNum);
        return waveDataDictionary.TryGetValue(key, out var waveList) ? waveList : new List<WaveSO>();
    }

    public WaveSO GetWaveData(int stageNum, int subStageNum, int waveIndex)
    {
        var key = (stageNum, subStageNum);
        if (waveDataDictionary.TryGetValue(key, out var waveList) && waveIndex < waveList.Count)
        {
            return waveList[waveIndex];
        }

        Debug.LogWarning($"[WaveDataManager] {stageNum}-{subStageNum}의 {waveIndex}번 웨이브 데이터를 찾을 수 없습니다.");
        return null;
    }
}
