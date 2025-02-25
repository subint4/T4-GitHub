using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using ExcelDataReader;

public static class ExcelToJson
{
    private static readonly string OutputFolder = "Assets/Resources/JsonData"; // 고정된 JSON 저장 경로

    public static string ConvertExcelToJson(string excelFilePath, string outputFolder = "Assets/Resources/JsonData")
    {
        if (!File.Exists(excelFilePath))
        {
            Debug.LogError($"Excel 파일이 존재하지 않습니다: {excelFilePath}");
            return null;
        }

        Debug.Log($"Excel 파일 로드 시도: {excelFilePath}");

        if (!Directory.Exists(OutputFolder))
        {
            Directory.CreateDirectory(OutputFolder);
        }

        string jsonFilePath = Path.Combine(OutputFolder, Path.GetFileNameWithoutExtension(excelFilePath) + ".json");

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
                        Debug.Log($"확인 중인 시트: '{sheetName}'");

                        if (table.Rows.Count < 3)
                        {
                            Debug.LogError($"{sheetName} 시트에 데이터가 부족합니다. 최소 3행(헤더 + 데이터 형식 + 값)이 필요합니다.");
                            continue;
                        }

                        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

                        // 1. 첫 번째 행 → 컬럼명 저장
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

                        // 2. 두 번째 행 → 데이터 타입 변환 (2행을 기준으로 설정)
                        Dictionary<string, string> metadata = new Dictionary<string, string>();
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            string typeString = table.Rows[1][j]?.ToString()?.Trim().ToLower();
                            if (string.IsNullOrEmpty(typeString) || !IsValidType(typeString))
                            {
                                Debug.LogWarning($"[{columnNames[j]}] 잘못된 데이터 타입({typeString}) → 기본값 'string' 적용");
                                typeString = "string";
                            }
                            metadata[columnNames[j]] = typeString;
                        }

                        // 3. 세 번째 행부터 데이터 저장
                        for (int i = 2; i < table.Rows.Count; i++)
                        {
                            Dictionary<string, object> rowDict = new Dictionary<string, object>();

                            for (int j = 0; j < columnNames.Count; j++)
                            {
                                object cellValue = table.Rows[i][j];

                                if (cellValue == null)
                                    continue;

                                rowDict[columnNames[j]] = ConvertToCorrectType(cellValue, metadata[columnNames[j]]);
                            }

                            if (rowDict.Count > 0)
                            {
                                dataList.Add(rowDict);
                            }
                        }

                        //  JsonToSO에서 사용할 형식으로 JSON 변환
                        var jsonOutput = new JsonWrapper
                        {
                            DataType = sheetName,
                            Metadata = metadata,
                            Data = dataList
                        };

                        string jsonString = JsonConvert.SerializeObject(jsonOutput, Formatting.Indented);
                        File.WriteAllText(jsonFilePath, jsonString);

                        Debug.Log($"{sheetName} 변환 완료! JSON 저장 위치: {jsonFilePath}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excel 변환 중 오류 발생: {ex.Message}");
            return null;
        }

        return jsonFilePath;
    }

    private static bool IsValidType(string type)
    {
        return type == "int" || type == "float" || type == "string" || type == "bool" || type == "enum" ||
               type == "array-int" || type == "array-float" || type == "array-string";
    }

    private static object ConvertToCorrectType(object value, string expectedType)
    {
        if (value == null) return null;

        string stringValue = value.ToString().Trim();

        if (expectedType == "int" && int.TryParse(stringValue, out int intValue))
        {
            return intValue;
        }
        else if (expectedType == "float" && float.TryParse(stringValue, out float floatValue))
        {
            return floatValue;
        }
        else if (expectedType == "bool" && bool.TryParse(stringValue, out bool boolValue))
        {
            return boolValue;
        }
        else if (expectedType == "enum")
        {
            return stringValue;
        }
        else if (expectedType.StartsWith("array-"))
        {
            string[] splitValues = stringValue.Split(',');
            List<object> list = new List<object>();

            foreach (var val in splitValues)
            {
                string trimmedVal = val.Trim();

                if (expectedType == "array-int" && int.TryParse(trimmedVal, out int intElement))
                {
                    list.Add(intElement);
                }
                else if (expectedType == "array-float" && float.TryParse(trimmedVal, out float floatElement))
                {
                    list.Add(floatElement);
                }
                else if (expectedType == "array-string")
                {
                    list.Add(trimmedVal);
                }
            }

            return list;
        }

        return stringValue;
    }

    // JSON 데이터 포맷 유지
    [Serializable]
    public class JsonWrapper
    {
        public string DataType;
        public Dictionary<string, string> Metadata;
        public List<Dictionary<string, object>> Data;
    }
}
