using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
    public Image gaugeFill;  // **파란색으로 차오르는 게이지 바**
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
            UpdateGauge(WaveManager.Instance.defeatedEnemies, WaveManager.Instance.totalEnemiesPerStage);
        }
    }

    /// <summary>
    /// **게이지 초기화**
    /// </summary>
    private void InitializeGauge()
    {
        if (gaugeFill == null)
        {
            Debug.LogError("[GaugeManager] gaugeFill이 설정되지 않았습니다!");
            return;
        }

        gaugeFill.fillAmount = 0f; // **초기 게이지 비움**
        gaugeFill.color = Color.blue; // **게이지 색상을 파란색으로 설정**

        if (WaveManager.Instance != null)
        {
            totalEnemies = WaveManager.Instance.totalEnemiesPerStage;  // **전체 적 수 가져오기**
        }

        defeatedEnemies = 0;
        UpdateGauge(defeatedEnemies, totalEnemies);
    }

    /// <summary>
    /// **게이지 업데이트 (처치 수 반영)**
    /// </summary>
    public void UpdateGauge(int defeated, int total)
    {
        if (gaugeFill == null)
        {
            Debug.LogError("[GaugeManager] UI 요소가 설정되지 않았습니다!");
            return;
        }

        if (WaveManager.Instance == null)
        {
            Debug.LogError("[GaugeManager] WaveManager를 찾을 수 없습니다!");
            return;
        }

        int totalEnemiesForGauge = WaveManager.Instance.totalEnemiesPerStage; // **스테이지 전체 적 수 사용**
        int defeatedEnemiesForGauge = WaveManager.Instance.defeatedEnemies;

        if (totalEnemiesForGauge == 0)
        {
            Debug.LogWarning("[GaugeManager] 총 적 수가 0으로 설정됨.");
            return;
        }

        float progress = (float)defeatedEnemiesForGauge / totalEnemiesForGauge;

        // **게이지 바가 왼쪽에서 오른쪽으로 차오름**
        gaugeFill.fillAmount = progress;

        Debug.Log($"[GaugeManager] 게이지 업데이트: {defeatedEnemiesForGauge} / {totalEnemiesForGauge}");
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

        UpdateGauge(WaveManager.Instance.defeatedEnemies, WaveManager.Instance.totalEnemiesPerStage);
    }
}
