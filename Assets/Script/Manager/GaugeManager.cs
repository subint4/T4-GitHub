using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
    public Image gaugeFill;
    public int currentStageNum = 1;

    private WaveDateManager waveDateManager = new WaveDateManager();
    
    public void Start()
    {        
        waveDateManager.LoadWaveData();

        int monsterCount = waveDateManager.GetMonsterCount(currentStageNum);

        UpdateGauge(monsterCount, 100f);
    }
    public void UpdateGauge(float remaining, float total)
    {
        if (gaugeFill != null)
        {
            gaugeFill.fillAmount = (total - remaining) / total;
        }
    }
}
