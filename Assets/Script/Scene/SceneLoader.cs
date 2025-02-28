using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.EventSystems;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance { get; private set; }
    private int stageNum;
    private int subStageNum;
    private bool isLoadingStage = false;

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

        if (scene.name == "MainMenu")
        {
            Debug.Log("[SceneLoader] MainMenu에서 버튼 감지 실행");
            StartCoroutine(WaitForUIAndDetectButtons());
            return;
        }

        if (scene.name.StartsWith("StageP"))
        {
            Debug.Log("[SceneLoader] StageP에서 서브 스테이지 설정 적용");
            ApplySubStageSettings();
            StartCoroutine(WaitForUIAndDetectButtons());
            return;
        }

        if (scene.name.StartsWith("Stage"))
        {
            Debug.Log("[SceneLoader] 일반 Stage 씬에서는 SceneLoader가 개입하지 않음.");
            return;
        }
    }

    private IEnumerator WaitForUIAndDetectButtons()
    {
        yield return new WaitForSeconds(0.5f);
        DetectButtons();
    }

    private void ApplySubStageSettings()
    {
        stageNum = PlayerPrefs.GetInt("CurrentStage", 1);
        subStageNum = PlayerPrefs.GetInt("CurrentSubStage", 1);

        Debug.Log($"[SceneLoader] {stageNum}-{subStageNum} 설정 적용 완료!");

        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.LoadWavesForStage();
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

            button.onClick.RemoveAllListeners();

            if (!button.interactable)
            {
                Debug.Log($"[SceneLoader] {buttonName} 버튼은 비활성화 상태이므로 클릭 이벤트 등록하지 않음.");
                button.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(null));
                continue;
            }

            switch (buttonName)
            {
                case "NextStageButton":
                    button.onClick.AddListener(() => LoadNextStage());
                    break;
                case "PreviousStageButton":
                    button.onClick.AddListener(() => LoadPreviousStage());
                    break;
                case "HomeButton":
                    button.onClick.AddListener(() => PopupManager.Instance.ShowHomePopup());
                    break;
                default:
                    button.onClick.AddListener(() => LoadTargetScene(buttonName));
                    break;
            }
        }
    }

    private void LoadTargetScene(string buttonName)
    {
        Debug.Log($"[SceneLoader] {buttonName} 버튼 클릭됨!");

        Match match = Regex.Match(buttonName, @"Stage(\d+)-(\d+)");
        if (match.Success)
        {
            if (int.TryParse(match.Groups[1].Value, out int stageNum) &&
                int.TryParse(match.Groups[2].Value, out int subStageNum))
            {
                LoadStage(stageNum, subStageNum);
                return;
            }
        }

        switch (buttonName)
        {
            case "PlayButton":
                LoadStageP1();
                break;
            case "ShopButton":
                LoadShop();
                break;
            case "TutorialButton":
                LoadTutorial();
                break;
            case "MainMenuButton":
                LoadMainMenu();
                break;
            default:
                string stageNumberStr = buttonName.Replace("Stage", "").Trim();
                if (int.TryParse(stageNumberStr, out int stage))
                {
                    LoadStage(stage, 1);
                }
                break;
        }
    }

    public void LoadStageP1()
    {
        SceneManager.LoadScene("StageP_1");
    }

    private void LoadStage(int stageNum, int subStageNum)
    {
        if (isLoadingStage) return;
        isLoadingStage = true;

        PlayerPrefs.SetInt("CurrentStage", stageNum);
        PlayerPrefs.SetInt("CurrentSubStage", subStageNum);
        PlayerPrefs.Save();

        SceneManager.LoadScene($"Stage{stageNum}");
    }

    private void LoadNextStage()
    {
        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        if (currentStage >= 3) return;
        currentStage++;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.SetInt("CurrentSubStage", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene($"StageP_{currentStage}");
    }

    private void LoadPreviousStage()
    {
        int currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        if (currentStage <= 1) return;
        currentStage--;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.SetInt("CurrentSubStage", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene($"StageP_{currentStage}");
    }

    public void LoadShop() => SceneManager.LoadScene("Shop");
    public void LoadTutorial() => SceneManager.LoadScene("Tutorial");
    public void LoadMainMenu() => SceneManager.LoadScene("MainMenu");
}
