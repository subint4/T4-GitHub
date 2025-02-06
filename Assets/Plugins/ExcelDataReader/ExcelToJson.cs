using ExcelDataReader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public static class ExcelToJson
{
    public static string ConvertExcelToJson(string excelFilePath, ExcelConverterEditor.DataType dataType)
    {
        if (!File.Exists(excelFilePath))
        {
            Debug.LogError($"Excel 파일이 존재하지 않습니다: {excelFilePath}");
            return null;
        }
        Debug.Log($"Excel 파일 로드 시도: {excelFilePath}");

        string outputFolder = "Assets/Resources/JsonData";
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        string outputJsonPath = null;

        try
        {
            Debug.Log($"Excel 파일 분석 중: {excelFilePath}");

            using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    Debug.Log($"액셀 파일 로드 완료, 총 시트 개수: {result.Tables.Count}");


                    foreach (System.Data.DataTable table in result.Tables)
                    {
                        string sheetName = table.TableName.Trim();
                        Debug.Log($"확인 중인 시트: '{sheetName}', 선택된 데이터 타입: {dataType}");

                        if (!IsMatchingSheet(sheetName, dataType))
                        {
                            Debug.LogWarning($"{sheetName} 시트는 {dataType}과 일치하지 않아 건너뜀.");
                            continue;
                        }

                        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

                        for (int i = 1; i < table.Rows.Count; i++) // 첫 번째 행(헤더) 제외
                        {
                            Dictionary<string, object> rowDict = new Dictionary<string, object>();

                            for (int j = 0; j < table.Columns.Count; j++)
                            {
                                string columnName = table.Rows[0][j]?.ToString();
                                object cellValue = table.Rows[i][j];

                                if (columnName == null)
                                {
                                    Debug.LogError($"컬럼명이 null입니다! {i}번째 행, {j}번째 열");
                                    continue;
                                }

                                if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
                                    continue;

                                rowDict[columnName] = cellValue;
                            }

                            if (rowDict.Count > 0)
                            {
                                dataList.Add(rowDict);
                            }
                        }

                        string jsonOutput = JsonConvert.SerializeObject(dataList, Formatting.Indented);
                        outputJsonPath = Path.Combine(outputFolder, $"{sheetName}.json");
                        File.WriteAllText(outputJsonPath, jsonOutput);

                        Debug.Log($"{sheetName} 시트 변환 완료! JSON 저장 위치: {outputJsonPath}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excel 변환 중 오류 발생: {ex.Message}");
            return null;
        }

        return outputJsonPath;
    }

    private static bool IsMatchingSheet(string sheetName, ExcelConverterEditor.DataType dataType)
    {
        switch (dataType)
        {
            case ExcelConverterEditor.DataType.EnemyData:
                return sheetName.Equals("EnemyData", StringComparison.OrdinalIgnoreCase);
            case ExcelConverterEditor.DataType.TowerData:
                return sheetName.Equals("TowerData", StringComparison.OrdinalIgnoreCase);
            case ExcelConverterEditor.DataType.WaveData:
                return sheetName.Equals("WaveData", StringComparison.OrdinalIgnoreCase);
            case ExcelConverterEditor.DataType.ProjectileData: // ProjectileData 처리 추가
                return sheetName.Equals("ProjectileData", StringComparison.OrdinalIgnoreCase);

            default:
                return false;
        }
    }
}
