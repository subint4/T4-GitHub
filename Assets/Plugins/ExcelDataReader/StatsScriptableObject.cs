using UnityEngine;
using System.IO;

[System.Serializable]
public class StatsData
{
    public string characterName;
    public int hp;
    public int attack;
    public int defense;
    public int SO;
}

[CreateAssetMenu(fileName = "StatsData", menuName = "Game/Stats Data")]
public class StatsScriptableObject : ScriptableObject
{
    public StatsData[] stats;

    private void OnEnable()
    {
        LoadDataFromJson();
    }

    public void LoadDataFromJson()
    {
        string jsonPath = Path.Combine(Application.persistentDataPath, "stats.json");
        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            stats = JsonHelper.FromJson<StatsData>(json);
            Debug.Log("Stats Data loaded from JSON.");
        }
        else
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + jsonPath);
        }
    }
}
