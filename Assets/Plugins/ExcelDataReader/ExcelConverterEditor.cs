using UnityEditor;
using UnityEngine;

public class ExcelConverterEditor : EditorWindow
{
    private enum DataType { EnemyData, UnitData, WaveData }
    private DataType selectedDataType; // 사용자가 선택할 데이터 유형

    [MenuItem("Tools/Excel to JSON Converter")]
    public static void ShowWindow()
    {
        GetWindow<ExcelConverterEditor>("Excel to JSON Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Excel to JSON 변환기", EditorStyles.boldLabel);

        // 데이터 유형 선택 드롭다운 메뉴
        selectedDataType = (DataType)EditorGUILayout.EnumPopup("데이터 타입 선택", selectedDataType);

        // 변환 버튼
        if (GUILayout.Button("엑셀 파일 선택 및 JSON 변환"))
        {
            ExcelToJson.ConvertExcelToJson((ExcelToJson.DataType)selectedDataType);
        }
    }
}
