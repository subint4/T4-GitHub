using UnityEngine;

public class DataManager
{
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataManager();
                instance.Initialize();
            }
            return instance;
        }
    }

    public EnemyDataManager EnemyDataManager { get; private set; }
    public WaveDataManager WaveDataManager { get; private set; }
    public TowerDataManager TowerDataManager { get; private set; }
    public StageDataManager StageDataManager { get; private set; }

    private DataManager() { }

    private void Initialize()
    {
        try
        {
            Debug.Log("[DataManager] 데이터 매니저 초기화 시작...");

            // SO 기반 데이터 로드
            EnemyDataManager = new EnemyDataManager();
            TowerDataManager = new TowerDataManager();
            WaveDataManager = new WaveDataManager();

            EnemyDataManager.LoadData();
            TowerDataManager.LoadData();
            WaveDataManager.LoadWaveData();

            Debug.Log("[DataManager] SO 기반 데이터 로드 완료.");

            // JSON 기반 데이터 로드 (StageDataManager)
            StageDataManager = new StageDataManager();
            StageDataManager.LoadStageData();

            Debug.Log("[DataManager] JSON 기반 데이터 로드 완료.");

            Debug.Log("[DataManager] 모든 데이터 매니저가 정상적으로 초기화되었습니다.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[DataManager] 초기화 중 오류 발생: {ex.Message}");
        }
    }
}
