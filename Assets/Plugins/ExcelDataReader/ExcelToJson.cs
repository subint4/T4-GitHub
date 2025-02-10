using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using ExcelDataReader;

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

        string outputJsonPath = Path.Combine(outputFolder, $"{dataType}.json");

        try
        {
            using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    Debug.Log($"엑셀 파일 로드 완료, 총 시트 개수: {result.Tables.Count}");

                    foreach (System.Data.DataTable table in result.Tables)
                    {
                        string sheetName = table.TableName.Trim();
                        Debug.Log($"확인 중인 시트: '{sheetName}', 선택된 데이터 타입: {dataType}");

                        if (table.Rows.Count < 3)
                        {
                            Debug.LogError($"{sheetName} 시트에 데이터가 부족합니다. 최소 3행(헤더 + 데이터 형식 + 값)이 필요합니다.");
                            continue;
                        }

                        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

                        // 첫 번째 행 → 컬럼명 저장
                        List<string> columnNames = new List<string>();
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            string columnName = table.Rows[0][j]?.ToString()?.Trim();
                            if (string.IsNullOrEmpty(columnName))
                            {
                                Debug.LogError($"컬럼명이 비어 있습니다! {j}번째 열을 건너뜁니다.");
                                continue;
                            }
                            columnNames.Add(columnName);
                        }

                        // 두 번째 행 → 데이터 타입 변환
                        Dictionary<string, string> metadata = new Dictionary<string, string>();
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            object typeSample = table.Rows[2][j]; // 첫 번째 실제 데이터 행 기준으로 타입 결정
                            string typeString = DetectDataType(typeSample);
                            metadata[columnNames[j]] = typeString;
                        }

                        // 세 번째 행부터 데이터 저장
                        for (int i = 2; i < table.Rows.Count; i++)
                        {
                            Dictionary<string, object> rowDict = new Dictionary<string, object>();

                            for (int j = 0; j < columnNames.Count; j++)
                            {
                                object cellValue = table.Rows[i][j];

                                if (cellValue == null)
                                    continue;

                                // 정수와 실수를 구별하여 저장
                                rowDict[columnNames[j]] = ConvertToCorrectType(cellValue, metadata[columnNames[j]]);
                            }

                            if (rowDict.Count > 0)
                            {
                                dataList.Add(rowDict);
                            }
                        }

                        // 올바른 JSON 구조로 저장 (DataType을 맨 위로 배치)
                        var jsonOutput = new
                        {
                            DataType = dataType.ToString(),
                            Metadata = metadata,
                            Data = dataList
                        };

                        string jsonString = JsonConvert.SerializeObject(jsonOutput, Formatting.Indented);
                        File.WriteAllText(outputJsonPath, jsonString);

                        Debug.Log($"{sheetName} 변환 완료! JSON 저장 위치: {outputJsonPath}");
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

    // 데이터 타입을 자동 감지하는 함수
    private static string DetectDataType(object value)
    {
        if (value == null) return "string";

        if (double.TryParse(value.ToString(), out double num))
        {
            if (num == Math.Floor(num)) // 소수점이 없으면 정수
                return "int";
            return "float";
        }
        if (bool.TryParse(value.ToString(), out _)) return "bool";

        return "string"; // 기본값
    }

    // 정수와 실수를 정확하게 변환하는 함수
    private static object ConvertToCorrectType(object value, string expectedType)
    {
        if (value == null) return null;

        if (expectedType == "int" && double.TryParse(value.ToString(), out double num))
        {
            return Convert.ToInt32(num); // 강제 `int` 변환
        }
        else if (expectedType == "float" && double.TryParse(value.ToString(), out double floatNum))
        {
            return floatNum; // 실수 변환 유지
        }
        return value;
    }
}
