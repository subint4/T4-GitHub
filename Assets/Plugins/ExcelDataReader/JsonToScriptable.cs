using UnityEngine;
using UnityEditor;
using System.IO;

public class JsonToScriptable
{
    public static void ConvertJsonToSO()
    {
        // JSON 파일 읽기
        string[] jsonFiles = Directory.GetFiles(Application.persistentDataPath, "*.json");
        if (jsonFiles.Length == 0)
        {
            Debug.LogError("JSON 파일이 존재하지 않습니다.");
            return;
        }

        foreach (string jsonPath in jsonFiles)
        {
            string json = File.ReadAllText(jsonPath);
            StatsData[] statsArray = JsonHelper.FromJson<StatsData>(json);

            // ScriptableObject 생성
            StatsScriptableObject statsSO = ScriptableObject.CreateInstance<StatsScriptableObject>();
            statsSO.stats = statsArray;

            // ScriptableObject 저장
            string fileName = Path.GetFileNameWithoutExtension(jsonPath); // JSON 파일 이름으로 저장
            string assetPath = $"Assets/Resources/{fileName}.asset";
            AssetDatabase.CreateAsset(statsSO, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"ScriptableObject 생성 완료: {assetPath}");
        }
    }
}
