using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
    public Image gaugeFill; // **게이지 바 (파란색으로 채울 부분)**
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
    /// 게이지 초기화 (총 적 수 설정)
    /// </summary>
    private void InitializeGauge()
    {
        if (gaugeFill != null)
        {
            gaugeFill.fillAmount = 0f; // 처음에는 게이지를 0으로 설정
        }
        else
        {
            Debug.LogError("[GaugeManager] gaugeFill이 설정되지 않았습니다!");
            return;
        }

        // 현재 스테이지에서 적 수 가져오기
        if (WaveManager.Instance != null)
        {
            totalEnemies = WaveManager.Instance.totalEnemies;
        }

        defeatedEnemies = 0;
        UpdateGauge(defeatedEnemies, totalEnemies);
    }

    /// <summary>
    /// 게이지 업데이트 (처치 수 반영)
    /// </summary>
    public void UpdateGauge(int defeated, int total)
    {
        if (gaugeFill == null)
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

        // `fillAmount`를 사용하여 왼쪽부터 차오르게 설정 (0~1)
        gaugeFill.fillAmount = progress;

        // 색상 변경 (흰색 → 파란색)
        UpdateGaugeColor(progress);

        Debug.Log($"[GaugeManager] 게이지 업데이트: {defeated} / {total}");
    }

    /// <summary>
    /// 적 처치 시 호출 (WaveManager에서 호출)
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

    /// <summary>
    /// 게이지 색상 변경 (흰색 → 파란색)
    /// </summary>
    private void UpdateGaugeColor(float progress)
    {
        if (gaugeFill == null)
        {
            Debug.LogWarning("[GaugeManager] 게이지 Fill 이미지가 설정되지 않았습니다!");
            return;
        }

        // **흰색에서 파란색으로 점진적으로 변함**
        gaugeFill.color = Color.Lerp(Color.white, Color.blue, progress);
    }
}
