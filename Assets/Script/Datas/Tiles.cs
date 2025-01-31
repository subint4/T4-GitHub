
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    private TowerSpawner towerSpawner;

    private void Start()
    {
        towerSpawner = FindObjectOfType<TowerSpawner>();
    }
    private void OnMouseDown()
    {
        if (towerSpawner != null)
        {
            Vector3 tilePosition = transform.position;
            towerSpawner.spawnTower(tilePosition);
        }
    }
}
