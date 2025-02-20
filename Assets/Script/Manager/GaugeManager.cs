using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
    public Image gaugeFill;

    // 게이지 업데이트 함수
    public void UpdateGauge(float current, float max)
    {
        if (gaugeFill != null)
        {
            gaugeFill.fillAmount = current / max; // 게이지 값을 비율로 설정
        }
    }
}
