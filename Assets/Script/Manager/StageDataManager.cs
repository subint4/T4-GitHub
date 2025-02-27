using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

#if UNITY_EDITOR
using UnityEditor.U2D.Aseprite;
#endif

public class StageDataManager
{
    private Dictionary<(int, int), StageData> stageDataCache = new Dictionary<(int, int), StageData>();
    private const string StageDataPath = "JsonData/StageData";

    public void LoadStageData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(StageDataPath);
        if (jsonFile == null)
        {
            Debug.LogError($"[StageDataManager] {StageDataPath} 파일을 찾을 수 없습니다!");
            return;
        }

        StageDataContainer stageDataContainer = JsonConvert.DeserializeObject<StageDataContainer>(jsonFile.text);

        if (stageDataContainer == null || stageDataContainer.Data == null)
        {
            Debug.LogError("[StageDataManager] JSON 데이터를 불러오지 못했습니다.");
            return;
        }

        foreach (var stage in stageDataContainer.Data)
        {
            var key = (stage.StageNum, stage.SubStageNum);
            if (!stageDataCache.ContainsKey(key))
            {
                stageDataCache[key] = stage;
            }
            else
            {
                Debug.LogWarning($"[StageDataManager] 중복된 스테이지 번호 발견: {stage.StageNum}-{stage.SubStageNum}");
            }
        }

        Debug.Log("[StageDataManager] 스테이지 데이터 로드 완료.");
    }
    /// <summary>
    /// 특정 스테이지와 서브스테이지에 해당하는 StageData를 반환
    /// </summary>
    public StageData GetStageData(int stageNum, int subStageNum)
    {
        var key = (stageNum, subStageNum);
        if (stageDataCache.TryGetValue(key, out var stageData))
        {
            return stageData;
        }

        Debug.LogError($"[StageDataManager] Stage {stageNum}-{subStageNum} 데이터를 찾을 수 없습니다!");
        return null;
    }

    /// <summary>
    /// 특정 스테이지 & 서브스테이지의 WaveIDs 가져오기
    /// </summary>
    public List<int> GetWaveIDsForStage(int stageNum, int subStageNum)
    {
        var key = (stageNum, subStageNum);
        if (stageDataCache.TryGetValue(key, out var stageData))
        {
            return ParseWaveIDs(stageData.WaveIDs);
        }

        Debug.LogError($"[StageDataManager] Stage {stageNum}-{subStageNum}의 WaveIDs를 찾을 수 없습니다!");
        return new List<int>();
    }

    /// <summary>
    /// 특정 스테이지 & 서브스테이지의 웨이브 데이터 리스트 반환
    /// </summary>
    public List<WaveSO> GetWaveDataList(int stageNum, int subStageNum)
    {
        List<int> waveIDs = GetWaveIDsForStage(stageNum, subStageNum);
        if (waveIDs.Count == 0)
        {
            Debug.LogError($"[StageDataManager] {stageNum}-{subStageNum}의 WaveIDs 데이터가 없습니다!");
            return new List<WaveSO>();
        }

        return DataManager.Instance.WaveDataManager.GetWaveDataList(waveIDs);
    }

    /// <summary>
    /// 문자열 WaveIDs("1,2,3")를 List<int>로 변환
    /// </summary>
    private List<int> ParseWaveIDs(string waveIDs)
    {
        List<int> result = new List<int>();

        if (string.IsNullOrEmpty(waveIDs))
        {
            Debug.LogWarning("[StageDataManager] 빈 WaveIDs 문자열이 입력되었습니다.");
            return result;
        }

        string[] splitWaveIDs = waveIDs.Split(',');

        foreach (var id in splitWaveIDs)
        {
            if (int.TryParse(id.Trim(), out int parsedWaveID))
            {
                result.Add(parsedWaveID);
            }
            else
            {
                Debug.LogError($"[StageDataManager] WaveID 변환 실패: {id}");
            }
        }

        return result;
    }
}
[Serializable]
public class StageData
{
    public int StageNum;             // 스테이지 번호
    public int SubStageNum;          // 서브 스테이지 번호
    public string StageName;         // 스테이지 이름
    public int MaxWaves;             // 최대 웨이브 수
    public string WaveIDs;           // 쉼표(,)로 구분된 웨이브 ID 목록 (예: "1,2,3")
    public List<int> AvailableEnemies; // 해당 스테이지에서 등장 가능한 적 ID 목록
}

[Serializable]
public class StageDataContainer
{
    public List<StageData> Data; // JSON에서 로드할 스테이지 데이터 목록
}