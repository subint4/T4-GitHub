using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public int currentGold = 25;
    public TMP_Text goldText;
    public int goldIncrement = 10;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        UpdateGoldUI();
        StartCoroutine(AutoAddGold());
    }
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
    }
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            Debug.Log($"골드 소모 : {amount}, 잔량 : {currentGold}");
            return true;
        }
        else
        {
            Debug.Log("골드부족");
            return false;
        }
    }
    public void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"{currentGold}";
        }
    }
    private IEnumerator AutoAddGold()
    {
        while (true)
        {
            //5초 후
            yield return new WaitForSeconds(5f);
            AddGold(goldIncrement);
            Debug.Log($"5초마다 {goldIncrement} 골드 추가됨. 현재 골드: {currentGold}");
        }
    }
    public bool SpendGoldForTower(TowerSO tower, bool isUpgrade)
    {
        if (tower == null)
        {
            Debug.LogError("타워 정보가 없습니다!");
            return false;
        }

        int cost = isUpgrade ? tower.UpgradeCost : tower.DeployCost;


        return SpendGold(cost);
    }

}