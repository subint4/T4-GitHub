using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class MoneyManager : MonoBehaviour
{
    //다른 스크립트에서 접근 가능(몬스터 잡을시 추가 할 수 있게 하기 위함)
    public static MoneyManager instance;
    //현재 돈
    public int money = 0;
    //UI 텍스트
    public TextMeshProUGUI moneyText; // UI 텍스트

    private List<float> timeIntervals = new List<float>(); // 증가 시간 간격
    private List<int> autoIncrements = new List<int>(); // 자동 증가 금액
    private Dictionary<string, int> monsterRewards = new Dictionary<string, int>(); // 몬스터별 보상

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadMoneyConfig(); // CSV 파일 로드
        StartCoroutine(AutoGenerateMoney()); // 자동 돈 증가 시작
        UpdateMoneyUI();
    }

    // CSV 파일에서 데이터 불러오기
    private void LoadMoneyConfig()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("PlayingMoney"); // 파일 로드
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다!");
            return;
        }

        string[] lines = csvFile.text.Split('\n'); // 줄 단위로 분리
        for (int i = 1; i < lines.Length; i++) // 첫 줄(헤더) 제외
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue; // 빈 줄 무시
            string[] values = lines[i].Split(','); // 쉼표로 분리
            float time = float.Parse(values[0]); // 시간
            int amount = int.Parse(values[1]); // 자동 증가 금액
            string monsterName = values[2].Trim(); // 몬스터 이름
            int reward = int.Parse(values[3]); // 보상 금액

            timeIntervals.Add(time);
            autoIncrements.Add(amount);
            if (!monsterRewards.ContainsKey(monsterName))
            {
                monsterRewards.Add(monsterName, reward);
            }
        }
    }

    // 일정 시간마다 돈 증가
    private IEnumerator AutoGenerateMoney()
    {
        for (int i = 0; i < timeIntervals.Count; i++)
        {
            yield return new WaitForSeconds(timeIntervals[i]); // CSV에서 설정한 시간 대기
            money += autoIncrements[i]; // 자동 증가 금액 추가
            UpdateMoneyUI();
        }
    }

    // 몬스터 처치 시 보상 추가
    public void AddMonsterReward(string monsterName)
    {
        if (monsterRewards.ContainsKey(monsterName))
        {
            money += monsterRewards[monsterName]; // 몬스터 보상 추가
            UpdateMoneyUI();
        }
        else
        {
            Debug.LogWarning($"몬스터 '{monsterName}'의 보상이 설정되지 않음.");
        }
    }

    // UI 업데이트
    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "돈: " + money.ToString();
        }
    }
}