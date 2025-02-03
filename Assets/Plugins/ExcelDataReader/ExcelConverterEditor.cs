using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ExcelConverterEditor : EditorWindow
{
    public enum DataType { EnemyData, UnitData, WaveData } // 하나의 공통 열거형 사용

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
                string outputJsonPath = ExcelToJson.ConvertExcelToJson(excelFilePath, selectedDataType);
                if (!string.IsNullOrEmpty(outputJsonPath))
                {
                    Debug.Log($"Excel 변환 완료: {outputJsonPath}");
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
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
            if (!string.IsNullOrEmpty(jsonFilePath))
            {
                if (selectedDataType == DataType.WaveData)
                {
                    Debug.Log($"선택한 WaveData JSON 파일 경로: {jsonFilePath}");
                    Debug.Log($"WaveData JSON 내용:\n{File.ReadAllText(jsonFilePath)}");
                }
                else
                {
                    JsonToSO.UpdateSOFromJson(jsonFilePath, selectedDataType);
                }
            }
            else
            {
                Debug.LogWarning("JSON 파일이 선택되지 않았습니다.");
            }
        }
    }
}
