using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Newtonsoft.Json;
using System.IO;
using System;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [SerializeField] private TMP_Text goldText;
    [SerializeField] private GameObject goldEffectPrefab;
    [SerializeField] private Transform goldUIPosition;

    private Dictionary<(int, int), GoldData> goldDataDictionary = new Dictionary<(int, int), GoldData>();

    private int currentGold;
    private int gainGold;
    private float gainInterval;

    private int currentStage;
    private int currentSubStage;

    public event Action<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadGoldData(); // JSON에서 골드 데이터 로드
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Debug.Log("[GoldManager] 골드 매니저 시작됨");

        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged += SetStage;
        }

        SetStage(StageManager.Instance?.currentStageNum ?? 1, StageManager.Instance?.GetCurrentSubStageNum() ?? 1);

        StartCoroutine(AutoGainGold());
        UpdateGoldUI();
    }

    /// <summary>
    /// **골드 데이터 JSON 로드**
    /// </summary>
    private void LoadGoldData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JsonData/GoldData");

        if (jsonFile == null)
        {
            Debug.LogError("[GoldManager] GoldData.json 파일을 찾을 수 없습니다!");
            return;
        }

        GoldDataContainer goldDataContainer = JsonConvert.DeserializeObject<GoldDataContainer>(jsonFile.text);

        if (goldDataContainer == null || goldDataContainer.Data == null)
        {
            Debug.LogError("[GoldManager] JSON 데이터를 불러오지 못했습니다.");
            return;
        }

        foreach (var data in goldDataContainer.Data)
        {
            var key = (data.stagenum, data.SubStagenum);
            goldDataDictionary[key] = data;
        }

        Debug.Log($"[GoldManager] {goldDataDictionary.Count}개의 골드 데이터 로드 완료.");
    }

    /// <summary>
    /// **스테이지별 골드 설정**
    /// </summary>
    public void SetStage(int stage, int subStage)
    {
        currentStage = stage;
        currentSubStage = subStage;

        var key = (stage, subStage);
        if (goldDataDictionary.TryGetValue(key, out GoldData data))
        {
            currentGold = data.startgold;
            gainGold = data.gaingold;
            gainInterval = data.sec;
            Debug.Log($"[GoldManager] {stage}-{subStage} 골드 데이터 설정 완료: StartGold={currentGold}, GainGold={gainGold}, GainInterval={gainInterval}");
        }
        else
        {
            Debug.LogWarning($"[GoldManager] {stage}-{subStage}에 대한 골드 데이터를 찾을 수 없습니다. 기본값 사용");
            currentGold = 500;
            gainGold = 10;
            gainInterval = 5;
        }

        UpdateGoldUI();
    }

    /// <summary>
    /// **자동 골드 획득 (주기적)**
    /// </summary>
    private IEnumerator AutoGainGold()
    {
        while (true)
        {
            yield return new WaitForSeconds(gainInterval);
            AddGold(gainGold, true);
        }
    }

    /// <summary>
    /// **현재 보유 골드 반환**
    /// </summary>
    public int GetGold() => currentGold;

    /// <summary>
    /// **골드 사용 가능 여부 확인**
    /// </summary>
    public bool CanAfford(int amount) => currentGold >= amount;

    /// <summary>
    /// **골드 사용 (차감)**
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (CanAfford(amount))
        {
            currentGold -= amount;
            UpdateGoldUI();
            Debug.Log($"[GoldManager] {amount} 골드 차감됨. 현재 보유 골드: {currentGold}");
            return true;
        }
        else
        {
            Debug.LogWarning("[GoldManager] 골드가 부족합니다!");
            return false;
        }
    }

    /// <summary>
    /// **골드 추가**
    /// </summary>
    public void AddGold(int amount, bool useAnimation = false)
    {
        currentGold += amount;
        UpdateGoldUI();

        if (useAnimation)
        {
            PlayGoldAnimation(amount);
        }
    }

    /// <summary>
    /// **UI 업데이트**
    /// </summary>
    private void UpdateGoldUI()
    {
        OnGoldChanged?.Invoke(currentGold);
        if (goldText != null)
        {
            goldText.text = currentGold.ToString();
        }
    }

    /// <summary>
    /// **골드 획득 애니메이션**
    /// </summary>
    private void PlayGoldAnimation(int amount)
    {
        if (goldEffectPrefab == null || goldUIPosition == null)
        {
            Debug.LogError("[GoldManager] 골드 애니메이션 프리팹 또는 UI 위치가 설정되지 않았습니다!");
            return;
        }

        GameObject goldEffect = Instantiate(goldEffectPrefab, goldUIPosition.position, Quaternion.identity, goldUIPosition.parent);
        CanvasGroup canvasGroup = goldEffect.GetComponent<CanvasGroup>();

        TMP_Text goldTextEffect = goldEffect.transform.Find("GoldTextEffect")?.GetComponent<TMP_Text>();
        if (goldTextEffect != null)
        {
            goldTextEffect.text = $"+{amount}";
            goldTextEffect.color = new Color(1f, 0.84f, 0f);
        }

        Vector3 startPosition = goldText.transform.position + new Vector3(-0.9f, -0.5f, 0);
        Vector3 endPosition = startPosition + new Vector3(0, -0.3f, 0);
        goldEffect.transform.position = startPosition;

        StartCoroutine(FadeOutGoldEffect(goldEffect, canvasGroup, endPosition));
    }

    /// <summary>
    /// **골드 애니메이션 페이드 아웃**
    /// </summary>
    private IEnumerator FadeOutGoldEffect(GameObject effect, CanvasGroup canvasGroup, Vector3 endPosition)
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            effect.transform.position = Vector3.Lerp(effect.transform.position, endPosition, elapsedTime / duration);
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(effect);
    }
}

// JSON 데이터를 저장할 클래스들
[Serializable]
public class GoldDataContainer
{
    public List<GoldData> Data;
}

[Serializable]
public class GoldData
{
    public int id;
    public int stagenum;
    public int SubStagenum;
    public int startgold;
    public int sec;
    public int gaingold;
}
