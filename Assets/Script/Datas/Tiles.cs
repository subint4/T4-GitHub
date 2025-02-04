
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    private TowerSpawner towerSpawner;
    public bool isOccupied = false;
    private void Start()
    {
        towerSpawner = FindObjectOfType<TowerSpawner>();
    }
    private void OnMouseDown()
    {
        if (towerSpawner != null)
        {
            if (isOccupied)
            {
                Debug.LogWarning("이미 타워가 있습니다.");
                return;
            }
            towerSpawner.spawnTower(this);
        }
    }
}
