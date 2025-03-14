using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class ItemData
{
    public int ID;
    public int ItemID;
    public string ItemName;
    public int Ammount;
    public string Type;
    public string UseType;
    public float Damage;
    public float Duration;
}

[Serializable]
public class ItemSettings
{
    public List<ItemData> Data = new List<ItemData>(); // 데이터 리스트

    private const string jsonResourcePath = "JsonData/ItemData"; // Resources 폴더 기준 경로

    /// <summary>
    /// Resources 폴더의 JsonData에서 데이터를 불러옴
    /// </summary>
    public static ItemSettings LoadFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonResourcePath); // Resources에서 JSON 파일 로드

        if (jsonFile != null)
        {
            ItemSettings loadedData = JsonConvert.DeserializeObject<ItemSettings>(jsonFile.text);

            if (loadedData != null)
            {
                Debug.Log("[ItemSettings] JSON 데이터 로드 완료.");
                return loadedData;
            }
            else
            {
                Debug.LogError("[ItemSettings] JSON 데이터 파싱 실패.");
            }
        }
        else
        {
            Debug.LogError($"[ItemSettings] JSON 파일을 찾을 수 없습니다! 경로: Resources/{jsonResourcePath}.json");
        }

        return new ItemSettings();
    }
}