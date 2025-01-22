using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class WaveDatas
{
    /// <summary>
    /// ID
    /// </summary>
    public int key;

    /// <summary>
    /// enemy ID
    /// </summary>
    public List<int> enemies;

    /// <summary>
    /// 적의 수
    /// </summary>
    public List<int> counts;

    /// <summary>
    /// 보스 ID
    /// </summary>
    public int bossID;

    /// <summary>
    /// hp 배율
    /// </summary>
    public float hpMultiplier;

}
public class WaveDatasLoader
{
    public List<WaveDatas> ItemsList { get; private set; }
    public Dictionary<int, WaveDatas> ItemsDict { get; private set; }

    public WaveDatasLoader(string path = "JSON/WaveDatas")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, WaveDatas>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<WaveDatas> Items;
    }

    public WaveDatas GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public WaveDatas GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
