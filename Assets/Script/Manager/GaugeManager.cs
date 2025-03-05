using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
    public Image gaugeBackground; // **항상 보이는 배경 (흰색)**
    public Image gaugeFill;       // **차오르는 게이지 (파란색)**

    private int totalEnemies = 0;
    private int defeatedEnemies = 0;

    private void Start()
    {
        InitializeGauge();
    }

    private void OnEnable()
    {
        if (WaveManager.Instance != null)
        {
            UpdateGauge(WaveManager.Instance.defeatedEnemies, WaveManager.Instance.totalEnemies);
        }
    }

    /// <summary>
    /// **게이지 초기화 (배경 유지, 게이지 초기화)**
    /// </summary>
    private void InitializeGauge()
    {
        if (gaugeBackground == null || gaugeFill == null)
        {
            Debug.LogError("[GaugeManager] UI 요소가 설정되지 않았습니다!");
            return;
        }

        // **배경은 항상 흰색 유지**
        gaugeBackground.color = Color.white;

        // **게이지 바는 파란색으로 설정하지만 fillAmount는 0 (처음엔 비어있음)**
        gaugeFill.color = Color.blue;
        gaugeFill.fillAmount = 0f;

        // **현재 스테이지에서 적 수 가져오기**
        if (WaveManager.Instance != null)
        {
            totalEnemies = WaveManager.Instance.totalEnemies;
        }

        defeatedEnemies = 0;
        UpdateGauge(defeatedEnemies, totalEnemies);
    }

    /// <summary>
    /// **게이지 업데이트 (처치 수 반영)**
    /// </summary>
    public void UpdateGauge(int defeated, int total)
    {
        if (gaugeBackground == null || gaugeFill == null)
        {
            Debug.LogError("[GaugeManager] UI 요소가 설정되지 않았습니다!");
            return;
        }

        if (total == 0)
        {
            Debug.LogWarning("[GaugeManager] 적이 존재하지 않습니다!");
            return;
        }

        float progress = (float)defeated / total;

        // **게이지 바가 왼쪽에서 오른쪽으로 차오름**
        gaugeFill.fillAmount = progress;

        Debug.Log($"[GaugeManager] 게이지 업데이트: {defeated} / {total}");
    }

    /// <summary>
    /// **적 처치 시 호출 (WaveManager에서 호출)**
    /// </summary>
    public void OnEnemyDefeated()
    {
        if (WaveManager.Instance == null)
        {
            Debug.LogError("[GaugeManager] WaveManager를 찾을 수 없습니다!");
            return;
        }

        UpdateGauge(WaveManager.Instance.defeatedEnemies, WaveManager.Instance.totalEnemies);
    }
}
