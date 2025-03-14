using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

[Serializable]
public class UserItemData
{
    public int ItemID;
    public string ItemName;
    public int Quantity;
}

[Serializable]
public class StageClearData
{
    public int StageNum;
    public int SubStageNum;
    public bool IsCleared;
}

[Serializable]
public class UserData
{
    public string UserName;
    public int Gold;
    public List<UserItemData> OwnedItems = new List<UserItemData>();
    public List<StageClearData> ClearedStages = new List<StageClearData>();
}

public class UserDataManager
{
    private static readonly string jsonPath = Path.Combine(Application.persistentDataPath, "UserData.json");

    public static UserData LoadUserData()
    {
        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            UserData loadedData = JsonConvert.DeserializeObject<UserData>(json);

            if (loadedData != null)
            {
                Debug.Log("[UserDataManager] 유저 데이터 로드 완료.");
                return loadedData;
            }
        }

        Debug.LogWarning("[UserDataManager] 유저 데이터가 존재하지 않습니다. 기본 데이터를 생성합니다.");
        return CreateDefaultUserData();
    }

    public static void SaveUserData(UserData userData)
    {
        string json = JsonConvert.SerializeObject(userData, Formatting.Indented);
        File.WriteAllText(jsonPath, json);
        Debug.Log("[UserDataManager] 유저 데이터 저장 완료. 경로: " + jsonPath);
    }

    private static UserData CreateDefaultUserData()
    {
        UserData newUserData = new UserData
        {
            UserName = "NewPlayer",
            Gold = 1000,
            OwnedItems = new List<UserItemData>
            {
                new UserItemData { ItemID = 101, ItemName = "Bomb", Quantity = 3 },
                new UserItemData { ItemID = 102, ItemName = "Rocket", Quantity = 2 },
                new UserItemData { ItemID = 103, ItemName = "Stun", Quantity = 1 }
            },
            ClearedStages = new List<StageClearData>()
        };

        SaveUserData(newUserData);
        return newUserData;
    }

    public static void AddItem(UserData userData, int itemID, string itemName, int amount)
    {
        UserItemData item = userData.OwnedItems.Find(i => i.ItemID == itemID);

        if (item != null)
        {
            item.Quantity += amount;
        }
        else
        {
            userData.OwnedItems.Add(new UserItemData { ItemID = itemID, ItemName = itemName, Quantity = amount });
        }

        SaveUserData(userData);
        Debug.Log($"[UserDataManager] 아이템 추가: {itemName} ({amount}개)");
    }
}
