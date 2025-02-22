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
    private float selectedTileX = float.MinValue; // 선택된 타일의 X좌표 저장
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
            Tiles tile = hit.collider.GetComponent<Tiles>();
            if (tile != null)
            {
                selectedTileX = tile.tileXPosition;
                Debug.Log($"[DamageItemManager] 타일 선택됨. 기준 X 좌표: {selectedTileX}");
                UseItem(); // 타일 선택 후 아이템 실행
            }
            else
            {
                Debug.LogError("[DamageItemManager] 감지된 오브젝트가 `Tiles` 컴포넌트를 가지고 있지 않습니다!");
            }
        }
        else
        {
            Debug.LogError("[DamageItemManager] 클릭한 위치에 유효한 타일이 없습니다!");
        }
    }

    private void UseItem()
    {
        if (selectedTileX == float.MinValue || string.IsNullOrEmpty(selectedItemName))
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

        Debug.Log($"[DamageItemManager] {selectedItemName} 사용 시작! 대상 X 좌표: {selectedTileX}");

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");

        List<GameObject> allTargets = new List<GameObject>();
        allTargets.AddRange(enemies);
        allTargets.AddRange(bosses);

        if (allTargets.Count == 0)
        {
            Debug.Log($"[DamageItemManager] {selectedItemName}: 현재 씬에 공격할 대상이 없습니다.");
            return;
        }

        bool targetHit = false;

        // 선택한 타일의 X축에 있는 적들만 공격
        foreach (GameObject target in allTargets)
        {
            if (Mathf.Abs(target.transform.position.x - selectedTileX) < 0.1f)
            {
                targetHit = true;
                Enemy enemyComponent = target.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    if (itemData.damageAmount > 0)
                    {
                        enemyComponent.TakeDamage(itemData.damageAmount);
                        Debug.Log($"[DamageItemManager] {target.name}에게 {itemData.damageAmount} 피해를 입힘.");
                    }

                    if (itemData.stunDuration > 0)
                    {
                        enemyComponent.ApplyStun(itemData.stunDuration);
                        Debug.Log($"[DamageItemManager] {target.name}에게 {itemData.stunDuration}초 스턴 적용.");
                    }
                }
            }
        }

        if (!targetHit)
        {
            Debug.LogWarning($"[DamageItemManager] {selectedItemName} 아이템이 해당 X좌표({selectedTileX})에서 아무 적도 맞추지 못함.");
        }

        // 아이템 사용 후 선택 해제
        selectedItemName = "";
        selectedTileX = float.MinValue;
        Debug.Log("[DamageItemManager] 아이템 사용 완료! 다시 선택하세요.");
    }
}
