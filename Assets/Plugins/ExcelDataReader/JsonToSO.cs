using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
<<<<<<< Updated upstream
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
=======
        string jsonFolder = "Assets/Resources/JsonData";

        ConvertJsonToEnemySO(jsonFolder + "/Enemy.json");
        ConvertJsonToTowerSO(jsonFolder + "/Tower.json");
        ConvertJsonToWaveSO(jsonFolder + "/Wave.json");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("JSON 변환 완료! EnemySO, TowerSO, WaveSO 업데이트 완료.");
    }

    // EnemySO 변환
    public static void ConvertJsonToEnemySO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"Enemy JSON 파일이 존재하지 않습니다: {jsonFilePath}");
            return;
        }

        string enemySOPath = "Assets/Resources/EnemySO";
        if (!Directory.Exists(enemySOPath)) Directory.CreateDirectory(enemySOPath);

        string jsonContent = File.ReadAllText(jsonFilePath);
        var jsonData = JObject.Parse(jsonContent);

        if (!IsValidDataType(jsonData, "EnemyData")) return;

        List<Dictionary<string, object>> enemyDataList = jsonData["Data"].ToObject<List<Dictionary<string, object>>>();

        foreach (var data in enemyDataList)
        {
            int enemyID = ConvertToInt(data["EnemyID"]);
            if (enemyID == 0)
            {
                Debug.LogError($"enemyID 변환 오류: {data["EnemyID"]}");
                continue;
            }

            string enemyName = data["EnemyName"].ToString();
            string assetPath = $"{enemySOPath}/{enemyName}.asset";

            EnemySO enemySO = LoadOrCreateAsset<EnemySO>(assetPath);
            enemySO.EnemyID = enemyID;
            enemySO.UnitName = enemyName;
            enemySO.Health = ConvertToInt(data["Health"]);
            enemySO.MovementSpeed = ConvertToFloat(data["Speed"]);
            enemySO.AttackPower = ConvertToInt(data["AttackPower"]);

            Debug.Log($"EnemySO 생성됨: {enemyName} (ID: {enemyID})");

            EditorUtility.SetDirty(enemySO);
        }
    }

    // TowerSO 변환
    public static void ConvertJsonToTowerSO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath)) return;

        string towerSOPath = "Assets/Resources/TowerSO";
        if (!Directory.Exists(towerSOPath)) Directory.CreateDirectory(towerSOPath);

        string jsonContent = File.ReadAllText(jsonFilePath);
        var jsonData = JObject.Parse(jsonContent);

        if (!IsValidDataType(jsonData, "TowerData")) return;

        List<Dictionary<string, object>> towerDataList = jsonData["Data"].ToObject<List<Dictionary<string, object>>>();

        foreach (var data in towerDataList)
        {
            string towerName = data["UnitName"].ToString();
            string assetPath = $"{towerSOPath}/{towerName}.asset";

            TowerSO towerSO = LoadOrCreateAsset<TowerSO>(assetPath);
            towerSO.UnitName = towerName;
            towerSO.AttackPower = ConvertToInt(data["AttackPower"]);
            towerSO.AttackSpeed = ConvertToFloat(data["AttackSpeed"]);

            EditorUtility.SetDirty(towerSO);
        }
    }

    // WaveSO 변환
    public static bool ConvertJsonToWaveSO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"Wave JSON 파일이 존재하지 않습니다: {jsonFilePath}");
            return false;
        }

        string waveSOPath = "Assets/Resources/WaveSO";
        if (!Directory.Exists(waveSOPath)) Directory.CreateDirectory(waveSOPath);

        string jsonContent = File.ReadAllText(jsonFilePath);
        var jsonData = JObject.Parse(jsonContent);

        if (!IsValidDataType(jsonData, "WaveData")) return false;

        Debug.Log($"로드된 JSON 데이터: {jsonContent}");

        JToken waveDataToken = jsonData["Data"];

        if (waveDataToken is JArray)
        {
            Debug.LogError("JSON 데이터가 배열 형식입니다. 변환을 수행합니다.");
            return ConvertJsonArrayToWaveSO(waveDataToken.ToObject<List<Dictionary<string, object>>>(), waveSOPath);
        }
        else if (waveDataToken is JObject)
        {
            // waveData를 Dictionary<int, Dictionary<int, int>>로 변환 후 전달
            var rawWaveData = waveDataToken.ToObject<Dictionary<string, Dictionary<string, int>>>();
            Dictionary<int, Dictionary<int, int>> convertedWaveData = ConvertWaveDataDictionary(rawWaveData);
            return ConvertJsonDictionaryToWaveSO(convertedWaveData, waveSOPath);
        }
        else
        {
            Debug.LogError("JSON 데이터의 구조가 예상과 다릅니다.");
            return false;
        }
    }





    // 숫자 변환 함수
    private static int ConvertToInt(object value)
    {
        if (value == null)
        {
            Debug.LogError("값이 null이므로 0 반환");
            return 0;
        }

        try
        {
            return Convert.ToInt32(value);
        }
        catch (Exception ex)
        {
            Debug.LogError($"숫자로 변환 실패: {value} (오류: {ex.Message})");
            return 0;
        }
    }
    private static float ConvertToFloat(object value) => value == null ? 0f : Convert.ToSingle(value);

    private static bool IsValidDataType(JObject jsonData, string expectedType)
    {
        return jsonData.ContainsKey("DataType") && jsonData["DataType"].ToString() == expectedType;
    }

    private static T LoadOrCreateAsset<T>(string assetPath) where T : ScriptableObject
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
        }
        return asset;
    }
    private static bool ConvertJsonArrayToWaveSO(List<Dictionary<string, object>> waveDataList, string waveSOPath)
    {
        // waveDataGrouped를 Dictionary<int, Dictionary<int, int>>로 선언
        Dictionary<int, Dictionary<int, int>> waveDataGrouped = new Dictionary<int, Dictionary<int, int>>();

        foreach (var waveEntry in waveDataList)
        {
            // waveEntry가 null인지 확인
            if (waveEntry == null)
            {
                Debug.LogError("WaveData 항목이 null입니다.");
                continue;
            }

            // JSON 키 변환 (waveCount → Wave, Count → EnemyCount)
            if (waveEntry.ContainsKey("waveCount") && !waveEntry.ContainsKey("Wave"))
            {
                waveEntry["Wave"] = waveEntry["waveCount"];
            }

            if (waveEntry.ContainsKey("Count") && !waveEntry.ContainsKey("EnemyCount"))
            {
                waveEntry["EnemyCount"] = waveEntry["Count"];
            }

            // 키 존재 여부 확인
            if (!waveEntry.ContainsKey("Wave") || !waveEntry.ContainsKey("EnemyID") || !waveEntry.ContainsKey("EnemyCount"))
            {
                Debug.LogError($"WaveData 항목이 잘못되었습니다: {JsonConvert.SerializeObject(waveEntry, Formatting.Indented)}");
                continue;
            }

            // 데이터를 올바른 타입으로 변환
            int waveCount = ConvertToInt(waveEntry["Wave"]);
            int enemyID = ConvertToInt(waveEntry["EnemyID"]);
            int enemyCount = ConvertToInt(waveEntry["EnemyCount"]);

            // 변환 실패 여부 확인
            if (waveCount == 0 || enemyID == 0 || enemyCount == 0)
            {
                Debug.LogError($"변환 실패: Wave={waveEntry["Wave"]}, EnemyID={waveEntry["EnemyID"]}, EnemyCount={waveEntry["EnemyCount"]}");
                continue;
            }

            // waveDataGrouped에 waveCount가 존재하는지 확인 후 초기화
            if (!waveDataGrouped.ContainsKey(waveCount))
            {
                waveDataGrouped[waveCount] = new Dictionary<int, int>();
            }

            // 적 데이터 추가 (중복된 EnemyID가 있으면 누적)
            if (!waveDataGrouped[waveCount].ContainsKey(enemyID))
            {
                waveDataGrouped[waveCount][enemyID] = 0;
            }

            waveDataGrouped[waveCount][enemyID] += enemyCount; // 누적 합산

            Debug.Log($"웨이브 {waveCount}: EnemyID {enemyID}, Count {enemyCount} 추가됨.");
        }

        // 변환된 Dictionary<int, Dictionary<int, int>>을 사용하여 SO 변환
        return ConvertJsonDictionaryToWaveSO(waveDataGrouped, waveSOPath);
    }

    private static bool ConvertJsonDictionaryToWaveSO(Dictionary<int, Dictionary<int, int>> waveData, string waveSOPath)
    {
        foreach (var waveEntry in waveData)
        {
            int waveCount = waveEntry.Key;

            string assetPath = $"{waveSOPath}/Wave_{waveCount}.asset";
            WaveSO waveSO = LoadOrCreateAsset<WaveSO>(assetPath);
            waveSO.waveCount = waveCount;
            waveSO.enemyCounts = new SerializableDictionary<int, int>();

            foreach (var kvp in waveEntry.Value)
            {
                waveSO.enemyCounts[kvp.Key] = kvp.Value;
            }

            Debug.Log($"WaveSO 생성됨: Wave {waveCount}");

            EditorUtility.SetDirty(waveSO);
>>>>>>> Stashed changes
        }
    }

<<<<<<< Updated upstream
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
=======

    private static Dictionary<int, Dictionary<int, int>> ConvertWaveDataDictionary(Dictionary<string, Dictionary<string, int>> rawWaveData)
    {
        Dictionary<int, Dictionary<int, int>> convertedWaveData = new Dictionary<int, Dictionary<int, int>>();

        foreach (var waveEntry in rawWaveData)
        {
            if (!int.TryParse(waveEntry.Key, out int waveCount))
            {
                Debug.LogError($"웨이브 키 변환 실패: {waveEntry.Key}");
                continue;
            }

            Dictionary<int, int> enemyCounts = new Dictionary<int, int>();

            foreach (var enemyEntry in waveEntry.Value)
            {
                if (!int.TryParse(enemyEntry.Key, out int enemyID))
                {
                    Debug.LogError($"EnemyID 변환 실패: {enemyEntry.Key}");
                    continue;
                }

                enemyCounts[enemyID] = enemyEntry.Value;
            }

            convertedWaveData[waveCount] = enemyCounts;
        }

        return convertedWaveData;
    }

>>>>>>> Stashed changes
}
