using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using static TowerSO;
using System;

public static class JsonToSO
{
    /// <summary>
    /// 적 데이터를 JSON에서 EnemySO로 변환
    /// </summary>
    public static void ConvertJsonToEnemySO(string jsonFilePath)
    {
        if (string.IsNullOrEmpty(jsonFilePath))
        {
            Debug.LogError("파일 경로가 비어 있습니다.");
            return;
        }

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {jsonFilePath}");
            return;
        }

        string jsonContent;
        try
        {
            jsonContent = File.ReadAllText(jsonFilePath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                Debug.LogError($"파일이 비어 있습니다: {jsonFilePath}");
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"JSON 파일을 읽는 중 오류 발생: {ex.Message}");
            return;
        }

        EnemyJsonWrapper jsonData;
        try
        {
            jsonData = JsonConvert.DeserializeObject<EnemyJsonWrapper>(jsonContent);
            if (jsonData == null)
            {
                Debug.LogError($"JSON 변환 실패: {jsonFilePath}");
                return;
            }

            if (jsonData.Data == null || jsonData.Data.Count == 0)
            {
                Debug.LogError($"JSON 변환 실패 또는 데이터가 비어 있습니다: {jsonFilePath}");
                Debug.LogError($"JSON 내용: {jsonContent}"); // JSON 원본 출력하여 디버깅
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"JSON 파싱 중 오류 발생: {ex.Message}");
            Debug.LogError($"JSON 내용: {jsonContent}"); // JSON 원본 출력하여 디버깅
            return;
        }

        foreach (var data in jsonData.Data)
        {
            if (data == null)
            {
                Debug.LogError("Enemy 데이터가 null입니다. 스킵합니다.");
                continue;
            }

            EnemySO enemySO = ScriptableObject.CreateInstance<EnemySO>();
            enemySO.LoadFromJson(data);
            SaveSO(enemySO, $"Assets/Resources/EnemySO/Enemy_{data.ID}.asset");
        }

        Debug.Log("모든 EnemySO 변환 완료!");
    }

    /// <summary>
    /// 타워 데이터를 JSON에서 TowerSO로 변환
    /// </summary>
    public static void ConvertJsonToTowerSO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {jsonFilePath}");
            return;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        var jsonData = JsonConvert.DeserializeObject<TowerJsonWrapper>(jsonContent);

        foreach (var data in jsonData.Data)
        {
            TowerSO towerSO = ScriptableObject.CreateInstance<TowerSO>();
            towerSO.LoadFromJson(data);
            SaveSO(towerSO, $"Assets/Resources/TowerSO/Tower_{data.ID}.asset");
        }
    }

    /// <summary>
    /// 웨이브 데이터를 JSON에서 WaveSO로 변환
    /// </summary>
    public static void ConvertJsonToWaveSO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {jsonFilePath}");
            return;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        var jsonData = JsonConvert.DeserializeObject<WaveJsonWrapper>(jsonContent);

        // 웨이브 데이터를 ID별로 그룹화
        Dictionary<int, List<WaveSpawnData>> waveDictionary = new Dictionary<int, List<WaveSpawnData>>();

        foreach (var data in jsonData.Data)
        {
            if (!waveDictionary.ContainsKey(data.ID))
            {
                waveDictionary[data.ID] = new List<WaveSpawnData>();
            }
            waveDictionary[data.ID].Add(data);
        }

        foreach (var waveEntry in waveDictionary)
        {
            WaveSO waveSO = ScriptableObject.CreateInstance<WaveSO>();
            waveSO.LoadFromJson(waveEntry.Value); // 리스트로 전달

            SaveSO(waveSO, $"Assets/Resources/WaveSO/Wave_{waveEntry.Key}.asset");
        }
    }

    /// <summary>
    /// ScriptableObject 저장
    /// </summary>
    private static void SaveSO(ScriptableObject so, string path)
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(so, path);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        Debug.Log($"ScriptableObject 저장됨: {path}");
    }
}

// JSON 구조체
[System.Serializable]
public class EnemyJsonWrapper
{
    public string DataType;
    public Dictionary<string, string> Metadata;
    public List<EnemyData> Data;
}

[System.Serializable]
public class TowerJsonWrapper
{
    public string DataType;
    public Dictionary<string, string> Metadata;
    public List<TowerData> Data;
}

[System.Serializable]
public class WaveJsonWrapper
{
    public string DataType;
    public Dictionary<string, string> Metadata;
    public List<WaveSpawnData> Data;
}
