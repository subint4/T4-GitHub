using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DamageItemManager : MonoBehaviour
{
    public Button BombButton;
    public Button RocketButton;
    public Button StunButton;

    private DamageItemSettings settings;
    private float selectedTilePositionX = float.MinValue; // 선택된 타일의 X 좌표 저장
    private string selectedItemName = ""; // 선택된 아이템 이름 저장
    private LayerMask tileLayerMask; // `SpawnableArea` 감지용 레이어 마스크

    private void Start()
    {
        settings = DamageItemSettings.LoadSettings(); // JSON에서 데이터 로드
        tileLayerMask = LayerMask.GetMask("SpawnableArea"); // SpawnableArea 감지 활성화

        // 버튼 클릭 시 아이템 선택 (즉시 공격 X)
        if (BombButton != null)
            BombButton.onClick.AddListener(() => SelectItem("Bomb"));

        if (RocketButton != null)
            RocketButton.onClick.AddListener(() => SelectItem("Rocket"));

        if (StunButton != null)
            StunButton.onClick.AddListener(() => SelectItem("Stun"));
    }

    private void SelectItem(string itemName)
    {
        selectedItemName = itemName; // 사용자가 선택한 아이템 저장
        Debug.Log($"[DamageItemManager] {itemName} 아이템 선택됨! 타일을 클릭하세요.");
    }

    private void Update()
    {
        // 마우스 클릭 또는 터치 입력 감지 (UI 클릭 제외)
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            SetTargetTile();
        }
    }

    public void SetTargetTile()
    {
        if (string.IsNullOrEmpty(selectedItemName))
        {
            Debug.LogWarning("[DamageItemManager] 먼저 아이템을 선택해야 합니다!");
            return;
        }

        // 클릭한 위치에서 `SpawnableArea` 레이어 타일 찾기
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, tileLayerMask);
        if (hit.collider != null)
        {
            selectedTilePositionX = hit.collider.transform.position.x; // 타일의 X 좌표 저장
            Debug.Log($"[DamageItemManager] 타일 선택됨. 기준 X 좌표: {selectedTilePositionX}");
            UseItem(); // 타일 선택 후 아이템 실행
        }
        else
        {
            Debug.LogError("[DamageItemManager] 클릭한 위치에 유효한 타일이 없습니다!");
        }
    }

    private void UseItem()
    {
        if (selectedTilePositionX == float.MinValue || string.IsNullOrEmpty(selectedItemName))
        {
            Debug.LogError("[DamageItemManager] 아이템 또는 타일이 선택되지 않았습니다!");
            return;
        }

        // JSON에서 해당 아이템 데이터를 찾기
        DamageItemData itemData = settings.items.Find(item => item.ItemName == selectedItemName);

        if (itemData == null)
        {
            Debug.LogError($"[DamageItemManager] {selectedItemName}에 해당하는 데이터가 JSON에서 발견되지 않았습니다! JSON을 확인하세요.");
            return;
        }

        Debug.Log($"[DamageItemManager] {selectedItemName} 사용 시작! 대상 X 좌표: {selectedTilePositionX}");

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (allEnemies.Length == 0)
        {
            Debug.Log($"[DamageItemManager] {selectedItemName}: 현재 씬에 공격할 대상이 없습니다.");
            return;
        }

        bool targetHit = false;

        // 선택한 타일의 X좌표와 적의 X좌표 비교
        foreach (GameObject target in allEnemies)
        {
            Enemy enemyComponent = target.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                float enemyX = enemyComponent.transform.position.x;

                if (Mathf.Abs(enemyX - selectedTilePositionX) < 0.5f) // 오차 범위 0.5 적용
                {
                    targetHit = true;
                    enemyComponent.TakeDamage(itemData.damageAmount);
                    enemyComponent.ApplyStun(itemData.stunDuration);
                    Debug.Log($"[DamageItemManager] {target.name}에게 {itemData.damageAmount} 피해 및 {itemData.stunDuration}초 스턴 적용!");
                }
            }
        }

        if (!targetHit)
        {
            Debug.LogWarning($"[DamageItemManager] {selectedItemName} 아이템이 해당 X좌표({selectedTilePositionX})에서 아무 적도 맞추지 못함.");
        }

        selectedItemName = "";
        selectedTilePositionX = float.MinValue;
    }
}
