using UnityEditor;
using UnityEngine;
using System.IO;

public class ExcelConverterEditor : EditorWindow
{
    private string excelFilePath = "";
    private string jsonOutputPath = "Assets/Resources/JsonData";

    [MenuItem("Tools/Excel To JSON Converter")]
    public static void ShowWindow()
    {
        GetWindow<ExcelConverterEditor>("Excel To JSON Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Excel → JSON 변환기", EditorStyles.boldLabel);

        if (GUILayout.Button("Excel 파일 선택"))
        {
            excelFilePath = EditorUtility.OpenFilePanel("Excel 파일 선택", "", "xlsx");
        }

        EditorGUILayout.TextField("Excel 파일 경로", excelFilePath);

        if (GUILayout.Button("Excel → JSON 변환"))
        {
            if (ValidateExcelPath())
            {
                string jsonPath = ExcelToJson.ConvertExcelToJson(excelFilePath, jsonOutputPath);
                if (!string.IsNullOrEmpty(jsonPath))
                {
                    Debug.Log($"Excel 변환 완료! JSON 저장 경로: {jsonPath}");
                }
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("JSON → SO 변환기", EditorStyles.boldLabel);

        if (GUILayout.Button("JSON 파일 선택"))
        {
            jsonOutputPath = EditorUtility.OpenFilePanel("JSON 파일 선택", "", "json");
        }

        EditorGUILayout.TextField("JSON 파일 경로", jsonOutputPath);

        if (GUILayout.Button("적 데이터 변환"))
        {
            if (ValidateJsonPath())
            {
                JsonToSO.ConvertJsonToEnemySO(jsonOutputPath);
                Debug.Log("적 데이터 변환 완료!");
            }
        }

        if (GUILayout.Button("타워 데이터 변환"))
        {
            if (ValidateJsonPath())
            {
                JsonToSO.ConvertJsonToTowerSO(jsonOutputPath);
                Debug.Log("타워 데이터 변환 완료!");
            }
        }

        if (GUILayout.Button("웨이브 데이터 변환"))
        {
            if (ValidateJsonPath())
            {
                JsonToSO.ConvertJsonToWaveSO(jsonOutputPath);
                Debug.Log("웨이브 데이터 변환 완료!");
            }
        }

        if (GUILayout.Button("모든 데이터 변환"))
        {
            if (ValidateJsonPath())
            {
                JsonToSO.ConvertJsonToEnemySO(jsonOutputPath);
                JsonToSO.ConvertJsonToTowerSO(jsonOutputPath);
                JsonToSO.ConvertJsonToWaveSO(jsonOutputPath);
                Debug.Log("모든 데이터 변환 완료!");
            }
        }
    }

    private bool ValidateExcelPath()
    {
        if (string.IsNullOrEmpty(excelFilePath))
        {
            Debug.LogError("Excel 파일 경로가 비어 있습니다.");
            return false;
        }

        if (!File.Exists(excelFilePath))
        {
            Debug.LogError($"Excel 파일을 찾을 수 없습니다: {excelFilePath}");
            return false;
        }

        return true;
    }

    private bool ValidateJsonPath()
    {
        if (string.IsNullOrEmpty(jsonOutputPath))
        {
            Debug.LogError("JSON 파일 경로가 비어 있습니다.");
            return false;
        }

        if (!File.Exists(jsonOutputPath))
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {jsonOutputPath}");
            return false;
        }

        return true;
    }
}
