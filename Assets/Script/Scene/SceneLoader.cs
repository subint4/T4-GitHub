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
    public static SceneLoader Instance; // 싱글턴 패턴 적용

    [SerializeField] private TMP_Text stageText;
    private Button button;
    private int stageNum;
    private int subStageNum;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되도 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        DetectButtons();
        DetectStageText();
    }

    private void DetectButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // 비활성화된 버튼도 감지

        foreach (var btn in buttons)
        {
            btn.gameObject.SetActive(true); // 버튼 강제 활성화
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => LoadTargetScene(btn));
            Debug.Log($"[SceneLoader] 버튼 감지 완료: {btn.gameObject.name}");
        }
    }

    private void DetectStageText()
    {
        stageText = FindObjectOfType<TMP_Text>();

        if (stageText != null)
        {
            Debug.Log($"[SceneLoader] 감지된 스테이지 텍스트: {stageText.text}");
            ExtractStageData(stageText.text);
        }
        else
        {
            Debug.Log("[SceneLoader] 스테이지 텍스트 감지 실패! 버튼 이름을 기반으로 기능을 실행합니다.");
        }
    }

    public void LoadTargetScene(Button clickedButton)
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

    private void LoadStage(Button clickedButton)
    {
        if (stageText != null)
        {
            ExtractStageData(stageText.text);
            if (stageNum > 0 && subStageNum > 0)
            {
                ApplySubStageSettings(stageNum, subStageNum);
                SceneManager.LoadScene("Stage1"); // 스테이지 씬 로드
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

    public void LoadStageMenu() => SceneManager.LoadScene("StageP_1");
    public void LoadShop() => SceneManager.LoadScene("Shop");
    public void LoadTutorial() => SceneManager.LoadScene("Tutorial");

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneLoader] 새로운 씬 로드됨: {scene.name}");
        DetectButtons(); // 새로운 씬에서 버튼 다시 찾기
        DetectStageText(); // 새로운 씬에서 TMP 다시 찾기
    }
}
