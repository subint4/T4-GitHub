using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GaugeManager : MonoBehaviour
{
    public Slider gaugeSlider;
    private int totalEnemies = 0;
    private int defeatedEnemies = 0;

    private void Start()
    {
        // 현재 스테이지와 서브 스테이지 정보를 가져옴
        int stageNum = PlayerPrefs.GetInt("CurrentStage", 1);
        int subStageNum = PlayerPrefs.GetInt("CurrentSubStage", 1);

        // 스테이지에서 해당하는 웨이브 ID 리스트 가져오기
        List<int> waveIDs = DataManager.Instance.StageDataManager.GetWaveIDsForStage(stageNum, subStageNum);
        List<WaveSO> waveDataList = DataManager.Instance.WaveDataManager.GetWaveDataList(waveIDs);

        // 적 수 카운트
        totalEnemies = CalculateTotalEnemies(waveDataList);
        defeatedEnemies = 0;

        // 게이지 초기화
        UpdateGauge(defeatedEnemies, totalEnemies);
    }

    /// <summary>
    /// 웨이브 데이터를 기반으로 총 적 수를 계산
    /// </summary>
    private int CalculateTotalEnemies(List<WaveSO> waveDataList)
    {
        int count = 0;
        foreach (var wave in waveDataList)
        {
            foreach (var spawnData in wave.spawnDataList)
            {
                count += spawnData.count;
            }
        }
        return count;
    }

    /// <summary>
    /// 게이지 업데이트
    /// </summary>
    public void UpdateGauge(int defeated, int total)
    {
        if (gaugeSlider == null)
        {
            Debug.LogError("[GaugeManager] Slider가 설정되지 않았습니다!");
            return;
        }

        if (total == 0)
        {
            Debug.LogWarning("[GaugeManager] 적이 존재하지 않습니다!");
            return;
        }

        defeatedEnemies = defeated;
        gaugeSlider.value = (float)defeated / total;
        Debug.Log($"[GaugeManager] 게이지 업데이트: {defeated} / {total}");
    }

    /// <summary>
    /// 적이 처치될 때 호출하여 게이지 업데이트
    /// </summary>
    public void OnEnemyDefeated()
    {
        defeatedEnemies++;
        UpdateGauge(defeatedEnemies, totalEnemies);
    }
}
