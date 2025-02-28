using UnityEngine;
using System.IO;

public class JsonLoader : MonoBehaviour
{
    public static JsonLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// `Resources/JsonData/` 폴더에서 JSON 파일을 로드 (확장자 제외)
    /// </summary>
    public static string LoadJsonFromResources(string fileName)
    {
        string path = $"JsonData/{Path.GetFileNameWithoutExtension(fileName)}"; // 확장자 제거 후 경로 설정
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogError($"[JsonLoader] {fileName} JSON 파일을 Resources/JsonData/에서 찾을 수 없습니다! (경로: {path})");
            return "";
        }
        return jsonFile.text;
    }
}
