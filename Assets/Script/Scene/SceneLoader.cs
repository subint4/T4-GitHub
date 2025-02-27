using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.EventSystems;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
    private int stageNum;
    private int subStageNum;
    private bool isLoadingStage = false; // 중복 방지 플래그

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneLoader] 씬 로드됨: {scene.name}");

        StartCoroutine(WaitForUIAndDetectButtons());

        if (scene.name == "MainMenu")
        {
            Debug.Log("[SceneLoader] 메인 메뉴에서 버튼 다시 감지");
            DetectButtons(); // MainMenu에서는 즉시 버튼 감지
        }

        // Stage 씬에서는 서브 스테이지 설정 적용
        if (scene.name.StartsWith("Stage"))
        {
            ApplySubStageSettings();
        }
    }

    private IEnumerator WaitForUIAndDetectButtons()
    {
        yield return new WaitForSeconds(0.5f); // UI가 완전히 로드될 시간을 확보
        DetectButtons();
    }

    private void CheckAndResetStage()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
        {
            Debug.Log("[SceneLoader] 메인 메뉴로 돌아와서 진행 정보 초기화!");
            PlayerPrefs.SetInt("CurrentStage", 1);
            PlayerPrefs.SetInt("CurrentSubStage", 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// `Stage1` 씬을 로드한 후 내부적으로 `Stage1-1` 데이터를 불러옴
    /// </summary>
    private void ApplySubStageSettings()
    {
        stageNum = PlayerPrefs.GetInt("CurrentStage", 1);
        subStageNum = PlayerPrefs.GetInt("CurrentSubStage", 1);

        Debug.Log($"[SceneLoader] {stageNum}-{subStageNum} 설정 적용 완료!");

        // WaveManager가 있는 경우, 현재 서브 스테이지에 맞는 데이터 적용
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.LoadWavesForStage(stageNum, subStageNum);
            Debug.Log($"[SceneLoader] {stageNum}-{subStageNum} 웨이브 데이터 로드 완료!");
        }
        else
        {
            Debug.LogWarning("[SceneLoader] WaveManager를 찾을 수 없음, 웨이브 데이터 적용 생략");
        }
    }

    private void DetectButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        Debug.Log($"[SceneLoader] {buttons.Length}개의 버튼 감지 완료.");

        foreach (var button in buttons)
        {
            string buttonName = button.gameObject.name;
            Debug.Log($"[SceneLoader] 감지된 버튼: {buttonName}");

            button.onClick.RemoveAllListeners(); // 중복 이벤트 방지

            // 비활성화된 버튼은 클릭 이벤트만 해제하고, 진행을 멈추지 않도록 수정
            if (!button.interactable)
            {
                Debug.Log($"[SceneLoader] {buttonName} 버튼은 비활성화 상태이므로 클릭 이벤트 등록하지 않음.");
                button.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(null)); // 포커스 해제
                continue;
            }

            switch (buttonName)
            {
                case "NextStageButton":
                    button.onClick.AddListener(() => LoadNextStage());
                    Debug.Log("[SceneLoader] NextStageButton 감지됨!");
                    break;
                case "PreviousStageButton":
                    button.onClick.AddListener(() => LoadPreviousStage());
                    Debug.Log("[SceneLoader] PreviousStageButton 감지됨!");
                    break;
                case "HomeButton":
                    button.onClick.AddListener(() => PopupManager.Instance.ShowHomePopup());
                    Debug.Log("[SceneLoader] HomeButton 감지됨! PopupManager에서 관리");
                    break;
                default:
                    Debug.Log($"[SceneLoader] {buttonName} 버튼 기본 처리");
                    button.onClick.AddListener(() => LoadTargetScene(buttonName));
                    break;
            }
        }
    }



    private void LoadTargetScene(string buttonName)
    {
        Debug.Log($"[SceneLoader] {buttonName} 버튼 클릭됨!");

        // 숫자가 포함된 StageX-Y 형식인지 확인 (예: "Stage1-2")
        Match match = Regex.Match(buttonName, @"Stage(\d+)-(\d+)");
        if (match.Success)
        {
            if (int.TryParse(match.Groups[1].Value, out int stageNum) &&
                int.TryParse(match.Groups[2].Value, out int subStageNum))
            {
                Debug.Log($"[SceneLoader] Stage{stageNum}-{subStageNum} 로드 요청됨.");
                LoadStage(stageNum, subStageNum);
                return;
            }
        }

        switch (buttonName)
        {
            case "PlayButton":
                Debug.Log("[SceneLoader] PlayButton 클릭 -> StageP1 로드");
                LoadStageP1();
                break;
            case "NextStageButton":
                Debug.Log("[SceneLoader] NextStageButton 클릭 -> 다음 스테이지 로드");
                LoadNextStage();
                break;
            case "PreviousStageButton":
                Debug.Log("[SceneLoader] PreviousStageButton 클릭 -> 이전 스테이지 로드");
                LoadPreviousStage();
                break;
            case "ShopButton":
                Debug.Log("[SceneLoader] ShopButton 클릭 -> 상점 로드");
                LoadShop();
                break;
            case "TutorialButton":
                Debug.Log("[SceneLoader] TutorialButton 클릭 -> 튜토리얼 로드");
                LoadTutorial();
                break;
            case "StageBackButton":
            case "BackButton":
            case "MainMenuButton":
                Debug.Log("[SceneLoader] 메인 메뉴로 이동");
                LoadMainMenu();
                break;
            default:
                // 숫자가 포함되지 않은 버튼은 Stage 변환을 수행하지 않음
                if (!Regex.IsMatch(buttonName, @"\d+"))
                {
                    Debug.Log($"[SceneLoader] '{buttonName}'은 숫자가 없는 버튼이므로 Stage 변환을 수행하지 않음.");
                    return;
                }

                // 숫자가 포함된 StageX 형식인지 확인 (예: "Stage1")
                string stageNumberStr = buttonName.Replace("Stage", "").Trim();
                if (int.TryParse(stageNumberStr, out int stage))
                {
                    Debug.Log($"[SceneLoader] Stage{stage} 로드 요청됨 (기본 서브 스테이지 1).");
                    LoadStage(stage, 1); // 기본적으로 서브 스테이지 1로 설정
                }
                else
                {
                    Debug.LogError($"[SceneLoader] {buttonName}에서 올바른 Stage 번호를 찾을 수 없습니다.");
                }
                break;
        }
    }



    public void LoadStageP1()
    {
        Debug.Log("[SceneLoader] StageP_1 씬 로드");
        SceneManager.LoadScene("StageP_1");
    }


    private void LoadStage(int stageNum, int subStageNum)
    {
        if (isLoadingStage)
        {
            Debug.Log("[SceneLoader] 스테이지 로딩 중이므로 중복 실행 방지됨.");
            return;
        }

        isLoadingStage = true; // 로딩 시작

        Debug.Log($"[SceneLoader] StageP{stageNum} - {subStageNum} 로드 중...");
        PlayerPrefs.SetInt("CurrentStage", stageNum);
        PlayerPrefs.SetInt("CurrentSubStage", subStageNum);
        PlayerPrefs.Save();

        SceneManager.LoadScene($"Stage{stageNum}");
    }

    private void LoadNextStage()
    {
        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);

        if (currentStage >= 3) // 마지막 스테이지가 StageP3인 경우
        {
            Debug.Log("[SceneLoader] 마지막 스테이지입니다.");
            return;
        }

        currentStage++;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.SetInt("CurrentSubStage", 1);
        PlayerPrefs.Save();

        string nextStageName = $"StageP_{currentStage}";
        Debug.Log($"[SceneLoader] 다음 스테이지 로드: {nextStageName}");
        SceneManager.LoadScene(nextStageName);
    }

    private void LoadPreviousStage()
    {
        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);

        if (currentStage <= 1)
        {
            Debug.Log("[SceneLoader] 첫 번째 스테이지입니다.");
            return;
        }

        currentStage--;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.SetInt("CurrentSubStage", 1);
        PlayerPrefs.Save();

        string previousStageName = $"StageP_{currentStage}";
        Debug.Log($"[SceneLoader] 이전 스테이지 로드: {previousStageName}");
        SceneManager.LoadScene(previousStageName);
    }


    public void LoadShop() => SceneManager.LoadScene("Shop");
    public void LoadTutorial() => SceneManager.LoadScene("Tutorial");
    public void LoadMainMenu() => SceneManager.LoadScene("MainMenu");
}
