using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

public class StageDataManager
{
    private List<StageData> stageDataList = new List<StageData>(); // 전체 스테이지 데이터를 저장하는 리스트
    private Dictionary<(int, int), StageData> stageDataDictionary = new Dictionary<(int, int), StageData>(); // 스테이지 데이터를 저장하는 딕셔너리
    private bool isDataLoaded = false;

    public void LoadStageData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JsonData/StageData");

        if (jsonFile == null)
        {
            Debug.LogError("[StageDataManager] JSON 파일을 찾을 수 없음: Resources/JsonData/StageData.json");
            return;
        }

        StageDataContainer stageDataContainer = JsonConvert.DeserializeObject<StageDataContainer>(jsonFile.text);

        if (stageDataContainer == null || stageDataContainer.Data == null)
        {
            Debug.LogError("[StageDataManager] JSON 데이터를 불러오는 중 오류 발생!");
            return;
        }

        stageDataDictionary.Clear();
        foreach (var data in stageDataContainer.Data)
        {
            var key = (data.StageNum, data.SubStageNum);
            stageDataDictionary[key] = data;
        }

        Debug.Log($"[StageDataManager] 총 {stageDataDictionary.Count}개의 스테이지 데이터 로드 완료.");
    }

    /// <summary>
    /// 특정 스테이지의 서브 스테이지 목록을 가져온다.
    /// </summary>
    public List<StageData> GetStageDataListForStage(int stageNum)
    {
        List<StageData> subStageList = stageDataList.FindAll(stage => stage.StageNum == stageNum);

        if (subStageList.Count == 0)
        {
            Debug.LogWarning($"[StageDataManager] Stage {stageNum}에 대한 서브 스테이지 데이터를 찾을 수 없습니다.");
        }

        return subStageList;
    }

    /// <summary>
    /// 특정 스테이지와 서브 스테이지에 해당하는 데이터를 반환
    /// </summary>
    public StageData GetStageData(int stageNum, int subStageNum)
    {
        var key = (stageNum, subStageNum);
        if (stageDataDictionary.TryGetValue(key, out var stageData))
        {
            return stageData;
        }

        Debug.LogError($"[StageDataManager] Stage {stageNum}-{subStageNum} 데이터를 찾을 수 없습니다!");
        return null;
    }

    /// <summary>
    /// 특정 스테이지와 서브 스테이지에 해당하는 WaveIDs 리스트 반환
    /// </summary>
    public List<int> GetWaveIDsForStage(int stageNum, int subStageNum)
    {
        var key = (stageNum, subStageNum);
        if (stageDataDictionary.TryGetValue(key, out var stageData))
        {
            return ParseWaveIDs(stageData.WaveIDs);
        }

        Debug.LogError($"[StageDataManager] Stage {stageNum}-{subStageNum}의 WaveIDs를 찾을 수 없습니다!");
        return new List<int>();
    }


    /// <summary>
    /// WaveID 문자열을 정수 리스트로 변환
    /// </summary>
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
    /// <summary>
    /// 특정 스테이지에서 가장 큰 서브 스테이지 번호를 반환
    /// </summary>
    public int GetMaxSubStageNum(int stageNum)
    {
        List<StageData> subStages = GetStageDataListForStage(stageNum);

        if (subStages == null || subStages.Count == 0)
        {
            Debug.LogError($"[StageDataManager] Stage {stageNum}의 서브 스테이지 데이터를 찾을 수 없습니다!");
            return 1;
        }

        int maxSubStage = 1;
        foreach (var stage in subStages)
        {
            if (stage.SubStageNum > maxSubStage)
            {
                maxSubStage = stage.SubStageNum;
            }
        }

        return maxSubStage;
    }
}

/// <summary>
/// JSON 데이터 컨테이너
/// </summary>
[Serializable]
public class StageDataContainer
{
    public List<StageData> Data;
}

/// <summary>
/// 개별 스테이지 데이터
/// </summary>
[Serializable]
public class StageData
{
    public int StageNum;
    public int SubStageNum;
    public string StageName;
    public string WaveIDs;
    public List<int> AvailableEnemies;
}
