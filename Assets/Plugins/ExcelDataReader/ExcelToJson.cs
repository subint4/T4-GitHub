using System;
using System.Data;
using System.IO;
using UnityEngine;
using ExcelDataReader;

public static class ExcelToJson
{
    public static string ConvertExcelToJson(string filePath, ExcelConverterEditor.DataType dataType)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Excel 파일을 찾을 수 없습니다: {filePath}");
            return null;
        }

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var config = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };

                DataSet result = reader.AsDataSet(config);
                if (result.Tables.Count == 0)
                {
                    Debug.LogError("Excel 파일에 데이터가 없습니다.");
                    return null;
                }

                DataTable table = result.Tables[0];
                string json = DataTableToJson(table, dataType);

                // JSON 파일 저장
                string outputPath = $"Assets/Resources/JsonData/{dataType}.json";
                File.WriteAllText(outputPath, json);

                Debug.Log($"Excel → JSON 변환 완료: {outputPath}");
                return outputPath;
            }
        }
    }

    private static string DataTableToJson(DataTable table, ExcelConverterEditor.DataType dataType)
    {
        var jsonArray = new System.Text.StringBuilder();
        jsonArray.Append("[");

        for (int i = 0; i < table.Rows.Count; i++)
        {
            jsonArray.Append("{");
            for (int j = 0; j < table.Columns.Count; j++)
            {
                jsonArray.AppendFormat("\"{0}\": \"{1}\"", table.Columns[j].ColumnName, table.Rows[i][j].ToString());
                if (j < table.Columns.Count - 1)
                    jsonArray.Append(", ");
            }
            jsonArray.Append("}");

            if (i < table.Rows.Count - 1)
                jsonArray.Append(", ");
        }
        jsonArray.Append("]");

        return jsonArray.ToString();
    }
}
