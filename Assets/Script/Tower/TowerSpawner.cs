using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    private GameObject selectedTowerPrefab;
    private bool isTowerSelected=false;

    public LayerMask SpawnableArea; // 타일이 있는 레이어 설정
    public float raycastDistance = 10f;
    public void SelectedTower(int index)
    {
        if (index >= 0 && index < towerPrefabs.Length)
        {
            selectedTowerPrefab = towerPrefabs[index];
            isTowerSelected = true;
            Debug.Log($"타워 선택: {selectedTowerPrefab.name}");
        }
        else
        {
            Debug.LogError("유효하지 않은 인덱스.");
        }
    }
    private void Update()
    {
        if(isTowerSelected && Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, raycastDistance, SpawnableArea);

            if (hit.collider != null) // 타일 위를 클릭했을 경우
            {
                Vector3 spawnPosition = hit.collider.transform.position; // 타일의 위치를 가져옴
                spawnTower(spawnPosition);
            }
            else
            {
                Debug.LogWarning("타워는 스폰 타일 위에만 배치 가능합니다!");
            }
        }
    }
    public void spawnTower(Vector3 position)
    {
        if (isTowerSelected && selectedTowerPrefab != null)
        {
            Instantiate(selectedTowerPrefab,position,Quaternion.identity);
            isTowerSelected = false;
            Debug.Log($"타워 배치 완료 : {position}");
        }
        else
        {
            Debug.LogWarning("타워가 선택되지 않았거나 프리팹이 설정되지 않음");
        }
    }
   
}