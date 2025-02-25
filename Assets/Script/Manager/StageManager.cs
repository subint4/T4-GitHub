using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int currentStageNum { get; private set; }
    public int currentSubStageNum { get; private set; }
    private StageData currentStageData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetStageData(StageData stageData)
    {
        if (stageData == null)
        {
            Debug.LogError("[StageManager] 제공된 StageData가 NULL입니다!");
            return;
        }

        currentStageData = stageData;
        currentStageNum = stageData.StageNum;
        currentSubStageNum = stageData.SubStageNum;
        Debug.Log($"[StageManager] 새로운 스테이지 데이터 적용됨: {stageData.StageName}");
    }

    public int GetCurrentSubStageNum()
    {
        return currentSubStageNum;
    }

    public int GetMaxWaves()
    {
        return currentStageData?.MaxWaves ?? 0;
    }

    public List<int> GetAvailableEnemies()
    {
        return currentStageData?.AvailableEnemies ?? new List<int>();
    }
}
