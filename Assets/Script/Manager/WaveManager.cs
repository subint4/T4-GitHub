using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class WaveManager : MonoBehaviour
{
    public TextAsset jsonFile; // JSON 파일 (유니티 에디터에서 넣어줌)
    private List<WaveStageData> waves;

    private void Start()
    {
        LoadWaveData();
    }

    private void LoadWaveData()
    {
        if (jsonFile == null)
        {
            Debug.LogError("Error: JSON 파일을 찾을 수 없습니다.");
            return;
        }

        string jsonData = jsonFile.text;
        WaveStageConfig waveConfig = JsonConvert.DeserializeObject<WaveStageConfig>(jsonData);
        waves = waveConfig.WaveStages;
    }
}
