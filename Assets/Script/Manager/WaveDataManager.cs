using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UIElements;

public class WaveDataManager
{
    private List<WaveData> allWaveDataList = new List<WaveData>(); // 모든 웨이브 데이터 저장


    public void LoadWaveDataFromJSON()
    {
        string filePath = Path.Combine(Application.dataPath, "Resources/JsonData/WaveData.json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"[WaveDataManager] JSON 파일을 찾을 수 없음: {filePath}");
            return;
        }

        string jsonContent = File.ReadAllText(filePath);
        WaveDataContainer waveDataContainer = JsonConvert.DeserializeObject<WaveDataContainer>(jsonContent);

        if (waveDataContainer == null || waveDataContainer.Data == null)
        {
            Debug.LogError("[WaveDataManager] JSON 데이터를 불러오는 중 오류 발생!");
            return;
        }

        allWaveDataList = waveDataContainer.Data; // JSON에서 전체 데이터 로드
        Debug.Log($"[WaveDataManager] 총 {allWaveDataList.Count}개의 웨이브 데이터 로드 완료.");
    }

    public List<WaveData> GetAllWaveData()
    {
        return allWaveDataList;
    }
        
    public List<WaveData>GetWaveData(int waveIndex)
    {
        return allWaveDataList.FindAll(wave=>wave.wave==waveIndex+1);
    }
    public List<WaveData> GetWaveDataList(List<int>waveIDs)
    {
        if (waveIDs == null || waveIDs.Count == 0)
        {
            Debug.LogError("[WaveDataManager] 요청된 웨이브 ID 리스트가 비어 있습니다!");
            return new List<WaveData>();
        }

        List<WaveData> waveDataList = allWaveDataList.FindAll(wave => waveIDs.Contains(wave.ID));

        Debug.Log($"[WaveDataManager] 총 {waveDataList.Count}개의 웨이브 데이터 반환됨.");
        return waveDataList;
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
