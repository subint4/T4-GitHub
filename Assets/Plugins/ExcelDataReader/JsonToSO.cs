using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;

public static class JsonToSO
{
    public static void ConvertJsonToSO()
    {
        Debug.Log("JSON 변환 시작");

        string jsonFolder = "Assets/Resources/JsonData";

        ConvertJsonToEnemySO(jsonFolder + "/Enemy.json");
        ConvertJsonToTowerSO(jsonFolder + "/Tower.json");
        ConvertJsonToWaveSO(jsonFolder + "/Wave.json");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("JSON 변환 완료! EnemySO, TowerSO, WaveSO 업데이트 완료.");
    }


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

    private static float ConvertToFloat(object value)
    {
        if (value == null) return 0f;
        try
        {
            return Convert.ToSingle(value);
        }
        catch
        {
            return 0f;
        }
    }

    // EnemySO 변환
    public static void ConvertJsonToEnemySO(string jsonFilePath)
    {
        Debug.Log($"Enemy JSON 변환 시작: {jsonFilePath}");

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"JSON 파일 없음: {jsonFilePath}");
            return;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        Debug.Log($"JSON 데이터 로드됨: {jsonFilePath}\n{jsonContent}");

        var jsonData = JObject.Parse(jsonContent);

        if (!IsValidDataType(jsonData, "EnemyData"))
        {
            Debug.LogError($"JSON 형식 오류: {jsonFilePath}");
            return;
        }

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
            string assetPath = $"Assets/Resources/EnemySO/{enemyName}.asset";

            EnemySO enemySO = LoadOrCreateAsset<EnemySO>(assetPath);
            enemySO.EnemyID = enemyID;
            enemySO.UnitName = enemyName;
            enemySO.Health = ConvertToInt(data["Health"]);
            enemySO.MovementSpeed = ConvertToFloat(data["Speed"]);
            enemySO.AttackPower = ConvertToInt(data["AttackPower"]);

            Debug.Log($"EnemySO 생성/업데이트 완료: {enemyName}");

