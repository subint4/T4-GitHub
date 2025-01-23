using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class CostDatas
{
    /// <summary>
    /// 키값
    /// </summary>
    public int key;

    /// <summary>
    /// 필요 재화
    /// </summary>
    public int CostRequired;

    /// <summary>
    /// 총 재화
    /// </summary>
    public int totalCost;

    /// <summary>
    /// 스텟 id
    /// </summary>
    public int statID;

}
public class CostDatasLoader
{
    public List<CostDatas> ItemsList { get; private set; }
    public Dictionary<int, CostDatas> ItemsDict { get; private set; }

    public CostDatasLoader(string path = "JSON/CostDatas")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, CostDatas>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<CostDatas> Items;
    }

    public CostDatas GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public CostDatas GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
