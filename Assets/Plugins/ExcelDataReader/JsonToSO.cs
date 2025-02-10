using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

public static class JsonToSO
{
    public static void ConvertJsonToSO()
    {
        string jsonFolder = "Assets/Resources/JsonData"; // JSON 파일 경로

        // 무조건 모든 데이터 변환 시도
        if (!ConvertJsonToEnemySO(jsonFolder + "/Enemy.json"))
            Debug.LogWarning("Enemy.json 변환 실패 또는 파일 없음.");

        if (!ConvertJsonToTowerSO(jsonFolder + "/Tower.json"))
            Debug.LogWarning("Tower.json 변환 실패 또는 파일 없음.");

        if (!ConvertJsonToProjectileSO(jsonFolder + "/ProjectileData.json"))
            Debug.LogWarning("Projectile.json 변환 실패 또는 파일 없음.");
   
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("JSON 변환 완료! EnemySO, TowerSO, ProjectileSO,WaveSO 업데이트 완료.");
    }

    // EnemySO 변환
    private static bool ConvertJsonToEnemySO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath)) return false;

        string enemySOPath = "Assets/Resources/EnemySO";
        if (!Directory.Exists(enemySOPath)) Directory.CreateDirectory(enemySOPath);

        string jsonContent = File.ReadAllText(jsonFilePath);
        List<Dictionary<string, object>> enemyDataList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent);

        foreach (var data in enemyDataList)
        {
            string enemyName = data["UnitName"].ToString();
            string assetPath = $"{enemySOPath}/{enemyName}.asset";

            EnemySO enemySO = Resources.Load<EnemySO>($"EnemySO/{enemyName}") ?? ScriptableObject.CreateInstance<EnemySO>();

            enemySO.UnitName = enemyName;
            enemySO.Health = Convert.ToInt32(data["Health"]);
            enemySO.MovementSpeed = Convert.ToSingle(data["Speed"]);
            enemySO.AttackPower = Convert.ToInt32(data["AttackDamage"]);

            AssetDatabase.CreateAsset(enemySO, assetPath);
            EditorUtility.SetDirty(enemySO);
        }

        return true;
    }

    // TowerSO 변환
    private static bool ConvertJsonToTowerSO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath)) return false;

        string towerSOPath = "Assets/Resources/TowerSO";
        if (!Directory.Exists(towerSOPath)) Directory.CreateDirectory(towerSOPath);

        string jsonContent = File.ReadAllText(jsonFilePath);
        List<Dictionary<string, object>> towerDataList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent);

        foreach (var data in towerDataList)
        {
            string towerName = data["UnitName"].ToString();
            string assetPath = $"{towerSOPath}/{towerName}.asset";

            TowerSO towerSO = Resources.Load<TowerSO>($"TowerSO/{towerName}") ?? ScriptableObject.CreateInstance<TowerSO>();

            towerSO.UnitName = towerName;
            towerSO.AttackPower = Convert.ToInt32(data["AttackPower"]);
            towerSO.AttackSpeed = Convert.ToSingle(data["AttackSpeed"]);

            AssetDatabase.CreateAsset(towerSO, assetPath);
            EditorUtility.SetDirty(towerSO);
        }

        return true;
    }

    // ProjectileSO 변환
    private static bool ConvertJsonToProjectileSO(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"Projectile JSON 파일이 존재하지 않습니다: {jsonFilePath}");
            return false;
        }

        string projectileSOPath = "Assets/Resources/ProjectileSO";
        if (!Directory.Exists(projectileSOPath))
        {
            Debug.Log($"ProjectileSO 폴더가 존재하지 않음. 폴더 생성 시도: {projectileSOPath}");
            Directory.CreateDirectory(projectileSOPath);
        }

        Debug.Log($"폴더 확인 완료: {projectileSOPath}");

        string jsonContent = File.ReadAllText(jsonFilePath);
        List<Dictionary<string, object>> projectileDataList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent);

        foreach (var data in projectileDataList)
        {
            string projectileName = data["ProjectileName"].ToString();
            string assetPath = $"{projectileSOPath}/{projectileName}.asset";

            Debug.Log($"Projectile 변환 시작: {projectileName}, 저장 경로: {assetPath}");

            ProjectileSO projectileSO = Resources.Load<ProjectileSO>($"ProjectileSO/{projectileName}")
                ?? ScriptableObject.CreateInstance<ProjectileSO>();

            projectileSO.ProjectileName = projectileName;
            projectileSO.Speed = Convert.ToSingle(data["Speed"]);
            projectileSO.CanPierce = Convert.ToBoolean(data["CanPierce"]);
            projectileSO.PierceCount = Convert.ToInt32(data["PierceCount"]);
            projectileSO.CanSlow = Convert.ToBoolean(data["CanSlow"]);
            projectileSO.SlowEffect = Convert.ToSingle(data["SlowEffect"]);
            projectileSO.SlowDuration = Convert.ToSingle(data["SlowDuration"]);
            projectileSO.CanStun = Convert.ToBoolean(data["CanStun"]);
            projectileSO.StunDuration = Convert.ToSingle(data["StunDuration"]);

            if (!File.Exists(assetPath))
            {
                Debug.Log($"새로운 ProjectileSO 생성: {assetPath}");
                AssetDatabase.CreateAsset(projectileSO, assetPath);
            }
            else
            {
                Debug.Log($"기존 ProjectileSO 덮어쓰기: {assetPath}");
            }

            EditorUtility.SetDirty(projectileSO);
        }

        return true;
    }

}
