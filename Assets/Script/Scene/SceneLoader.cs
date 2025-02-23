using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text stageText; // 버튼의 TMP 텍스트를 읽어올 변수
    private Button button;
    private int stageNum;
    private int subStageNum;

    private void Start()
    {
        // 버튼 감지 및 이벤트 등록
        button = GetComponentInChildren<Button>() ?? transform.parent?.GetComponentInChildren<Button>();

        if (button != null)
        {
            button.onClick.AddListener(LoadTargetScene);
            Debug.Log("[SceneLoader] 버튼 감지 완료.");
        }
        else
        {
            Debug.LogError("[SceneLoader] 버튼을 찾을 수 없습니다!");
        }

        // TMP 텍스트 감지 후 StageData 추출
        stageText = GetComponentInChildren<TMP_Text>() ?? transform.parent?.GetComponentInChildren<TMP_Text>();

        if (stageText != null)
        {
            Debug.Log($"[SceneLoader] 감지된 스테이지 텍스트: {stageText.text}");
            ExtractStageData(stageText.text); // 여기서 stageNum, subStageNum 설정됨
        }
        else
        {
            Debug.LogError("[SceneLoader] TMP Text 감지 실패!");
        }
    }




    private void LoadTargetScene()
    {
        if (stageText != null)
        {
            ExtractStageData(stageText.text);
            if (stageNum > 0 && subStageNum > 0)
            {
                ApplySubStageSettings(stageNum, subStageNum);
                SceneManager.LoadScene("Stage1"); // Stage1만 로드
            }
            else
            {
                Debug.LogError("[SceneLoader] 올바른 Stage 번호를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogError("[SceneLoader] StageText가 없습니다!");
        }
    }

    private void ExtractStageData(string stageText)
    {
        // 정규식: "StageX-Y" 또는 "Stage X-Y" 형식을 모두 인식
        Match match = Regex.Match(stageText, @"Stage\s*(\d+)-(\d+)");

        if (match.Success)
        {
            stageNum = int.Parse(match.Groups[1].Value);
            subStageNum = int.Parse(match.Groups[2].Value);
            Debug.Log($"[SceneLoader] 감지된 Stage: {stageNum}, SubStage: {subStageNum}");
        }
        else
        {
            Debug.LogWarning($"[SceneLoader] {stageText}에서 유효한 스테이지 번호를 찾을 수 없습니다.");
            stageNum = -1;
            subStageNum = -1;
        }
    }



    private void ApplySubStageSettings(int stageNum, int subStageNum)
    {
        string jsonPath = Path.Combine(Application.dataPath, "Resources/JsonData/StageData.json");

        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"[SceneLoader] {jsonPath} 파일을 찾을 수 없습니다!");
            return;
        }

        string jsonData = File.ReadAllText(jsonPath);
        StageDataContainer stageDataContainer = JsonConvert.DeserializeObject<StageDataContainer>(jsonData);

        if (stageDataContainer == null || stageDataContainer.Data == null)
        {
            Debug.LogError("[SceneLoader] JSON 데이터를 불러오지 못했습니다.");
            return;
        }

        var matchedStage = stageDataContainer.Data.Find(s => s.StageNum == stageNum && s.SubStageNum == subStageNum);
        if (matchedStage != null)
        {
            Debug.Log($"[SceneLoader] {stageNum}-{subStageNum} 설정 적용 완료!");
            PlayerPrefs.SetInt("CurrentStage", stageNum);
            PlayerPrefs.SetInt("CurrentSubStage", subStageNum);
        }
        else
        {
            Debug.LogError($"[SceneLoader] {stageNum}-{subStageNum}에 해당하는 데이터를 찾을 수 없습니다!");
        }
    }
}