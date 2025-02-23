using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class StageDataManager
{
    private Dictionary<string, StageData> stageDataCache = new Dictionary<string, StageData>();
    private const string StageDataPath = "JsonData/StageData"; // Resources 폴더 내부 JSON 파일 경로

    public void LoadStageData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(StageDataPath);
        if (jsonFile == null)
        {
            Debug.LogError($"[StageDataManager] {StageDataPath}.json 파일을 찾을 수 없습니다!");
            return;
        }

        string jsonData = jsonFile.text;
        StageDataContainer stageDataContainer = JsonConvert.DeserializeObject<StageDataContainer>(jsonData);

        if (stageDataContainer == null || stageDataContainer.Data == null)
        {
            Debug.LogError("[StageDataManager] JSON 데이터를 불러오지 못했습니다.");
            return;
        }

        foreach (var stage in stageDataContainer.Data)
        {
            string stageKey = $"{stage.StageNum}-{stage.SubStageNum}"; // StageNum-SubStageNum 조합
            if (!stageDataCache.ContainsKey(stageKey))
            {
                stageDataCache[stageKey] = stage;
            }
            else
            {
                Debug.LogWarning($"[StageDataManager] 중복된 스테이지 발견: {stageKey}");
            }
        }

        Debug.Log("[StageDataManager] 스테이지 데이터 로드 완료.");
    }

    public StageData GetStageData(int stageNum, int subStageNum)
    {
        string stageKey = $"{stageNum}-{subStageNum}";
        if (stageDataCache.TryGetValue(stageKey, out var stageData))
        {
            return stageData;
        }

        Debug.LogError($"[StageDataManager] Stage {stageKey} 데이터를 찾을 수 없습니다!");
        return null;
    }
}

// JSON에서 사용할 데이터 클래스 정의
[System.Serializable]
public class StageDataContainer
{
    public List<StageData> Data;
}

[System.Serializable]
public class StageData
{
    public int StageNum;
    public int SubStageNum;
    public string StageName;
    public int MaxWaves;
    public List<int> AvailableEnemies;
}
