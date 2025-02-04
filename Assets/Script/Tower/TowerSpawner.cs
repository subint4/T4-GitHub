using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    private GameObject selectedTowerPrefab;
    private bool isTowerSelected = false;

    public LayerMask SpawnableArea; // 타일이 있는 레이어 설정
    public float raycastDistance = 10f;

    private PlayerSystem playerSystem;
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
        if (isTowerSelected && Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, raycastDistance, SpawnableArea);

            if (hit.collider != null) // 타일 위를 클릭했을 경우
            {
                Tiles targetTile = hit.collider.GetComponent<Tiles>();
                if (targetTile != null)
                {
                    if (targetTile.isOccupied)
                    {
                        Debug.LogWarning("해당 타일에는 이미 타워가 있습니다!");
                        return;
                    }

                    Tower towerComponent = selectedTowerPrefab.GetComponent<Tower>();
                    if (towerComponent != null)
                    {
                        if (playerSystem.Money < towerComponent.towerStats.DeployCost)
                        {
                            Debug.LogWarning("돈이 부족하여 타워를 배치할 수 없습니다!");
                            return;
                        }

                        // 비용 차감
                        playerSystem.Money -= towerComponent.towerStats.DeployCost;

                        // 타워 배치
                        spawnTower(targetTile);
                    }
                }
            }
            else
            {
                Debug.LogWarning("타워는 스폰 타일 위에만 배치 가능합니다!");
            }
        }
    }
    public void spawnTower(Tiles targetTile)
    {
        if (isTowerSelected && selectedTowerPrefab != null)
        {
            if(targetTile.isOccupied)
            {
                Debug.LogWarning("해당 타일에는 이미 타워가 있습니다.");
            }
            Tower newTower = Instantiate(selectedTowerPrefab, targetTile.transform.position, Quaternion.identity).GetComponent<Tower>();
            if (newTower != null)
            {
                targetTile.isOccupied = true;
                isTowerSelected = false;
                Debug.Log($"타워 배치 완료: {targetTile.transform.position}");
            }
            else
            {
                {
                    Debug.LogWarning("타워가 선택되지 않았거나 프리팹이 설정되지 않음");

                }
            }
        }
    }
}