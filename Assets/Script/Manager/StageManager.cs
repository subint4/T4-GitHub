using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int currentStageNum { get; private set; } = 1;
    private int currentSubStageNum = 1;
    private StageData currentStageData;

    [SerializeField] private TMP_Text stageText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadStageFromPrefs(); // PlayerPrefs에서 스테이지 불러오기
        StartCoroutine(WaitForUIAndSetStage());
    }

    private IEnumerator WaitForUIAndSetStage()
    {
        yield return new WaitUntil(() => stageText != null && !string.IsNullOrEmpty(stageText.text));

        ReadStageFromTMP();
        StartCoroutine(LoadStageDataWithRetry());
    }

    private void LoadStageFromPrefs()
    {
        currentStageNum = PlayerPrefs.GetInt("CurrentStage", 1);
        currentSubStageNum = PlayerPrefs.GetInt("CurrentSubStage", 1);
    }

    private void SaveStageToPrefs()
    {
        PlayerPrefs.SetInt("CurrentStage", currentStageNum);
        PlayerPrefs.SetInt("CurrentSubStage", currentSubStageNum);
        PlayerPrefs.Save();
    }

    private void ReadStageFromTMP()
    {
        if (stageText == null)
        {
            Debug.LogError("[StageManager] TMP(TextMeshPro)가 설정되지 않았습니다!");
            return;
        }

        string stageString = stageText.text.Trim();

        if (!string.IsNullOrEmpty(stageString))
        {
            string[] split = stageString.Split('-');

            if (split.Length == 2 && int.TryParse(split[0], out int stage) && int.TryParse(split[1], out int subStage))
            {
                currentStageNum = stage;
                currentSubStageNum = subStage;
                SaveStageToPrefs();
                Debug.Log($"[StageManager] 현재 스테이지 설정됨: {currentStageNum}-{currentSubStageNum}");
            }
            else
            {
                Debug.LogError($"[StageManager] TMP에서 올바른 형식의 스테이지 정보를 읽을 수 없음: {stageString}");
            }
        }
    }

    private IEnumerator LoadStageDataWithRetry()
    {
        yield return new WaitUntil(() => StageDataManager.Instance != null);

        StageDataManager.Instance.LoadStageData();

        StageData stageData = StageDataManager.Instance.GetStageData(currentStageNum, currentSubStageNum);
        if (stageData != null)
        {
            SetStageData(stageData);
        }
        else
        {
            Debug.LogError($"[StageManager] Stage {currentStageNum}-{currentSubStageNum} 데이터를 찾을 수 없습니다!");
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
        SaveStageToPrefs();
        Debug.Log($"[StageManager] 새로운 스테이지 데이터 적용됨: {stageData.StageNum}-{stageData.SubStageNum}");
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
