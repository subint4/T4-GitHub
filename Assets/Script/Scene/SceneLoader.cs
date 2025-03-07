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

        isLoadingStage = false;

        // Stage로 이동 시 GameManager 자동 생성
        if (IsStageScene(scene.name))
        {
            if (GameManager.Instance == null)
            {
                Debug.Log("[SceneLoader] GameManager가 없어서 새로 생성됨.");
                GameObject gameManagerPrefab = Resources.Load<GameObject>("Prefabs/GameManager");
                if (gameManagerPrefab != null)
                {
                    Instantiate(gameManagerPrefab);
                }
                else
                {
                    Debug.LogError("[SceneLoader] GameManager 프리팹을 찾을 수 없습니다!");
                }
            }
        }
        else if (scene.name == "MainMenu")
        {
            Debug.Log("[SceneLoader] MainMenu에서 버튼 감지 실행");

            // MainMenu에서는 GameManager 삭제
            if (GameManager.Instance != null)
            {
                Destroy(GameManager.Instance.gameObject);
                Debug.Log("[SceneLoader] GameManager 삭제 완료.");
            }

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
    }

    private IEnumerator WaitForUIAndDetectButtons()
    {
        yield return new WaitForSeconds(0.5f);
        DetectButtons();
    }

    private void ApplySubStageSettings()
    {
        int stageNum = PlayerPrefs.GetInt("CurrentStage", 1);
        int subStageNum = PlayerPrefs.GetInt("CurrentSubStage", 1);

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

        // 현재 씬 이름 가져오기
        string currentScene = SceneManager.GetActiveScene().name;

        // StageP2, StageP3에서는 버튼을 비활성화
        if (currentScene == "StageP2" || currentScene == "StageP3")
        {
            Debug.LogWarning($"[SceneLoader] {currentScene}에서는 버튼 클릭을 비활성화합니다.");
            return; // 여기서 함수 종료 → 버튼 클릭 불가능
        }

        foreach (var button in buttons)
        {
            string buttonName = button.gameObject.name;
            Debug.Log($"[SceneLoader] 감지된 버튼: {buttonName}");

            button.onClick.RemoveAllListeners(); // 기존 이벤트 제거

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
                    button.onClick.AddListener(() => LoadMainMenu());
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

        // 현재 씬이 StageP2 또는 StageP3일 경우, 특정 버튼 제한
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "StageP2" || currentScene == "StageP3")
        {
            Debug.LogWarning($"[SceneLoader] {currentScene}에서 스테이지 이동 제한됨!");
            return;
        }

        // 버튼 이름에서 Stage 번호와 SubStage 번호를 정확히 추출
        Match match = Regex.Match(buttonName, @"Stage(\d+)-(\d+)");
        if (match.Success)
        {
            if (int.TryParse(match.Groups[1].Value, out int stageNum) &&
                int.TryParse(match.Groups[2].Value, out int subStageNum))
            {
                Debug.Log($"[SceneLoader] 버튼에서 추출된 스테이지: {stageNum}-{subStageNum}");
                LoadStage(stageNum, subStageNum);
                return;
            }
        }

        // 기본 버튼 로직 처리
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
            case "BackButton":
                LoadMainMenu();
                break;
            default:
                // 버튼 이름이 "StageX" 형식이라면 해당 Stage로 이동
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
        if (isLoadingStage) return; // 중복 실행 방지
        isLoadingStage = true;

        Debug.Log($"[SceneLoader] {stageNum}-{subStageNum} 스테이지 로드 준비 중...");

        // GameManager 데이터 유지 & 상태 초기화
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameManager();
        }

        // 스테이지 정보 저장
        PlayerPrefs.SetInt("CurrentStage", stageNum);
        PlayerPrefs.SetInt("CurrentSubStage", subStageNum);
        PlayerPrefs.Save();

        // 씬 로드
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

    public void LoadShop() => SceneManager.LoadScene("Shop_pack");
    public void LoadTutorial() => SceneManager.LoadScene("Tutorial");

    public void LoadMainMenu()
    {
        Debug.Log("[SceneLoader] 메인 메뉴 로드 중...");

        // GameManager 삭제
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
            Debug.Log("[SceneLoader] GameManager 삭제 완료.");
        }

        SceneManager.LoadScene("MainMenu");
    }

    private bool IsStageScene(string sceneName)
    {
        return sceneName.StartsWith("Stage");
    }
}
