using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using UnityEngine;
public enum TowerType
{
    Default,  // 기본형
    Slow,     // 슬로우 효과
    Pierce,   // 관통 공격
    Stun      // 기절 효과
}
[CreateAssetMenu(fileName = "NewTowerData", menuName = "Game Data/Tower")]
public class TowerSO : ScriptableObject
{
    public int ID;
    public string Name;
    public int NextLevelID;
    public float Health;
    public int Level;
    public float AttackPower;
    public float AttackSpeed;
    public int DeployCost;
    public int UpgradeCost;
    public TowerType TowerType;
    public float SlowEffect;
    public float SlowDuration;
    public int PierceCount;
    public float StunDuration;
    public string AttackAnimationName;  // 추가된 필드


    public void LoadFromJson(TowerData data)
    {
        ID = data.ID;
        Name = data.Name;
        NextLevelID = data.NextLevelID;
        Health = (int)data.Health;
        Level = data.Level;
        AttackPower = (int)data.AttackPower;
        AttackSpeed = data.AttackSpeed;
        DeployCost = data.DeployCost;
        UpgradeCost = data.UpgradeCost;
        SlowEffect = data.SlowEffect;
        SlowDuration = data.SlowDuration;
        PierceCount = data.PierceCount;
        StunDuration = data.StunDuration;
       

        if (System.Enum.TryParse(data.Type.Trim(), true, out TowerType parsedType))
        {
            TowerType = parsedType;
        }
        else
        {
            Debug.LogError($"{Name}: TowerType 변환 실패! 기본값(Default) 적용.");
            TowerType = TowerType.Default;
        }
    }

[System.Serializable]
public class TowerData
{
    public int ID;
    public string Name;
    public int NextLevelID;
    public float Health;
    public int Level;
    public float AttackPower;
    public float AttackSpeed;
    public int DeployCost;
    public int UpgradeCost;
    public string Type;
    public float SlowEffect;
    public float SlowDuration;
    public int PierceCount;
    public float StunDuration;
}
}
