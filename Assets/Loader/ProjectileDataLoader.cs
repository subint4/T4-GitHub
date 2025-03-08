//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//using ExcelDataReader;

//public class ProjectileDataLoader : MonoBehaviour
//{
//    private static Dictionary<TowerType, ProjectileData> projectileDataDict = new Dictionary<TowerType, ProjectileData>();

//    [System.Serializable]
//    public class ProjectileData
//    {
//        public float Speed;
//        public bool CanPierce;
//        public bool HasExplosion;
//        public float SlowEffect;
//        public float SlowDuration;
//    }

//    private void Awake()
//    {
//        LoadProjectileData();
//    }

//    public static void LoadProjectileData()
//    {
//        string filePath = Path.Combine(Application.streamingAssetsPath, "unit_data.xlsx");

//        if (!File.Exists(filePath))
//        {
//            Debug.LogError("엑셀 파일을 찾을 수 없습니다!");
//            return;
//        }

//        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
//        {
//            using (var reader = ExcelReaderFactory.CreateReader(stream))
//            {
//                while (reader.Read())
//                {
//                    if (reader.Depth == 0) continue; // 첫 번째 행(헤더) 건너뛰기

//                    TowerType type;
//                    if (!System.Enum.TryParse(reader.GetString(1), out type)) continue; // TowerType 변환

//                    ProjectileData data = new ProjectileData
//                    {
//                        Speed = (float)reader.GetDouble(2),
//                        CanPierce = reader.GetString(3).ToLower() == "true",
//                        HasExplosion = reader.GetString(4).ToLower() == "true",
//                        SlowEffect = (float)reader.GetDouble(5),
//                        SlowDuration = (float)reader.GetDouble(6)
//                    };

//                    projectileDataDict[type] = data;
//                }
//            }
//        }

//        Debug.Log($"엑셀 데이터 로드 완료! {projectileDataDict.Count}개의 투사체 데이터가 로드되었습니다.");
//    }

//    public static ProjectileData GetProjectileData(TowerType towerType)
//    {
//        if (projectileDataDict.TryGetValue(towerType, out ProjectileData data))
//        {
//            return data;
//        }
//        else
//        {
//            Debug.LogWarning($"해당 타워 유형({towerType})에 대한 데이터가 없습니다. 기본값을 반환합니다.");
//            return new ProjectileData { Speed = 10, CanPierce = false, HasExplosion = false, SlowEffect = 0, SlowDuration = 0 };
//        }
//    }
//}
