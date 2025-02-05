using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    private TowerSpawner towerSpawner;
    public bool isOccupied = false;
    public Tower currentTower = null;

    private void Start()
    {
        towerSpawner = FindObjectOfType<TowerSpawner>();
    }

    public void PlaceTower(Tower tower)
    {
        isOccupied = true;
        currentTower = tower;
        tower.currentTiles = this;
        Debug.Log($"타워 배치됨: {transform.position} (isOccupied: {isOccupied})");
    }

    public void RemoveTower()
    {
        if (currentTower != null)
        {
            Debug.Log($"타워 제거 시작: {transform.position}");
            Destroy(currentTower.gameObject);
            currentTower = null;
            isOccupied = false; // 타일 다시 사용 가능
            Debug.Log($"타워 제거 완료: {transform.position}, isOccupied: {isOccupied}");
        }
        else
        {
            Debug.LogWarning($"RemoveTower() 실행됨, 하지만 currentTower가 null입니다. (isOccupied: {isOccupied})");
        }
    }


    private void OnMouseDown()
    {
        Debug.Log($"타일 클릭됨: {transform.position}, isOccupied: {isOccupied}");

        if (towerSpawner != null && !isOccupied) // 타워가 없는 경우만 배치 가능
        {
            Debug.Log($"타워 배치 가능: {transform.position}");
            towerSpawner.SpawnTower(this);
        }
        else
        {
            Debug.LogWarning($"타워 배치 불가: {transform.position} (isOccupied: {isOccupied})");
        }
    }
}
