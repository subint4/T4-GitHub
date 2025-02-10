using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ExcelConverterEditor : EditorWindow
{
    public enum DataType { EnemyData, TowerData, WaveData, ProjectileData }

    private DataType selectedDataType;
    private string jsonFilePath = "";
    private string excelFilePath = "";

    [MenuItem("Tools/Data Converter")]
    public static void ShowWindow()
    {
        GetWindow<ExcelConverterEditor>("Data Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Excel ↔ JSON ↔ ScriptableObject 변환기", EditorStyles.boldLabel);

        // 데이터 유형 선택 드롭다운 메뉴
        selectedDataType = (DataType)EditorGUILayout.EnumPopup("데이터 타입 선택", selectedDataType);

        GUILayout.Space(10);
        GUILayout.Label("Excel → JSON 변환", EditorStyles.boldLabel);

        // Excel 파일 선택 버튼
        if (GUILayout.Button("Excel 파일 선택 및 JSON 변환"))
        {
            excelFilePath = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
            if (!string.IsNullOrEmpty(excelFilePath))
            {
                string outputJsonPath = Path.Combine("Assets/Resources/JsonData", $"{selectedDataType}.json");

                ExcelToJson.ConvertExcelToJson(excelFilePath, selectedDataType);

                if (File.Exists(outputJsonPath))
                {
                    Debug.Log($"Excel 변환 완료: {outputJsonPath}");
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("Excel 변환 실패");
                }
            }
            else
            {
                Debug.LogWarning("Excel 파일이 선택되지 않았습니다.");
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("JSON → ScriptableObject 변환", EditorStyles.boldLabel);

        // JSON 파일 선택 버튼
        if (GUILayout.Button("JSON 파일 선택 및 SO 변환"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "Assets/Resources/JsonData", "json");
            if (!string.IsNullOrEmpty(jsonFilePath))
            {
                switch (selectedDataType)
                {
                    case DataType.EnemyData:
                        JsonToSO.ConvertJsonToEnemySO(jsonFilePath);
                        Debug.Log($"EnemyData JSON 변환 완료: {jsonFilePath}");
                        break;

                    case DataType.TowerData:
                        JsonToSO.ConvertJsonToTowerSO(jsonFilePath);
                        Debug.Log($"TowerData JSON 변환 완료: {jsonFilePath}");
                        break;

                    case DataType.WaveData:
                        JsonToSO.ConvertJsonToWaveSO(jsonFilePath);
                        Debug.Log($"WaveData JSON 변환 완료: {jsonFilePath}");
                        break;

                    default:
                        Debug.LogError("지원되지 않는 데이터 유형입니다.");
                        break;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("JSON 파일이 선택되지 않았습니다.");
            }
        }
    }
}
