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
    [SerializeField] private TMP_Text stageText;
    private int stageNum;
    private int subStageNum;

    private void Start()
    {
        DetectButtons(); // 버튼 감지 및 리스너 등록
        DetectStageText(); // TMP 텍스트 감지
    }

    /// <summary>
    /// 버튼 감지 및 리스너 등록
    /// </summary>
    private void DetectButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // 활성/비활성 버튼 모두 찾음

        foreach (var btn in buttons)
        {
            btn.onClick.RemoveAllListeners(); // 기존 리스너 제거

            if (btn.gameObject.name == "StageBackButton")
            {
                btn.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
                Debug.Log("[SceneLoader] 뒤로가기 버튼 감지 완료!");
            }
            else
            {
                btn.onClick.AddListener(() => LoadTargetScene(btn));
                Debug.Log($"[SceneLoader] 버튼 감지 완료: {btn.gameObject.name}");
            }
        }
    }

    /// <summary>
    /// TMP 텍스트 감지하여 Stage 데이터 추출
    /// </summary>
    private void DetectStageText()
    {
        stageText = GetComponentInChildren<TMP_Text>() ?? transform.parent?.GetComponentInChildren<TMP_Text>();

        if (stageText != null)
        {
            Debug.Log($"[SceneLoader] 감지된 스테이지 텍스트: {stageText.text}");
            ExtractStageData(stageText.text);
        }
        else
        {
            Debug.Log("[SceneLoader] 스테이지 텍스트 감지 실패! 버튼 이름을 기반으로 기능 실행");
        }
    }

    /// <summary>
    /// 버튼 클릭 시 해당 씬 로드
    /// </summary>
    private void LoadTargetScene(Button clickedButton)
    {
        string buttonName = clickedButton.gameObject.name;
        Debug.Log($"[SceneLoader] {buttonName} 버튼 클릭됨!");

        switch (buttonName)
        {
            case "StageButton":
                LoadStageMenu();
                break;
            case "ShopButton":
                LoadShop();
                break;
            case "TutorialButton":
                LoadTutorial();
                break;
            default:
                if (buttonName.StartsWith("Stage"))
                {
                    LoadStage(clickedButton);
                }
                else
                {
                    Debug.LogWarning($"[SceneLoader] {buttonName} 버튼에 대한 처리 없음.");
                }
                break;
        }
    }

    /// <summary>
    /// Stage X-Y 형식의 TMP 텍스트에서 StageNum과 SubStageNum을 추출
    /// </summary>
    private void ExtractStageData(string stageText)
    {
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

    /// <summary>
    /// 스테이지 데이터 JSON에서 해당하는 StageNum, SubStageNum 설정 적용
    /// </summary>
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

    /// <summary>
    /// Stage 버튼을 클릭하면 해당 Stage를 로드
    /// </summary>
    private void LoadStage(Button stageButton)
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

    /// <summary>
    /// JSON 없이도 작동하는 스테이지 메뉴 이동
    /// </summary>
    public void LoadStageMenu()
    {
        Debug.Log("[SceneLoader] 스테이지 메뉴 로드");
        SceneManager.LoadScene("StageP_1");
    }

    /// <summary>
    /// JSON 없이도 작동하는 상점 이동 (Shop 씬이 추가될 경우 사용)
    /// </summary>
    public void LoadShop()
    {
        Debug.Log("[SceneLoader] 상점 씬 로드");
        SceneManager.LoadScene("Shop");
    }

    /// <summary>
    /// JSON 없이도 작동하는 튜토리얼 씬 이동
    /// </summary>
    public void LoadTutorial()
    {
        Debug.Log("[SceneLoader] 튜토리얼 씬 로드");
        SceneManager.LoadScene("Tutorial");
    }
}
