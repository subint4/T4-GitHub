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

    // ConvertToInt, ConvertToFloat 유틸 함수
    private static int ConvertToInt(object value)
    {
        if (value == null)
        {
            Debug.LogError("값이 null이므로 0 반환");
            return 0;
        }

        if (int.TryParse(value.ToString(), out int result))
        {
            return result;
        }
        else
        {
            Debug.LogError($"[ConvertToInt] 변환 실패: {value}");
            return 0;
        }
    }

    private static float ConvertToFloat(object value)
    {
        if (value == null) return 0f;

        if (float.TryParse(value.ToString(), out float result))
        {
            return result;
        }
        else
        {
            Debug.LogError($"[ConvertToFloat] 변환 실패: {value}");
            return 0f;
        }
    }


    // EnemySO 변환
    public static void ConvertJsonToEnemySO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath)) return;

        string enemySOPath = "Assets/Resources/EnemySO";
        EnsureDirectoryExists(enemySOPath);

        string jsonContent = File.ReadAllText(jsonFilePath);
        var jsonData = JObject.Parse(jsonContent);

        if (!IsValidDataType(jsonData, "EnemyData")) return;

        List<Dictionary<string, object>> enemyDataList = jsonData["Data"].ToObject<List<Dictionary<string, object>>>();

        foreach (var data in enemyDataList)
        {
            string enemyName = data["EnemyName"].ToString();
            string assetPath = $"{enemySOPath}/{enemyName}.asset";

            EnemySO enemySO = LoadOrCreateAsset<EnemySO>(assetPath);

            // 기존 데이터 업데이트
            enemySO.EnemyID = ConvertToInt(data["EnemyID"]);
            enemySO.UnitName = enemyName;
            enemySO.Health = ConvertToInt(data["Health"]);
            enemySO.MovementSpeed = ConvertToFloat(data["Speed"]);
            enemySO.AttackPower = ConvertToInt(data["AttackPower"]);

            Debug.Log($"[EnemySO] {enemyName} 업데이트됨 (ID: {enemySO.EnemyID})");

            EditorUtility.SetDirty(enemySO);
        }
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
        EnsureDirectoryExists(towerSOPath);

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

            TowerSO towerSO = LoadOrCreateAsset<TowerSO>(assetPath);

            // **디버깅용 로그 추가**
            Debug.Log($"[TowerSO] {towerName} 생성/업데이트 진행 중...");

            // **데이터 가져오기**
            towerSO.UnitName = towerName;
            towerSO.AttackPower = ConvertToInt(data["AttackPower"]);
            towerSO.AttackSpeed = ConvertToFloat(data["AttackSpeed"]);
            towerSO.Health = ConvertToInt(data["Health"]);
            towerSO.DeployCost = ConvertToInt(data["DeployCost"]);
            towerSO.UpgradeCost = ConvertToInt(data["UpgradeCost"]);

            // **TowerType 설정**
            if (data.ContainsKey("TowerType") && Enum.TryParse(data["TowerType"].ToString().Trim(), true, out TowerType parsedType))
            {
                towerSO.TowerType = parsedType;
            }
            else
            {
                towerSO.TowerType = TowerType.Default;
            }

            // **디버깅용 로그 추가 (확인용)**
            Debug.Log($"[TowerSO] {towerName} 업데이트 완료: " +
                      $"공격력={towerSO.AttackPower}, 속도={towerSO.AttackSpeed}, 체력={towerSO.Health}, " +
                      $"배치 비용={towerSO.DeployCost}, 업그레이드 비용={towerSO.UpgradeCost}, 타입={towerSO.TowerType}");

            EditorUtility.SetDirty(towerSO);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    // WaveSO 변환
    public static void ConvertJsonToWaveSO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath)) return;

        string waveSOPath = "Assets/Resources/WaveSO";
        EnsureDirectoryExists(waveSOPath);

        string jsonContent = File.ReadAllText(jsonFilePath);
        var jsonData = JObject.Parse(jsonContent);

        if (!IsValidDataType(jsonData, "WaveData")) return;

        JToken waveDataToken = jsonData["Data"];

        if (waveDataToken is JArray waveArray)
        {
            ConvertJsonArrayToWaveSO(waveArray.ToObject<List<Dictionary<string, object>>>(), waveSOPath);
        }
        else if (waveDataToken is JObject waveObject)
        {
            var rawWaveData = waveObject.ToObject<Dictionary<string, Dictionary<string, int>>>();
            ConvertJsonDictionaryToWaveSO(ConvertWaveDataDictionary(rawWaveData), waveSOPath);
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
                waveSO.enemyCounts[kvp.Key] = kvp.Value;
            }

            Debug.Log($"[WaveSO] Wave {waveCount} 업데이트됨");
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
        EnsureDirectoryExists(Path.GetDirectoryName(assetPath));

        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            Debug.Log($"[SO 생성] {assetPath}");
        }
        else
        {
            Debug.Log($"[SO 로드] {assetPath}");
        }

        return asset;
    }


    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }
}
