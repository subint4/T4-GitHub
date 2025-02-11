using System.IO;
using UnityEngine;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TowerDataLoader : MonoBehaviour
{
    public TextAsset unitDataJson;

    [ContextMenu("Load Tower Data")]
    public void LoadTowerData()
    {
        if (unitDataJson == null)
        {
            Debug.LogError("unitDataJson 파일이 지정되지 않았습니다.");
            return;
        }

        try
        {
            UnitConfig unitConfig = JsonConvert.DeserializeObject<UnitConfig>(unitDataJson.text);
            if (unitConfig == null || unitConfig.Units == null)
            {
                Debug.LogError("JSON 데이터가 올바르지 않습니다.");
                return;
            }

            foreach (UnitData unit in unitConfig.Units)
            {
                if (unit == null)
                {
                    Debug.LogError("unit 데이터가 null입니다!");
                    continue;
                }

                string assetPath = $"Assets/TowerData/Tower_{unit.UnitName}.asset";
                TowerSO tower = AssetDatabase.LoadAssetAtPath<TowerSO>(assetPath);

                if (tower == null)
                {
                    Debug.Log($"새로운 TowerSO 생성: {assetPath}");
                    tower = ScriptableObject.CreateInstance<TowerSO>();
                    AssetDatabase.CreateAsset(tower, assetPath);
                }

                if (tower != null)
                {
                    tower.LoadFromUnitData(unit);
                    EditorUtility.SetDirty(tower);
                }
                else
                {
                    Debug.LogError($"TowerSO 생성 실패: {assetPath}");
                }
            }

            Debug.Log("TowerSO 업데이트 완료!");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error: JSON 데이터를 불러오는 중 오류 발생! {ex.Message}");
        }
    }
}
