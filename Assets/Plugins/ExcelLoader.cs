using System;
using System.Data;
using System.IO;
using UnityEngine;
using ExcelDataReader;

public static class ExcelLoader
{
    public static DataTable LoadExcel(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Error: Excel 파일을 찾을 수 없습니다: {filePath}");
            return null;
        }

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // 첫 번째 행을 컬럼 이름으로 사용
                var config = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true  // 첫 번째 행을 "헤더"로 인식하도록 설정
                    }
                };

                DataSet result = reader.AsDataSet(config);
                if (result.Tables.Count == 0)
                {
                    Debug.LogError("Error: Excel 파일에 시트가 없습니다.");
                    return null;
                }

                Debug.Log($"Excel 로드 완료! 파일: {filePath}, 총 {result.Tables[0].Rows.Count}개 행");
                return result.Tables[0]; // 첫 번째 시트 반환
            }
        }
    }
}
