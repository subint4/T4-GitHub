using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 유지
            Debug.Log("DataManager가 정상적으로 초기화되었습니다.");
        }
        else
        {
            Debug.LogWarning("DataManager의 중복 인스턴스가 감지되었습니다! 기존 인스턴스를 유지하고 새로 생성된 인스턴스를 삭제합니다.");
            Destroy(gameObject);
        }
    }
public EnemySO GetEnemyData(int enemyID)
    {
        return EnemyManager.Instance?.GetEnemyData(enemyID);
    }

    public TowerSO GetTowerData(int towerID)
    {
        return TowerManager.Instance?.GetTowerData(towerID);
    }

    public WaveSO GetWaveData(int waveID)
    {
        return WaveManager.Instance?.GetWaveData(waveID);
    }
}
