using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyDataLoader : MonoBehaviour
{
    public TextAsset enemyDataJson;

    [ContextMenu("Load Enemy Data")]

    public void LoadEnemyData()
    {
        if (enemyDataJson == null)
        {
            Debug.LogError("EnemyDataJson 파일이 지정되지 않았습니다.");
            return;
        }
        try
        {
            // JSON 데이터 읽기
            EnemyConfig enemyConfig = JsonConvert.DeserializeObject<EnemyConfig>(enemyDataJson.text);

            foreach (EnemyData enemy in enemyConfig.Enemies)
            {
                // 기존 EnemySO 파일 찾기 (이름 기반)
                string assetPath = $"Assets/EnemyData/Enemy_{enemy.ID}.asset";
                EnemySO enemySO = AssetDatabase.LoadAssetAtPath<EnemySO>(assetPath);

                if (enemySO == null)
                {
                    // 기존 EnemySO가 없으면 새로 생성
                    enemySO = ScriptableObject.CreateInstance<EnemySO>();
                    AssetDatabase.CreateAsset(enemySO, assetPath);
                }

                // JSON 데이터를 기존 EnemySO에 적용
                enemySO.LoadFromJson(enemy);

                // 변경 사항 저장
                EditorUtility.SetDirty(enemySO);
            }

            Debug.Log("EnemySO 업데이트 완료!");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error: JSON 데이터를 불러오는 중 오류 발생! {ex.Message}");
        }
    }
}


