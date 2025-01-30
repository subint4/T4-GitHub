using System;
using System.Data;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using System.Collections.Generic;

public class ExcelToJson : EditorWindow
{
    public enum DataType { EnemyData, UnitData, WaveData }
    private DataType selectedDataType;


    private void OnGUI()
    {
        GUILayout.Label("Excel to JSON 변환기", EditorStyles.boldLabel);

        selectedDataType = (DataType)EditorGUILayout.EnumPopup("데이터 타입 선택", selectedDataType);

        if (GUILayout.Button("엑셀 파일 선택 및 변환"))
        {
            ConvertExcelToJson(selectedDataType);
        }
    }

    public static void ConvertExcelToJson(DataType dataType)
    {
        string excelFilePath = EditorUtility.OpenFilePanel("엑셀 파일 선택", "", "xlsx,csv");

        if (string.IsNullOrEmpty(excelFilePath))
        {
            Debug.LogWarning("파일 선택이 취소되었습니다.");
            return;
        }

        Debug.Log($"선택된 엑셀 파일: {excelFilePath}");

        DataTable excelData = ExcelLoader.LoadExcel(excelFilePath);
        if (excelData == null || excelData.Rows.Count == 0)
        {
            Debug.LogError("Error: 엑셀 데이터가 비어 있습니다.");
            return;
        }

        switch (dataType)
        {
            case DataType.EnemyData:
                ConvertToEnemyJson(excelData);
                break;
            case DataType.UnitData:
                ConvertToUnitJson(excelData);
                break;
            case DataType.WaveData:
                ConvertToWaveJson(excelData);
                break;
        }
    }

    private static void ConvertToEnemyJson(DataTable excelData)
    {
        var jsonList = new List<EnemyData>();

        foreach (DataRow row in excelData.Rows)
        {
            try
            {
                jsonList.Add(new EnemyData
                {
                    key = Convert.ToInt32(row["key"]),
                    UnitName = row["UnitName"].ToString(),
                    Health = Convert.ToInt32(row["Health"]),
                    AttackPower = Convert.ToInt32(row["AttackPower"]),
                    AttackSpeed = Convert.ToSingle(row["AttackSpeed"]),
                    RewardMoney = Convert.ToInt32(row["RewardMoney"]),
                    MovementSpeed = Convert.ToSingle(row["MovementSpeed"]),
                    EnemyType = row["EnemyType"].ToString(),
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: 적 데이터 변환 중 오류 발생! ({ex.Message})");
                continue;
            }
        }

        SaveJson(new EnemyConfig { Enemies = jsonList }, "enemy_data.json");
    }

    private static void ConvertToUnitJson(DataTable excelData)
    {
        var jsonList = new List<UnitData>();

        foreach (DataRow row in excelData.Rows)
        {
            try
            {
                jsonList.Add(new UnitData
                {
                    key = Convert.ToInt32(row["key"]),
                    UnitName = row["UnitName"].ToString(),
                    Health = Convert.ToInt32(row["Health"]),
                    AttackPower = Convert.ToInt32(row["AttackPower"]),
                    AttackSpeed = Convert.ToSingle(row["AttackSpeed"]),
                    DeployCost = Convert.ToInt32(row["DeployCost"])
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: 유닛 데이터 변환 중 오류 발생! ({ex.Message})");
                continue;
            }
        }

        SaveJson(new UnitConfig { Units = jsonList }, "unit_data.json");
    }

    private static void ConvertToWaveJson(DataTable excelData)
    {
        var jsonList = new List<WaveStageData>();

        foreach (DataRow row in excelData.Rows)
        {
            try
            {
                jsonList.Add(new WaveStageData
                {
                    key = Convert.ToInt32(row["key"]),
                    EnemyType = row["EnemyType"].ToString(),
                    SpawnCount = Convert.ToInt32(row["SpawnCount"]),
                    SpawnRate = Convert.ToSingle(row["SpawnRate"]),
                    EnemyPrefab = row["EnemyPrefab"].ToString()
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: 웨이브 데이터 변환 중 오류 발생! ({ex.Message})");
                continue;
            }
        }

        SaveJson(new WaveStageConfig { WaveStages = jsonList }, "wave_data.json");
    }

    private static void SaveJson(object data, string fileName)
    {
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        string outputJsonPath = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(outputJsonPath, jsonData);

        Debug.Log($"JSON 변환 완료: {outputJsonPath}");
        EditorUtility.RevealInFinder(outputJsonPath);
    }
}
