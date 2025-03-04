using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DamageItemManager : MonoBehaviour
{
    public static DamageItemManager Instance { get; private set; }

    public Button BombButton;
    public Button RocketButton;
    public Button StunButton;

    private DamageItemSettings settings;
    private string selectedItemName = ""; // 선택된 아이템 이름 저장

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        settings = DamageItemSettings.LoadSettings(); // JSON에서 데이터 로드

        if (settings == null || settings.Data == null || settings.Data.Count == 0)
        {
            Debug.LogError("[DamageItemManager] JSON 데이터가 로드되지 않았거나 비어 있습니다!");
            return;
        }

        Debug.Log($"[DamageItemManager] JSON에서 {settings.Data.Count}개의 아이템이 로드됨.");

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

    public string GetSelectedItemName()
    {
        return selectedItemName;
    }

    public void UseItemOnTile(Vector3 tilePosition)
    {
        Debug.Log($"[DamageItemManager] UseItemOnTile 호출됨 - 대상 타일 위치: {tilePosition}, 선택된 아이템: {selectedItemName}");

        if (string.IsNullOrEmpty(selectedItemName))
        {
            Debug.LogError("[DamageItemManager] 아이템이 선택되지 않았습니다!");
            return;
        }

        if (settings == null || settings.Data == null)
        {
            Debug.LogError("[DamageItemManager] JSON 데이터가 로드되지 않았습니다! `settings`를 확인하세요.");
            return;
        }

        DamageItemData itemData = settings.Data.Find(item => item.ItemName == selectedItemName);

        if (itemData == null)
        {
            Debug.LogError($"[DamageItemManager] {selectedItemName}에 해당하는 데이터가 JSON에서 발견되지 않았습니다! JSON을 확인하세요.");
            return;
        }

        Debug.Log($"[DamageItemManager] {selectedItemName} 사용 시작! 대상 타일: {tilePosition}");

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (allEnemies.Length == 0)
        {
            Debug.Log($"[DamageItemManager] {selectedItemName}: 현재 씬에 공격할 대상이 없습니다.");
            return;
        }

        bool targetHit = false;

        foreach (GameObject target in allEnemies)
        {
            Enemy enemyComponent = target.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                float enemyX = enemyComponent.transform.position.x;
                float enemyY = enemyComponent.transform.position.y;

                if (Mathf.Abs(enemyX - tilePosition.x) < 0.5f && Mathf.Abs(enemyY - tilePosition.y) < 0.5f)
                {
                    targetHit = true;
                    enemyComponent.TakeDamage(itemData.Damage);
                    enemyComponent.ApplyStun(itemData.StunDuration);
                    Debug.Log($"[DamageItemManager] {target.name}에게 {itemData.Damage} 피해 및 {itemData.StunDuration}초 스턴 적용!");
                }
            }
        }

        if (!targetHit)
        {
            Debug.LogWarning($"[DamageItemManager] {selectedItemName} 아이템이 타일({tilePosition})에서 아무 적도 맞추지 못함.");
        }

        selectedItemName = "";
    }
}
