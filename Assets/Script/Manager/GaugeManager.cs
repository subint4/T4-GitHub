using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
    public Slider gaugeBar;
    //게이지 아무것도 없을때 색
    public Color normalColor = Color.white;
    //게이지 바 찰때 색
    public Color fullColor = Color.blue;

    private int totalEnemies;
    private int defeatedEnemies = 0;

    private void Start()
    {
        if (gaugeBar == null)
        {
            Debug.LogError("게이지 바 UI가 안됬습니다");
        }
        gaugeBar.value = 0;
    }

    public void InitializeGauge(int total)
    {
        totalEnemies = total;
        defeatedEnemies = 0;
        gaugeBar.value = 0;
        gaugeBar.fillRect.GetComponent<Image>().color = normalColor;
    }

    public void UpdateGage()
    {
        defeatedEnemies += 1;
        gaugeBar.value = (float)defeatedEnemies / totalEnemies;

        if (defeatedEnemies >= totalEnemies)
        {
            gaugeBar.fillRect.GetComponent <Image>().color = fullColor;
            Debug.Log("색깔 변했나?");
        }
    }
}
