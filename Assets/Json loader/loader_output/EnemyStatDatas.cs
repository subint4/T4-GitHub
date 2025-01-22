using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EnemyStatDatas
{
    /// <summary>
    /// ID
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

    /// <summary>
    /// 기본 속도
    /// </summary>
    public float baseSpeed;

    /// <summary>
    /// 경험치
    /// </summary>
    public int experienceGiven;

}
public class EnemyStatDatasLoader
{
    public List<EnemyStatDatas> ItemsList { get; private set; }
    public Dictionary<int, EnemyStatDatas> ItemsDict { get; private set; }

    public EnemyStatDatasLoader(string path = "JSON/EnemyStatDatas")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, EnemyStatDatas>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<EnemyStatDatas> Items;
    }

    public EnemyStatDatas GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public EnemyStatDatas GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
