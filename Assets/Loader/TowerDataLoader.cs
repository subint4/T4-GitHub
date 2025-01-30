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
            Debug.LogError("unitDataJson파일이 지정되지 않았습니다.");
            return;
        }
        try
        {
            UnitConfig unitConfig = JsonConvert.DeserializeObject<UnitConfig>(unitDataJson.text);

            foreach (UnitData unit in unitConfig.Units)
            {
                string assetPath = $"Assets/TowerData/Tower_{unit.UnitName}.asset";
                TowerSO tower = AssetDatabase.LoadAssetAtPath<TowerSO>(assetPath);

                if (tower == null)
                {
                    tower = ScriptableObject.CreateInstance<TowerSO>();
                    AssetDatabase.CreateAsset(tower, assetPath);
                }
                tower.LoadFromUnitData(unit);

                EditorUtility.SetDirty(tower);
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

