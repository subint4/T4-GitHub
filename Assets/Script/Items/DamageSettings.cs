using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class DamageItemData
{
    public string ItemName;
    public float damageAmount;
    public float stunDuration;
}

[Serializable]
public class DamageItemSettings
{
    public List<DamageItemData> items;

    public static DamageItemSettings LoadSettings()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JsonData/ItemData");

        if (jsonFile == null)
        {
            Debug.LogError("[DamageItemSettings] JSON 파일을 찾을 수 없습니다! 경로: Resources/JsonData/ItemData.json");
            return new DamageItemSettings();
        }

        Debug.Log("[DamageItemSettings] JSON 파일 로드 성공!");
        return JsonConvert.DeserializeObject<DamageItemSettings>(jsonFile.text);
    }
}
