using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int currentStageNum { get; private set; } = 1;
    public int currentSubStageNum = 1;
    private StageData currentStageData;
    public event Action<int, int> OnStageChanged;

    [SerializeField] private TMP_Text stageText; // 스테이지 텍스트 표시 UI

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
        LoadStageFromPrefs();
        StartCoroutine(WaitForUIAndSetStage());

        // **이벤트 등록 → 스테이지 변경 시 UI 자동 업데이트**
        OnStageChanged += UpdateStageUI;
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
        yield return new WaitUntil(() => DataManager.Instance.StageDataManager != null);

        DataManager.Instance.StageDataManager.LoadStageData();

        StageData stageData = DataManager.Instance.StageDataManager.GetStageData(currentStageNum, currentSubStageNum);
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

        // **이벤트 호출 → 스테이지가 변경되었음을 알림**
        OnStageChanged?.Invoke(currentStageNum, currentSubStageNum);
    }

    /// <summary>
    /// **스테이지 UI 업데이트 (TextMeshPro)**
    /// </summary>
    private void UpdateStageUI(int stage = -1, int subStage = -1)
    {
        if (stageText != null)
        {
            stageText.text = $"{currentStageNum}-{currentSubStageNum}";
            Debug.Log($"[StageManager] 스테이지 UI 업데이트됨: {stageText.text}");
        }
        else
        {
            Debug.LogWarning("[StageManager] Stage UI 텍스트가 할당되지 않았습니다.");
        }
    }

    /// <summary>
    /// **다음 서브 스테이지로 이동**
    /// </summary>
    public bool MoveToNextSubStage()
    {
        if (currentStageData == null)
        {
            Debug.LogError("[StageManager] 현재 스테이지 데이터가 설정되지 않았습니다!");
            return false;
        }

        // 현재 스테이지에서 존재하는 최고 SubStageNum을 가져오기
        int maxSubStageNum = DataManager.Instance.StageDataManager.GetMaxSubStageNum(currentStageNum);

        if (currentSubStageNum < maxSubStageNum)
        {
            currentSubStageNum++;
            Debug.Log($"[StageManager] 서브스테이지 변경: {currentStageNum}-{currentSubStageNum}");
            SaveStageToPrefs();
            StartCoroutine(LoadStageDataWithRetry());

            // **이벤트 호출 → UI 자동 업데이트**
            OnStageChanged?.Invoke(currentStageNum, currentSubStageNum);
            return true; // 서브스테이지 변경 성공
        }
        else
        {
            // 서브스테이지가 마지막이면 다음 메인 스테이지로 변경
            currentStageNum++;
            currentSubStageNum = 1;

            Debug.Log($"[StageManager] 메인 스테이지 변경: {currentStageNum}-{currentSubStageNum}");
            SaveStageToPrefs();
            StartCoroutine(LoadStageDataWithRetry());

            // **이벤트 호출 → UI 자동 업데이트**
            OnStageChanged?.Invoke(currentStageNum, currentSubStageNum);
            return false; // 새로운 메인 스테이지 시작
        }
    }

    public int GetCurrentSubStageNum()
    {
        return currentSubStageNum;
    }
}
