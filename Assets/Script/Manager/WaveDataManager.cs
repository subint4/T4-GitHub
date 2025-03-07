using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UIElements;

public class WaveDataManager
{
    private List<WaveData> allWaveDataList = new List<WaveData>(); // 모든 웨이브 데이터 저장
    private List<WaveData> currentWaveDataList = new List<WaveData>(); // 현재 웨이브 데이터 리스트
    private List<WaveData> waveDataList = new List<WaveData>(); // 웨이브 데이터를 저장하는 리스트



    public void LoadWaveDataFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JsonData/WaveData");

        if (jsonFile == null)
        {
            Debug.LogError("[WaveDataManager] JSON 파일을 찾을 수 없음: Resources/JsonData/WaveData.json");
            return;
        }

        WaveDataContainer waveDataContainer = JsonConvert.DeserializeObject<WaveDataContainer>(jsonFile.text);

        if (waveDataContainer == null || waveDataContainer.Data == null)
        {
            Debug.LogError("[WaveDataManager] JSON 데이터를 불러오는 중 오류 발생!");
            return;
        }

        waveDataList.Clear();
        waveDataList.AddRange(waveDataContainer.Data);

        Debug.Log($"[WaveDataManager] 총 {waveDataList.Count}개의 웨이브 데이터 로드 완료.");
    }

    public List<WaveData> GetAllWaveData()
    {
        return allWaveDataList;
    }
        
    public List<WaveData>GetWaveData(int waveIndex)
    {
        return allWaveDataList.FindAll(wave=>wave.wave==waveIndex+1);
    }

    public List<WaveData> GetWaveDataList(List<int> waveIDs)
    {
        List<WaveData> selectedWaves = new List<WaveData>();

        foreach (var wave in waveDataList)
        {
            if (waveIDs.Contains(wave.ID))
            {
                selectedWaves.Add(wave);
            }
        }

        Debug.Log($"[WaveDataManager] 요청된 웨이브 데이터 개수: {selectedWaves.Count}");
        return selectedWaves;
    }
}


[System.Serializable]
public class WaveData
{
    public int ID;
    public int wave;
    public int enemyID; // 적 ID 리스트
    public int count; // 적 개수 리스트
    public float SpawnDelay; // 스폰 딜레이 리스트
    public int SpawnGroup;
    public float interval;
    public int SpawnLaneID;
}

[System.Serializable]
public class WaveDataContainer
{
    public List<WaveData> Data;
}
