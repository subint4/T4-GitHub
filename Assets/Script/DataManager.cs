using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    private TowerStatDatasLoader _towerStatDatasLoader;

    public TowerStatDatasLoader TowerStatDatasLoader
    {
        get { return _towerStatDatasLoader; }
    }

    private PlayerTowerStatDatasLoader _playerTowerStatDatasLoader;

    public PlayerTowerStatDatasLoader PlayerTowerStatDatasLoader
    {
        get { return _playerTowerStatDatasLoader; }
    }

    private void Awake()
    {
        _towerStatDatasLoader = new TowerStatDatasLoader();
        _playerTowerStatDatasLoader = new PlayerTowerStatDatasLoader(); 
    }
}
