using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Newtonsoft.Json;
using System;

public class StageDataManager
{
    public static StageDataManager Instance { get; private set; } = new StageDataManager();

    private Dictionary<(int, int), StageData> stageDataCache = new Dictionary<(int, int), StageData>();
    private bool isDataLoaded = false;

    public void LoadStageData()
    {
        if (isDataLoaded)
        {
            Debug.Log("[StageDataManager] 스테이지 데이터가 이미 로드되었습니다.");
            return;
        }

        string jsonContent = JsonLoader.LoadJsonFromResources("JsonData/StageData");
        if (!string.IsNullOrEmpty(jsonContent))
        {
            ProcessStageData(jsonContent);
            isDataLoaded = true;

            Debug.Log($"[StageDataManager] {stageDataCache.Count}개의 스테이지 데이터가 로드됨.");
            foreach (var key in stageDataCache.Keys)
            {
                Debug.Log($"[StageDataManager] 캐시된 데이터: Stage {key.Item1}-{key.Item2}");
            }
        }
        else
        {
            Debug.LogError("[StageDataManager] StageData.json 파일을 찾을 수 없습니다!");
        }
    }

    private void ProcessStageData(string jsonContent)
    {
        StageDataContainer stageDataContainer = JsonConvert.DeserializeObject<StageDataContainer>(jsonContent);
        if (stageDataContainer == null || stageDataContainer.Data == null)
        {
            Debug.LogError("[StageDataManager] JSON 데이터를 불러오지 못했습니다.");
            return;
        }

        foreach (var data in stageDataContainer.Data)
        {
            var key = (data.StageNum, data.SubStageNum);
            if (!stageDataCache.ContainsKey(key))
            {
                stageDataCache[key] = data;
                Debug.Log($"[StageDataManager] 등록됨: Stage {data.StageNum}-{data.SubStageNum}, WaveIDs: {data.WaveIDs}");
            }
            else
            {
                Debug.LogWarning($"[StageDataManager] 중복된 스테이지 데이터 발견: {data.StageNum}-{data.SubStageNum}");
            }
        }
    }
    public StageData GetStageData(int stageNum, int subStageNum)
    {
        var key = (stageNum, subStageNum);
        if (stageDataCache.TryGetValue(key, out var stageData))
        {
            Debug.Log($"[StageDataManager] Stage {stageNum}-{subStageNum} 데이터 찾음.");
            return stageData;
        }

        Debug.LogError($"[StageDataManager] Stage {stageNum}-{subStageNum} 데이터를 찾을 수 없습니다!");
        return null;
    }


    public List<int> GetWaveIDsForStage(int stageNum, int subStageNum)
    {
        var key = (stageNum, subStageNum);
        if (stageDataCache.TryGetValue(key, out var stageData))
        {
            Debug.Log($"[StageDataManager] Stage {stageNum}-{subStageNum}의 WaveIDs 찾음: {stageData.WaveIDs}");
            return ParseWaveIDs(stageData.WaveIDs);
        }

        Debug.LogError($"[StageDataManager] Stage {stageNum}-{subStageNum}의 WaveIDs를 찾을 수 없습니다!");
        return new List<int>();
    }

    private List<int> ParseWaveIDs(string waveIDs)
    {
        List<int> result = new List<int>();
        if (string.IsNullOrEmpty(waveIDs))
        {
            Debug.LogWarning("[StageDataManager] WaveIDs가 비어있음!");
            return result;
        }

        string[] split = waveIDs.Split(',');
        foreach (string id in split)
        {
            if (int.TryParse(id.Trim(), out int parsedID))
            {
                result.Add(parsedID);
            }
            else
            {
                Debug.LogError($"[StageDataManager] WaveID 파싱 실패: {id}");
            }
        }
        return result;
    }
}

[Serializable]
public class StageDataContainer
{
    public List<StageData> Data;
}

[Serializable]
public class StageData
{
    public int StageNum;
    public int SubStageNum;
    public string StageName;
    public int MaxWaves;
    public string WaveIDs;
    public List<int> AvailableEnemies;
}
