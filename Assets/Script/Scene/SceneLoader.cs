using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
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

    private void OnEnable()
    {
        Invoke(nameof(DetectButtons), 0.2f); // UI 로드 후 버튼 감지
    }

    /// <summary>
    /// 씬이 변경될 때 자동으로 버튼 감지
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneLoader] 씬 로드됨: {scene.name}");
        DetectButtons(); // 씬이 변경될 때 버튼을 다시 감지
    }

    private IEnumerator WaitForUIAndDetectButtons()
    {
        yield return new WaitForSeconds(0.5f); // UI가 로드될 시간을 보장
        DetectButtons();
    }


    /// <summary>
    /// 모든 버튼을 찾아 클릭 이벤트 등록
    /// </summary>
    private void DetectButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        Debug.Log($"[SceneLoader] {buttons.Length}개의 버튼 감지 완료.");

        foreach (var button in buttons)
        {
            string buttonName = button.gameObject.name;
            Debug.Log($"[SceneLoader] 감지된 버튼: {buttonName}");

            // 기존 리스너 제거 (중복 방지)
            button.onClick.RemoveAllListeners();

            // 스테이지 버튼 감지 및 비활성화 처리
            Match match = Regex.Match(buttonName, @"Stage\s*(\d+)-(\d+)");

            if (match.Success)
            {
                int stageNum = int.Parse(match.Groups[1].Value);

                if (!IsStageUnlocked(stageNum))
                {
                    button.interactable = false; // 버튼 비활성화
                    Debug.Log($"[SceneLoader] {buttonName} 버튼 비활성화됨.");
                    continue; // 이벤트 등록 방지
                }

                button.interactable = true;
            }

            // 버튼 클릭 이벤트 추가
            button.onClick.AddListener(() =>
            {
                Debug.Log($"[SceneLoader] {buttonName} 클릭 이벤트 실행됨!");
                LoadTargetScene(buttonName);
            });
        }
    }

    private void LoadTargetScene(string buttonName)
    {
        Debug.Log($"[SceneLoader] {buttonName} 버튼 클릭됨!");

        switch (buttonName)
        {
            case "PlayButton":
                LoadStageP1();
                break;
            case "NextStageButton":
                LoadNextStage();
                break;
            case "PreviousStageButton":
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
            case "MainMenuButton":
                LoadMainMenu();
                break;
            default:
                Debug.Log($"[SceneLoader] {buttonName}는 스테이지 버튼입니다. LoadStage() 호출");
                LoadStage(buttonName);
                break;
        }
    }
    private void LoadStage(string buttonName)
    {
        Match match = Regex.Match(buttonName, @"Stage\s*(\d+)-(\d+)");

        if (!match.Success)
        {
            Debug.LogWarning($"[SceneLoader] {buttonName}에서 유효한 스테이지 번호를 찾을 수 없습니다.");
            return;
        }

        int stageNum = int.Parse(match.Groups[1].Value);
        int subStageNum = int.Parse(match.Groups[2].Value);

        if (!IsStageUnlocked(stageNum))
        {
            Debug.LogWarning($"[SceneLoader] {stageNum} 스테이지는 잠겨 있습니다!");
            return;
        }

        // 선택된 스테이지 정보를 저장하여 GameManager에서 사용할 수 있도록 함
        ApplySubStageSettings(stageNum, subStageNum);

        // Stage1 씬만 로드하고 내부 데이터를 SubStage에 맞게 설정
        Debug.Log($"[SceneLoader] Stage1 로드, 데이터는 {stageNum}-{subStageNum} 적용");
        SceneManager.LoadScene("Stage1");
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

    public void LoadStageP1()
    {
        Debug.Log("[SceneLoader] StageP_1 씬 로드");
        SceneManager.LoadScene("StageP_1");
    }

    private void LoadNextStage()
    {
        if (currentStageIndex >= 3)
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
        if (currentStageIndex <= 1)
        {
            Debug.Log("[SceneLoader] 첫 번째 스테이지입니다.");
            return;
        }

        currentStageIndex--;
        string previousStageName = $"StageP_{currentStageIndex}";

        Debug.Log($"[SceneLoader] 이전 스테이지 로드: {previousStageName}");
        SceneManager.LoadScene(previousStageName);
    }

    public void LoadStageMenu()
    {
        Debug.Log("[SceneLoader] 스테이지 메뉴 로드");
        SceneManager.LoadScene("StageMenu");
    }

    public void LoadShop()
    {
        Debug.Log("[SceneLoader] 상점 씬 로드");
        SceneManager.LoadScene("Shop");
    }

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

    private void ApplySubStageSettings(int stageNum, int subStageNum)
    {
        PlayerPrefs.SetInt("CurrentStage", stageNum);
        PlayerPrefs.SetInt("CurrentSubStage", subStageNum);
        Debug.Log($"[SceneLoader] {stageNum}-{subStageNum} 설정 적용 완료!");
    }
    private bool IsStageUnlocked(int stageNum)
    {
        // MainMenu 및 1스테이지는 항상 해금된 상태
        if (stageNum == 0 || stageNum == 1) return true;

        // 이전 스테이지가 클리어되지 않았다면 잠금
        string prevStageKey = $"Stage{stageNum - 1}_Cleared";
        int isPrevCleared = PlayerPrefs.GetInt(prevStageKey, 0);

        bool isUnlocked = isPrevCleared == 1;

        Debug.Log($"[SceneLoader] 스테이지 {stageNum} 해금 여부: {isUnlocked} (이전 스테이지 Cleared 값: {isPrevCleared})");

        return isUnlocked;
    }


}