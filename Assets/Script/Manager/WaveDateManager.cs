using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDateManager
{
    private Dictionary<int, WaveSO> waveDataDictionary = new Dictionary<int, WaveSO>();
    public void LoadWaveData()
    {
        WaveSO[] waveDataList = Resources.LoadAll<WaveSO>("WaveSO");
        foreach (var wave in waveDataList)
        {
            if (wave != null)
            {
                waveDataDictionary[wave.ID] = wave;
            }
        }
    }

    public WaveSO GetWaveData(int id)
    {
        return waveDataDictionary.TryGetValue(id, out var data) ? data : null;
    }

    public int GetMonsterCount(int stageNum)
    {

        return 0;
    }

}
