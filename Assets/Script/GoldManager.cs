using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using ExcelDataReader;
using DG.Tweening;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    // 골드 UI
    [SerializeField] private TMP_Text goldText;
    // 추가되는 골드UI
    [SerializeField] private TMP_Text secondaryGoldText;
    // 골드 획득시 사운드
    [SerializeField] private AudioSource goldSound;
    [SerializeField] private AudioClip goldSoundClip;


    private Dictionary<int, (int startGold, int gainGold, int sec)> goldData = new Dictionary<int, (int, int, int)>();
    // 몬스터별 보상
    private Dictionary<string, int> monsterRewards = new Dictionary<string, int>();

    private int currentGold;
    private int currentStage;
    private float elapsedTime = 0f;
    private string excelFilePath;
    private string csvFilePath;

    // 원래 위치 저장
    private Vector3 goldTextOriginalPos;
    // 새로운 골드 텍스트 위치
    private Vector3 secondaryTextOriginalPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (goldText == null)
        {
            goldText = GameObject.Find("GoldText")?.GetComponent<TMP_Text>();
            if (goldText == null)
            {
                Debug.LogError("GoldText UI를 찾을 수 없습니다!");
            }
        }
        if (secondaryGoldText == null)
        {
            secondaryGoldText = GameObject.Find("SecondaryGoldText")?.GetComponent<TMP_Text>();
            if (secondaryGoldText == null)
            {
                Debug.LogError("SecondaryGoldText UI를 찾을 수 없습니다!");
            }
        }
        if (goldSound == null)
        {
            goldSound = gameObject.AddComponent<AudioSource>();
        }

        if (goldSoundClip == null)
        {
            goldSoundClip = Resources.Load<AudioClip>("Sounds/goldSound"); // 리소스에서 사운드 로드
        }

        goldSound.clip = goldSoundClip;
        goldSound.playOnAwake = false;

        // 원래 위치 저장
        goldTextOriginalPos = goldText.transform.position;
        secondaryTextOriginalPos = goldTextOriginalPos + new Vector3(0, -0.3f, 0);
        secondaryGoldText.transform.position = secondaryTextOriginalPos;
    
}

    private void Start()
    {
        excelFilePath = Path.Combine(Application.dataPath, "Excels/GoldData.xlsx");
        csvFilePath = Path.Combine(Application.dataPath, "Excels/GoldData.csv");

        LoadGoldData();
        SetStageByScene();
        InitializeGold();
        StartCoroutine(AutoGoldRoutine());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 씬이 변경될 때 실행 (자동 호출)
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetStageByScene();
        InitializeGold();
    }

    /// <summary>
    /// 씬 이름을 기반으로 스테이지 설정
    /// </summary>
    private void SetStageByScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"🎮 현재 씬 이름: {sceneName}");

        int stageNum = ExtractStageNumber(sceneName);

        if (stageNum == -1)
        {
            Debug.LogError("⚠ 씬 이름이 'Stage[n]' 형식이 아닙니다. 씬 이름을 변경하세요!");
            return;
        }

        SetStage(stageNum);
    }

    /// <summary>
    /// 씬 이름에서 숫자를 추출하여 스테이지 번호 변환
    /// </summary>
    private int ExtractStageNumber(string sceneName)
    {
        for (int i = 1; i <= 100; i++)
        {
            if (sceneName == $"Stage{i}")
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// GoldData.xlsx → CSV 변환 후 로드
    /// </summary>
    private void LoadGoldData()
    {
        if (File.Exists(csvFilePath))
        {
            LoadCSVData(csvFilePath);
            return;
        }

        if (File.Exists(excelFilePath))
        {
            ConvertXlsxToCsv(excelFilePath, csvFilePath);
            LoadCSVData(csvFilePath);
            return;
        }

        Debug.LogError($"❌ GoldData 파일을 찾을 수 없습니다! 경로 확인: {csvFilePath}");
    }

    /// <summary>
    /// CSV 파일을 로드하여 goldData 저장
    /// </summary>
    private void LoadCSVData(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        Debug.Log($"✅ CSV 데이터 로드 완료: {lines.Length - 1}개의 데이터");

        goldData.Clear();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] values = lines[i].Split(',');

            if (values.Length < 5) continue;

            if (int.TryParse(values[1], out int stageNum) &&
                int.TryParse(values[2], out int startGold) &&
                int.TryParse(values[3], out int sec) &&
                int.TryParse(values[4], out int gainGold))
            {
                goldData[stageNum] = (startGold, gainGold, sec);
                Debug.Log($"✅ 스테이지 {stageNum} 데이터 로드 완료 → StartGold={startGold}, Sec={sec}, GainGold={gainGold}");
            }
            else
            {
                Debug.LogError($"CSV 데이터 파싱 실패: {lines[i]}");
            }
        }
    }

    /// <summary>
    /// XLSX 파일을 CSV로 변환
    /// </summary>
    private void ConvertXlsxToCsv(string xlsxFilePath, string csvFilePath)
    {
        using (var stream = File.Open(xlsxFilePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                using (var writer = new StreamWriter(csvFilePath))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            string line = $"{reader.GetValue(0)},{reader.GetValue(1)},{reader.GetValue(2)},{reader.GetValue(3)},{reader.GetValue(4)}";
                            writer.WriteLine(line);
                        }
                    } while (reader.NextResult());
                }
            }
        }
        Debug.Log($"✅ XLSX 파일을 CSV로 변환 완료: {csvFilePath}");
    }

    /// <summary>
    /// 현재 스테이지의 초기 골드 값을 설정
    /// </summary>
    private void InitializeGold()
    {
        if (goldData.TryGetValue(currentStage, out var data))
        {
            currentGold = data.startGold;
        }
        else
        {
            currentGold = 25;
        }
        UpdateGoldUI();
    }

    /// <summary>
    /// 골드 증가
    /// </summary>
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
        AnimateGoldGain(amount);
        PlayGoldSound();
    }

    /// <summary>
    /// UI에 골드 값 업데이트
    /// </summary>
    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"{currentGold}";
        }
    }

    /// <summary>
    /// n초마다 골드 자동 증가
    /// </summary>
    private IEnumerator AutoGoldRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;

            if (goldData.TryGetValue(currentStage, out var data))
            {
                if (elapsedTime % data.sec == 0)
                {
                    AddGold(data.gainGold);
                    Debug.Log($"[{elapsedTime}초 지남 / {data.gainGold}골드 증가 / 누적 골드 {currentGold}]"); 
                }
            }
        }
    }

    /// <summary>
    /// 스테이지 설정 및 골드 초기화
    /// </summary>
    public void SetStage(int stageNum)
    {
        Debug.Log($"🔄 스테이지 변경 요청: {stageNum}");

        if (!goldData.ContainsKey(stageNum))
        {
            Debug.LogWarning($"⚠ {stageNum}번 스테이지 데이터 없음. 기본값 1로 설정.");
            stageNum = 1;
        }

        currentStage = stageNum;
        InitializeGold();
    }
    private void PlayGoldSound()
    {
        if (goldSound != null && goldSound.clip != null)
        {
            goldSound.Play();
        }
    }
    private void AnimateGoldGain(int amount)
    {
        // 추가 골드 텍스트 복제
        TMP_Text newGoldText = Instantiate(secondaryGoldText, secondaryGoldText.transform.parent);
        newGoldText.text = $"+{amount}";
        newGoldText.color = Color.yellow;
        newGoldText.gameObject.SetActive(true);

        // 초기 위치 설정 (기존 골드 텍스트 바로 아래)
        newGoldText.transform.position = secondaryTextOriginalPos;

        // 애니메이션 적용
        Sequence seq = DOTween.Sequence();
            // 0.5초 동안 살짝 아래 이동
        seq.Append(newGoldText.transform.DOMoveY(secondaryTextOriginalPos.y - 0.2f, 0.5f).SetEase(Ease.OutQuad))
           // 1.5초 유지
           .AppendInterval(1.5f)
           // 0.5초 동안 페이드아웃
           .Append(newGoldText.DOFade(0, 0.5f))
           // 애니메이션 종료 후 삭제
           .OnComplete(() => Destroy(newGoldText.gameObject));
    }
}
