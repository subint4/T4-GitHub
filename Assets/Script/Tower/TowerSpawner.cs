using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    private GameObject selectedTowerPrefab;
    private bool isTowerSelected = false;
    private int selectedTowerIndex = -1; // 선택된 타워 인덱스 저장

    public LayerMask SpawnableArea; // 타일이 있는 레이어 설정
    public float raycastDistance = 10f;

    private PlayerSystem playerSystem;

    private void Update()
    {
        if (isTowerSelected && Input.GetMouseButtonDown(0)) // 타워 선택 후 클릭하면 배치
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, raycastDistance, SpawnableArea);

            if (hit.collider != null) // 타일 클릭 감지
            {
                Tiles targetTile = hit.collider.GetComponent<Tiles>();
                if (targetTile != null)
                {
                    if (targetTile.isOccupied)
                    {
                        Debug.LogWarning("해당 타일에는 이미 타워가 있습니다!");
                        return;
                    }

                    // 타워 배치
                    SpawnTower(targetTile);
                }
            }
            else
            {
                Debug.LogWarning("타워는 스폰 타일 위에만 배치 가능합니다!");
            }
        }
    }

    // 버튼 클릭 시 실행되는 메서드 (타워 선택만 함)
    public void SelectedTower(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPrefabs.Length)
        {
            Debug.LogError("잘못된 타워 인덱스입니다.");
            return;
        }

        selectedTowerPrefab = towerPrefabs[towerIndex];
        selectedTowerIndex = towerIndex;
        isTowerSelected = true; //  타워 배치 모드 활성화
        Debug.Log($"타워 {towerIndex} 선택됨, 타일 클릭 대기 중...");
    }

    // 타일을 클릭하면 호출되는 타워 배치 함수
    public void SpawnTower(Tiles targetTile)
    {
        Debug.Log($"SpawnTower 호출됨 - 타일 위치: {targetTile.transform.position}, isOccupied: {targetTile.isOccupied}");

        if (isTowerSelected && selectedTowerPrefab != null)
        {
            if (targetTile.isOccupied)
            {
                Debug.LogWarning($"해당 타일({targetTile.transform.position})에는 이미 타워가 있습니다! (isOccupied: {targetTile.isOccupied})");
                return;
            }

            Debug.Log($"타워 배치 시도: {targetTile.transform.position}");

            Tower newTower = Instantiate(selectedTowerPrefab, targetTile.transform.position, Quaternion.identity).GetComponent<Tower>();
            if (newTower != null)
            {
                targetTile.PlaceTower(newTower);
                Debug.Log($"타워 배치 완료: {targetTile.transform.position} (isOccupied: {targetTile.isOccupied})");
            }
            else
            {
                Debug.LogWarning("타워 배치에 실패했습니다.");
            }

            isTowerSelected = false;
            selectedTowerPrefab = null;
            selectedTowerIndex = -1;
        }
    }

}