            EditorUtility.SetDirty(enemySO);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // TowerSO 변환
    public static void ConvertJsonToTowerSO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"Tower JSON 파일이 존재하지 않습니다: {jsonFilePath}");
            return;
        }

        string towerSOPath = "Assets/Resources/TowerSO";
        if (!Directory.Exists(towerSOPath)) Directory.CreateDirectory(towerSOPath);

        string jsonContent = File.ReadAllText(jsonFilePath);
        var jsonData = JObject.Parse(jsonContent);

        if (!IsValidDataType(jsonData, "TowerData"))
        {
            Debug.LogError("잘못된 JSON 타입입니다!");
            return;
        }

        List<Dictionary<string, object>> towerDataList = jsonData["Data"].ToObject<List<Dictionary<string, object>>>();

        foreach (var data in towerDataList)
        {
            string towerName = data["UnitName"].ToString();
            string assetPath = $"{towerSOPath}/{towerName}.asset";

            TowerSO towerSO = AssetDatabase.LoadAssetAtPath<TowerSO>(assetPath);

            if (towerSO == null)
            {
                towerSO = ScriptableObject.CreateInstance<TowerSO>();
                AssetDatabase.CreateAsset(towerSO, assetPath);
                Debug.Log($"새로운 TowerSO 생성: {towerName}");  // **생성이 실행되는지 확인**
            }
            else
            {
                Debug.Log($"기존 TowerSO 로드: {towerName}");  // **기존 SO가 로드되는지 확인**
            }

            towerSO.UnitName = towerName;
            towerSO.AttackPower = ConvertToInt(data["AttackPower"]);
            towerSO.AttackSpeed = ConvertToFloat(data["AttackSpeed"]);

            if (data.ContainsKey("TowerType") && Enum.TryParse(data["TowerType"].ToString().Trim(), true, out TowerType parsedType))
            {
                towerSO.TowerType = parsedType;
                Debug.Log($"{towerName}의 TowerType 설정됨: {parsedType}");  // **타워 타입이 올바르게 설정되는지 확인**
            }
            else
            {
                towerSO.TowerType = TowerType.Default;
                Debug.LogError($"{towerName}: TowerType 변환 실패! 기본값(Default)으로 설정됨.");
            }

            EditorUtility.SetDirty(towerSO);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }





    // WaveSO 변환 추가
    public static void ConvertJsonToWaveSO(string jsonFilePath)
    {
        Debug.Log($"Wave JSON 변환 시작: {jsonFilePath}");

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"JSON 파일 없음: {jsonFilePath}");
            return;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        Debug.Log($"JSON 데이터 로드됨: {jsonFilePath}\n{jsonContent}");

        var jsonData = JObject.Parse(jsonContent);

        if (!IsValidDataType(jsonData, "WaveData"))
        {
            Debug.LogError($"JSON 형식 오류: {jsonFilePath}");
            return;
        }

        JToken waveDataToken = jsonData["Data"];

        if (waveDataToken is JArray waveArray)
        {
            ConvertJsonArrayToWaveSO(waveArray.ToObject<List<Dictionary<string, object>>>(), "Assets/Resources/WaveSO");
        }
        else if (waveDataToken is JObject waveObject)
        {
            var rawWaveData = waveObject.ToObject<Dictionary<string, Dictionary<string, int>>>();
            Dictionary<int, Dictionary<int, int>> convertedWaveData = ConvertWaveDataDictionary(rawWaveData);
            ConvertJsonDictionaryToWaveSO(convertedWaveData, "Assets/Resources/WaveSO");
        }
    }

    private static void ConvertJsonArrayToWaveSO(List<Dictionary<string, object>> waveDataList, string waveSOPath)
    {
        Dictionary<int, Dictionary<int, int>> waveDataGrouped = new Dictionary<int, Dictionary<int, int>>();

        foreach (var waveEntry in waveDataList)
        {
            int waveCount = ConvertToInt(waveEntry["Wave"]);
            int enemyID = ConvertToInt(waveEntry["EnemyID"]);
            int enemyCount = ConvertToInt(waveEntry["EnemyCount"]);

            if (!waveDataGrouped.ContainsKey(waveCount))
            {
                waveDataGrouped[waveCount] = new Dictionary<int, int>();
            }

            waveDataGrouped[waveCount][enemyID] = enemyCount;
        }

        ConvertJsonDictionaryToWaveSO(waveDataGrouped, waveSOPath);
    }

    private static void ConvertJsonDictionaryToWaveSO(Dictionary<int, Dictionary<int, int>> waveData, string waveSOPath)
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
                waveSO.enemyCounts.Add(kvp.Key, kvp.Value);
            }

            EditorUtility.SetDirty(waveSO);
        }
    }

    private static Dictionary<int, Dictionary<int, int>> ConvertWaveDataDictionary(Dictionary<string, Dictionary<string, int>> rawWaveData)
    {
        Dictionary<int, Dictionary<int, int>> convertedWaveData = new Dictionary<int, Dictionary<int, int>>();

        foreach (var waveEntry in rawWaveData)
        {
            if (!int.TryParse(waveEntry.Key, out int waveCount)) continue;

            Dictionary<int, int> enemyCounts = new Dictionary<int, int>();

            foreach (var enemyEntry in waveEntry.Value)
            {
                if (!int.TryParse(enemyEntry.Key, out int enemyID)) continue;
                enemyCounts[enemyID] = enemyEntry.Value;
            }

            convertedWaveData[waveCount] = enemyCounts;
        }

        return convertedWaveData;
    }

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
            Debug.Log($"SO 생성됨: {assetPath}");
        }
        else
        {
            Debug.Log($"기존 SO 불러옴: {assetPath}");
        }
        return asset;
    }

}
