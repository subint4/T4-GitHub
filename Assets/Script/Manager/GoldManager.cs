using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [SerializeField] private TMP_Text goldText;
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
            AddGold(gainGold);
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

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        OnGoldChanged?.Invoke(currentGold);
        if (goldText != null)
        {
            goldText.text = currentGold.ToString();
        }
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
