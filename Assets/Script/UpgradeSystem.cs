using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public int CurrentMoney = 100;
    public int UpgradeCost = 50;
    public int CurrentTier = 1;
    public int MaxTier = 3;
    private void Start()
    {
        Debug.Log($"현재 재화 : {CurrentMoney}, 업그레이드 단계 : {CurrentTier}");
    }
    public void Upgrade()
    {
        if (CurrentTier >= MaxTier)
        {
            Debug.Log("최대 단계에 도달했습니다.");
            return;
        }
        if (CurrentMoney >= UpgradeCost)
        {
            CurrentMoney -= UpgradeCost;
            CurrentTier++;
            UpgradeCost = Mathf.CeilToInt(UpgradeCost * 1.5f);

            Debug.Log($"업그레이드 완료. 현재 단계: {CurrentTier},남은 재화 : {CurrentMoney}");
        }
        else
        {
            Debug.Log("돈이 모자랍니다.");
        }
    }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.U))
            {
                Upgrade();
            }
        }
}