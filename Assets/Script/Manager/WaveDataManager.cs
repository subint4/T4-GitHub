using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class WaveDataManager
{
    private List<WaveData> waveDataList = new List<WaveData>(); // 모든 웨이브 데이터 저장

    /// <summary>
    /// JSON에서 웨이브 데이터 로드
    /// </summary>
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

        // **웨이브 데이터 내용 출력**
        foreach (var wave in waveDataList)
        {
            Debug.Log($"[WaveData] ID: {wave.ID}, Wave: {wave.wave}, EnemyID: {wave.enemyID}, " +
                      $"Count: {wave.count}, SpawnDelay: {wave.SpawnDelay}, SpawnGroup: {wave.SpawnGroup}, " +
                      $"Interval: {wave.interval}, SpawnLaneID: {wave.SpawnLaneID}, " +
                      $"StageNum: {wave.stageNum}, SubStageNum: {wave.subStageNum}");
        }
    }

    /// <summary>
    /// 모든 웨이브 데이터 반환
    /// </summary>
    public List<WaveData> GetAllWaveData()
    {
        return waveDataList;
    }

    /// <summary>
    /// 특정 웨이브 인덱스에 해당하는 웨이브 데이터를 반환
    /// </summary>
    public List<WaveData> GetWaveData(int waveIndex)
    {
        return waveDataList.Where(wave => wave.wave == waveIndex + 1).ToList();
    }

    /// <summary>
    /// 특정 웨이브 ID 목록에 해당하는 웨이브 데이터 반환
    /// </summary>
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

        Debug.Log($"[WaveDataManager] 요청된 웨이브 데이터 개수: {selectedWaves.Count}, waveIDs: {string.Join(", ", waveIDs)}");
        return selectedWaves;
    }


    /// <summary>
    /// 특정 스테이지와 서브스테이지에 해당하는 웨이브 데이터 반환
    /// </summary>
    public List<WaveData> GetWaveDataListForStage(int stageNum, int subStageNum)
    {
        if (waveDataList == null || waveDataList.Count == 0)
        {
            Debug.LogError("[WaveDataManager] 웨이브 데이터가 로드되지 않았습니다!");
            return new List<WaveData>(); // 빈 리스트 반환
        }

        List<WaveData> filteredWaves = waveDataList
            .Where(wave => wave.stageNum == stageNum && wave.subStageNum == subStageNum)
            .ToList();

        if (filteredWaves.Count == 0)
        {
            Debug.LogError($"[WaveDataManager] {stageNum}-{subStageNum}에 대한 웨이브 데이터를 찾을 수 없습니다!");
        }
        else
        {
            Debug.Log($"[WaveDataManager] {stageNum}-{subStageNum} 웨이브 데이터 {filteredWaves.Count}개 로드 완료!");
        }

        return filteredWaves;
    }
}

[System.Serializable]
public class WaveData
{
    public int ID;
    public int wave;
    public int enemyID; // 적 ID
    public int count; // 적 개수
    public float SpawnDelay; // 스폰 딜레이
    public int SpawnGroup;
    public float interval;
    public int SpawnLaneID;
    public int stageNum; 
    public int subStageNum;
}

[System.Serializable]
public class WaveDataContainer
{
    public List<WaveData> Data;
}
