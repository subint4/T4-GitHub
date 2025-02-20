using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Newtonsoft.Json;
using DG.Tweening;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [SerializeField] private TMP_Text goldText;
    [SerializeField] private GameObject goldEffectPrefab;
    [SerializeField] private Transform goldUIPosition;

    private int currentGold;
    private int gainGold;
    private float gainInterval;
    private Dictionary<int, GoldData> goldDataDictionary = new Dictionary<int, GoldData>();
    private int currentStage = 1;

    public event Action<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        LoadGoldData();
    }

    private void Start()
    {
        Debug.Log("GoldManager Start() 실행됨");

        Debug.Log($"현재 goldText: {goldText?.name}");
        Debug.Log($"현재 goldUIPosition: {goldUIPosition?.name}");
        SetStage(currentStage);
        StartCoroutine(AutoGainGold());
        UpdateGoldUI();
    }

    private void LoadGoldData()
    {
        string path = Path.Combine(Application.dataPath, "Resources/JsonData/GoldData.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var goldDataContainer = JsonConvert.DeserializeObject<GoldDataContainer>(json);
            foreach (var data in goldDataContainer.Data)
            {
                goldDataDictionary[data.stagenum] = data;
            }
            Debug.Log("Gold 데이터 로드 완료.");
        }
        else
        {
            Debug.LogError("GoldData.json 파일을 찾을 수 없습니다!");
        }
    }

    public void SetStage(int stage)
    {
        currentStage = stage;
        if (goldDataDictionary.TryGetValue(stage, out GoldData data))
        {
            currentGold = data.startgold;
            gainGold = data.gaingold;
            gainInterval = data.sec;
        }
        else
        {
            currentGold = 500;
            gainGold = 10;
            gainInterval = 5;
        }
        UpdateGoldUI();
    }

    private IEnumerator AutoGainGold()
    {
        while (true)
        {
            yield return new WaitForSeconds(gainInterval);
            AddGold(gainGold, true);
        }
    }

    public int GetGold() => currentGold;

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            return true;
        }
        else
        {
            Debug.Log("골드가 부족합니다!");
            return false;
        }
    }

    public void AddGold(int amount, bool useAnimation = false)
    {
        currentGold += amount;
        UpdateGoldUI();

        if (useAnimation)
        {
            PlayGoldAnimation(amount);
        }
    }

    private void UpdateGoldUI()
    {
        Debug.Log($"골드 UI 업데이트됨: {currentGold}");

        OnGoldChanged?.Invoke(currentGold);
        if (goldText != null)
        {
            goldText.text = currentGold.ToString();
        }
    }
    private void PlayGoldAnimation(int amount)
    {
        if (goldEffectPrefab == null || goldUIPosition == null)
        {
            Debug.LogError("골드 애니메이션 프리팹 또는 UI 위치가 설정되지 않았습니다!");
            return;
        }

        // 새로운 골드 이펙트 생성
        GameObject goldEffect = Instantiate(goldEffectPrefab, goldUIPosition.position, Quaternion.identity, goldUIPosition.parent);
        CanvasGroup canvasGroup = goldEffect.GetComponent<CanvasGroup>();

        TMP_Text goldTextEffect = goldEffect.transform.Find("GoldTextEffect").GetComponent<TMP_Text>();
        Image goldIconEffect = goldEffect.transform.Find("GoldIcon").GetComponent<Image>();

        if (goldTextEffect != null)
        {            
            goldTextEffect.enableVertexGradient = false;
            goldTextEffect.color = new Color(1f, 0.84f, 0f);
            goldTextEffect.fontMaterial.SetColor("_FaceColor", new Color(1f, 0.84f, 0f));
            goldTextEffect.text = $"+{amount}";
        }

        if (goldIconEffect != null)
        {
            Destroy(goldIconEffect.gameObject);
        }

        Vector3 startPosition = goldText.transform.position + new Vector3(-0.9f, -0.5f, 0);
        Vector3 endPosition = startPosition + new Vector3(0, -0.3f, 0);
        goldEffect.transform.position = startPosition;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(goldEffect.transform.DOMoveY(endPosition.y, 0.5f).SetEase(Ease.OutQuad))
                .Join(canvasGroup.DOFade(1, 0.2f)) // 페이드인 효과
                .AppendInterval(1.0f) // 유지 시간
                .Append(canvasGroup.DOFade(0, 0.5f)) // 페이드아웃
                .OnComplete(() => Destroy(goldEffect)); // 애니메이션 종료 후 삭제
    }
}

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
    public int startgold;
    public int sec;
    public int gaingold;
}
