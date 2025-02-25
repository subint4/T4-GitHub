using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text stageText;
    private static SceneLoader instance;
    private int stageNum;
    private int subStageNum;
    private int currentStageIndex = 1; // 기본값: StageP_1에서 시작

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[SceneLoader] DontDestroyOnLoad 적용됨");
        }
        else
        {
            Debug.Log("[SceneLoader] 기존 SceneLoader 유지");
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        DetectButtons(); // 처음 씬의 버튼 감지
    }

    /// <summary>
    /// 씬이 변경될 때 자동으로 버튼 감지
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneLoader] 씬 로드됨: {scene.name}");
        DetectButtons();
    }

    /// <summary>
    /// 모든 버튼을 찾아 클릭 이벤트 등록
    /// </summary>
    private void DetectButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>();

        Debug.Log($"[SceneLoader] 감지된 버튼 개수: {buttons.Length}");

        foreach (var button in buttons)
        {
            if (button == null) continue; // null 체크

            button.onClick.RemoveAllListeners(); // 중복 방지
            button.onClick.AddListener(() => LoadTargetScene(button));
            Debug.Log($"[SceneLoader] 버튼 감지 완료: {button.gameObject.name}");
        }
    }

    /// <summary>
    /// 버튼 클릭 시 올바른 기능 호출
    /// </summary>
    private void LoadTargetScene(Button clickedButton)
    {
        if (clickedButton == null)
        {
            Debug.LogError("[SceneLoader] LoadTargetScene 호출 시 clickedButton이 null입니다!");
            return;
        }

        string buttonName = clickedButton.gameObject.name;
        Debug.Log($"[SceneLoader] {buttonName} 버튼 클릭됨!");

        switch (buttonName)
        {
            case "PlayButton":
                LoadStageP1();
                break;
            case "NextStageButton":
                Debug.Log("[SceneLoader] 다음 스테이지 버튼 클릭!");
                LoadNextStage();
                break;
            case "PreviousStageButton":
                Debug.Log("[SceneLoader] 이전 스테이지 버튼 클릭!");
                LoadPreviousStage();
                break;
            case "ShopButton":
                LoadShop();
                break;
            case "TutorialButton":
                LoadTutorial();
                break;
            case "StageBackButton":
                LoadStageMenu();
                break;
            case "BackButton":
                LoadMainMenu();
                break;
            default:
                LoadStage(clickedButton);
                break;
        }
    }


    /// <summary>
    /// 스테이지 버튼을 클릭했을 때, 스테이지 정보를 읽어 이동
    /// </summary>
    private void LoadStage(Button clickedButton)
    {
        if (clickedButton == null)
        {
            Debug.LogError("[SceneLoader] LoadStage 호출 시 clickedButton이 null입니다!");
            return;
        }

        TMP_Text buttonText = clickedButton.GetComponentInChildren<TMP_Text>();
        if (buttonText == null)
        {
            Debug.LogError($"[SceneLoader] {clickedButton.gameObject.name} 버튼에서 TMP_Text를 찾을 수 없습니다!");
            return;
        }

        ExtractStageData(buttonText.text);

        if (stageNum <= 0 || subStageNum <= 0)
        {
            Debug.LogError($"[SceneLoader] 올바른 스테이지 정보가 없습니다! (stageNum: {stageNum}, subStageNum: {subStageNum})");
            return;
        }

        ApplySubStageSettings(stageNum, subStageNum);
        SceneManager.LoadScene($"Stage{stageNum}");
    }

    /// <summary>
    /// 정규식을 사용하여 "Stage X-Y" 형식에서 StageNum과 SubStageNum을 추출
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
    /// PlayButton 클릭 시 StageP_1 씬 로드
    /// </summary>
    public void LoadStageP1()
    {
        Debug.Log("[SceneLoader] StageP_1 씬 로드");
        SceneManager.LoadScene("StageP_1");
    }

    /// <summary>
    /// 다음 스테이지로 이동
    /// </summary>

    private void LoadNextStage()
    {
        if (currentStageIndex >= 3) // StageP_3이 마지막 페이지라면 이동 불가
        {
            Debug.Log("[SceneLoader] 마지막 스테이지입니다.");
            return;
        }

        currentStageIndex++;
        string nextStageName = $"StageP_{currentStageIndex}";

        Debug.Log($"[SceneLoader] 다음 스테이지 로드: {nextStageName}");
        SceneManager.LoadScene(nextStageName);
    }

    private void LoadPreviousStage()
    {
        if (currentStageIndex <= 1) // StageP_1이 첫 페이지라면 이동 불가
        {
            Debug.Log("[SceneLoader] 첫 번째 스테이지입니다.");
            return;
        }

        currentStageIndex--;
        string previousStageName = $"StageP_{currentStageIndex}";

        Debug.Log($"[SceneLoader] 이전 스테이지 로드: {previousStageName}");
        SceneManager.LoadScene(previousStageName);
    }


    /// <summary>
    /// 스테이지 메뉴로 이동
    /// </summary>
    public void LoadStageMenu()
    {
        Debug.Log("[SceneLoader] 스테이지 메뉴 로드");
        SceneManager.LoadScene("StageMenu");
    }

    /// <summary>
    /// 상점으로 이동
    /// </summary>
    public void LoadShop()
    {
        Debug.Log("[SceneLoader] 상점 씬 로드");
        SceneManager.LoadScene("Shop");
    }

    /// <summary>
    /// 튜토리얼 씬으로 이동
    /// </summary>
    public void LoadTutorial()
    {
        Debug.Log("[SceneLoader] 튜토리얼 씬 로드");
        SceneManager.LoadScene("Tutorial");
    }
    public void LoadMainMenu()
    {
        Debug.Log("[SceneLoader] 메인 메뉴 로드");
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// 스테이지 설정을 적용
    /// </summary>
    private void ApplySubStageSettings(int stageNum, int subStageNum)
    {
        PlayerPrefs.SetInt("CurrentStage", stageNum);
        PlayerPrefs.SetInt("CurrentSubStage", subStageNum);
        Debug.Log($"[SceneLoader] {stageNum}-{subStageNum} 설정 적용 완료!");
    }
}
