using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class JsonToSO
{
    public static void UpdateSOFromJson(string filePath, ExcelConverterEditor.DataType dataType)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {filePath}");
            return;
        }

        string jsonText = File.ReadAllText(filePath);
        Debug.Log($"JSON 파일 로드 완료: {filePath}");

        switch (dataType)
        {
            case ExcelConverterEditor.DataType.EnemyData:
                List<EnemyData> enemyDataList = JsonUtility.FromJson<EnemyConfig>("{\"Enemies\":" + jsonText + "}").Enemies;
                UpdateEnemyDataSO(enemyDataList);
                break;

            case ExcelConverterEditor.DataType.UnitData:
                List<UnitData> unitDataList = JsonUtility.FromJson<UnitConfig>("{\"Units\":" + jsonText + "}").Units;
                UpdateTowerDataSO(unitDataList);
                break;

            case ExcelConverterEditor.DataType.WaveData:
                Debug.Log($"WaveData JSON 출력:\n{jsonText}");
                break;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void UpdateEnemyDataSO(List<EnemyData> enemyDataList)
    {
        string path = "Assets/Resources/EnemyData/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach (EnemyData enemy in enemyDataList)
        {
            string sanitizedUnitName = enemy.UnitName.Replace(" ", "_"); // 공백을 '_'로 변환
            string assetPath = $"{path}{sanitizedUnitName}.asset";
            EnemySO existingSO = AssetDatabase.LoadAssetAtPath<EnemySO>(assetPath);

            if (existingSO == null)
            {
                // 기존 SO가 없으면 새로 생성
                existingSO = ScriptableObject.CreateInstance<EnemySO>();
                existingSO.LoadFromEnemyData(enemy);
                AssetDatabase.CreateAsset(existingSO, assetPath);
                Debug.Log($"새로운 EnemySO 생성 완료: {assetPath}");
            }
            else
            {
                // 기존 SO 업데이트
                existingSO.LoadFromEnemyData(enemy);
                Debug.Log($"기존 EnemySO 업데이트 완료: {assetPath}");
            }

            EditorUtility.SetDirty(existingSO);
        }
    }

    private static void UpdateTowerDataSO(List<UnitData> unitDataList)
    {
        string path = "Assets/Resources/TowerData/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach (UnitData unit in unitDataList)
        {
            string sanitizedUnitName = unit.UnitName.Replace(" ", "_"); // 공백을 '_'로 변환
            string assetPath = $"{path}{sanitizedUnitName}.asset";
            TowerSO existingSO = AssetDatabase.LoadAssetAtPath<TowerSO>(assetPath);

            if (existingSO == null)
            {
                // 기존 SO가 없으면 새로 생성
                existingSO = ScriptableObject.CreateInstance<TowerSO>();
                existingSO.LoadFromUnitData(unit);
                AssetDatabase.CreateAsset(existingSO, assetPath);
                Debug.Log($"새로운 TowerSO 생성 완료: {assetPath}");
            }
            else
            {
                // 기존 SO 업데이트
                existingSO.LoadFromUnitData(unit);
                Debug.Log($"기존 TowerSO 업데이트 완료: {assetPath}");
            }

            EditorUtility.SetDirty(existingSO);
        }
    }
}
