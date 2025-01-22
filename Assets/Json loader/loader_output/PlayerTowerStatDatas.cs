using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerTowerStatDatas
{
    /// <summary>
    /// 키
    /// </summary>
    public int key;

    /// <summary>
    /// 기본 체력
    /// </summary>
    public int baseHealth;

    /// <summary>
    /// 기본 공격력
    /// </summary>
    public int baseAttack;

}
public class PlayerTowerStatDatasLoader
{
    public List<PlayerTowerStatDatas> ItemsList { get; private set; }
    public Dictionary<int, PlayerTowerStatDatas> ItemsDict { get; private set; }

    public PlayerTowerStatDatasLoader(string path = "JSON/PlayerTowerStatDatas")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, PlayerTowerStatDatas>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<PlayerTowerStatDatas> Items;
    }

    public PlayerTowerStatDatas GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public PlayerTowerStatDatas GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
