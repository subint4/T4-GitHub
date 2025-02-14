using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    public int gold = 500; // 초기 골드 값

    // 골드 변경 시 UI 업데이트를 위한 이벤트
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
        }
    }

    // 현재 골드 반환
    public int GetGold()
    {
        return gold;
    }

    // 골드 소비 (성공 시 true, 실패 시 false 반환)
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke(gold); // UI 업데이트 호출
            Debug.Log($"{amount} 골드 사용됨. 남은 골드: {gold}");
            return true;
        }
        else
        {
            Debug.Log("골드가 부족합니다!");
            return false;
        }
    }

    // 골드 추가
    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold); // UI 업데이트 호출
        Debug.Log($"{amount} 골드 추가됨. 현재 골드: {gold}");
    }
}
