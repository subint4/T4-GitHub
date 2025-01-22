using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EnemyDatas
{
    /// <summary>
    /// ID
    /// </summary>
    public int key;

    /// <summary>
    /// 타입
    /// </summary>
    public string type;

    /// <summary>
    /// 이름
    /// </summary>
    public string name;

    /// <summary>
    /// 스탯ID
    /// </summary>
    public int statsId;

    /// <summary>
    /// 드랍확률
    /// </summary>
    public int dropRate;

    /// <summary>
    /// 프리팹 ID
    /// </summary>
    public int prefabId;

}
public class EnemyDatasLoader
{
    public List<EnemyDatas> ItemsList { get; private set; }
    public Dictionary<int, EnemyDatas> ItemsDict { get; private set; }

    public EnemyDatasLoader(string path = "JSON/EnemyDatas")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, EnemyDatas>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<EnemyDatas> Items;
    }

    public EnemyDatas GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public EnemyDatas GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
