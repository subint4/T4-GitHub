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

    private DataManager() { }

    private void Initialize()
    {
        EnemyDataManager = new EnemyDataManager();
        WaveDataManager = new WaveDataManager();
        TowerDataManager = new TowerDataManager();

        EnemyDataManager.LoadData();
        TowerDataManager.LoadData();
        WaveDataManager.LoadWaveData();
        Debug.Log("DataManager가 정상적으로 초기화되었습니다.");
    }
}
