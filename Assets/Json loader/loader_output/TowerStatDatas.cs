using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class TowerStatDatas
{
    /// <summary>
    /// 키
    /// </summary>
    public int key;

    /// <summary>
    /// 캐릭터 이름
    /// </summary>
    public string name;

}
public class TowerStatDatasLoader
{
    public List<TowerStatDatas> ItemsList { get; private set; }
    public Dictionary<int, TowerStatDatas> ItemsDict { get; private set; }

    public TowerStatDatasLoader(string path = "JSON/TowerStatDatas")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, TowerStatDatas>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<TowerStatDatas> Items;
    }

    public TowerStatDatas GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public TowerStatDatas GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
