using UnityEngine;

public class Tiles : MonoBehaviour
{
    public bool isOccupied = false;
    public Tower currentTower = null;

    private void OnMouseDown()
    {
        Debug.Log($"타일 클릭됨: {transform.position}, isOccupied: {isOccupied}");

        if (TowerManager.Instance != null && !isOccupied)
        {
            TowerManager.Instance.PlaceSelectedTower(this);
        }
        else
        {
            Debug.LogWarning($"타워 배치 불가: {transform.position} (isOccupied: {isOccupied})");
        }
    }

    public void PlaceTower(Tower tower)
    {
        isOccupied = true;
        currentTower = tower;
        tower.currentTile = this;
        Debug.Log($"타워 배치됨: {transform.position}");
    }
}
