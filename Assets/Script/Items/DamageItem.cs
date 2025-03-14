using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DamageItemManager : MonoBehaviour
{
    public static DamageItemManager Instance { get; private set; }

    public Button BombButton;
    public Button RocketButton;
    public Button StunButton;

    private ItemSettings settings;
    public int selectedItemID = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        settings = ItemSettings.LoadFromJson();

        if (settings == null || settings.Data == null || settings.Data.Count == 0)
        {
            Debug.LogError("[DamageItemManager] JSON 데이터가 로드되지 않았거나 비어 있습니다!");
            return;
        }

        Debug.Log($"[DamageItemManager] JSON에서 {settings.Data.Count}개의 아이템이 로드됨.");

        if (BombButton != null)
            BombButton.onClick.AddListener(() => UseItem(101));

        if (RocketButton != null)
            RocketButton.onClick.AddListener(() => SelectItem(102));

        if (StunButton != null)
            StunButton.onClick.AddListener(() => UseItem(103));
    }

    public void SelectItem(int itemID)
    {
        selectedItemID = itemID;
        Debug.Log($"[DamageItemManager] 아이템 {itemID} 선택됨! 화면을 터치하여 사용하세요.");
    }

    private void HandleScreenClick(Vector3 clickPosition)
    {
        if (selectedItemID == 0)
        {
            Debug.Log("[DamageItemManager] 선택된 아이템이 없습니다!");
            return;
        }

        Debug.Log($"[DamageItemManager] 아이템 {selectedItemID} 사용 시도! 클릭 위치: {clickPosition}");

        if (selectedItemID == 102) // Rocket
        {
            UseItemOnTile(clickPosition);
        }

        selectedItemID = 0;
    }

    public void UseItem(int itemID)
    {
        if (settings == null || settings.Data == null)
        {
            Debug.LogError("[DamageItemManager] JSON 데이터가 로드되지 않았습니다!");
            return;
        }

        ItemData itemData = settings.Data.Find(item => item.ItemID == itemID);

        if (itemData == null)
        {
            Debug.LogError($"[DamageItemManager] ID {itemID}에 대한 데이터가 JSON에서 발견되지 않았습니다!");
            return;
        }

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] allBosses = GameObject.FindGameObjectsWithTag("Boss");

        if ((itemID == 101 && allEnemies.Length == 0 && allBosses.Length == 0) ||
            (itemID == 103 && allBosses.Length == 0))
        {
            Debug.LogWarning($"[DamageItemManager] ID {itemID} 아이템이 사용할 대상이 없어 발동되지 않음.");
            return;
        }

        Debug.Log($"[DamageItemManager] 아이템 {itemID} 즉시 발동!");

        bool targetHit = false;

        if (itemID == 101) // Bomb
        {
            EffectManager.Instance.PlayEffect(Vector3.zero, "Bomb");

            foreach (GameObject target in allEnemies)
            {
                target.GetComponent<Enemy>()?.TakeDamage(itemData.Damage);
                targetHit = true;
            }

            foreach (GameObject boss in allBosses)
            {
                boss.GetComponent<Enemy>()?.TakeDamage(itemData.Damage);
                targetHit = true;
            }

            Debug.Log("[DamageItemManager] 폭탄 피해 적용 완료!");
        }
        else if (itemID == 103) // Stun
        {
            EffectManager.Instance.PlayEffect(Vector3.zero, "Stun");

            foreach (GameObject boss in allBosses)
            {
                boss.GetComponent<Enemy>()?.ApplyStun(itemData.Duration);
                targetHit = true;
            }

            Debug.Log("[DamageItemManager] 보스에게 스턴 적용 완료!");
        }

        if (!targetHit)
        {
            Debug.LogWarning($"[DamageItemManager] ID {itemID} 아이템이 아무 적도 맞추지 못해 발동되지 않음.");
        }
    }

    public void UseItemOnTile(Vector3 tilePosition)
    {
        if (selectedItemID == 0)
        {
            Debug.LogError("[DamageItemManager] 아이템이 선택되지 않았습니다!");
            return;
        }

        if (selectedItemID == 102) // Rocket
        {
            EffectManager.Instance.PlayEffect(tilePosition, "Rocket");

            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] allBosses = GameObject.FindGameObjectsWithTag("Boss");

            bool targetHit = false;

            foreach (GameObject target in allEnemies)
            {
                Enemy enemyComponent = target.GetComponent<Enemy>();
                if (enemyComponent != null && Mathf.Abs(enemyComponent.transform.position.y - tilePosition.y) < 0.5f)
                {
                    enemyComponent.TakeDamage(settings.Data.Find(item => item.ItemID == 102).Damage);
                    targetHit = true;
                    Debug.Log($"[DamageItemManager] {target.name}에게 로켓 피해!");
                }
            }

            foreach (GameObject boss in allBosses)
            {
                Enemy bossComponent = boss.GetComponent<Enemy>();
                if (bossComponent != null && Mathf.Abs(bossComponent.transform.position.y - tilePosition.y) < 0.5f)
                {
                    bossComponent.TakeDamage(settings.Data.Find(item => item.ItemID == 102).Damage);
                    targetHit = true;
                    Debug.Log($"[DamageItemManager] {boss.name}에게 로켓 피해!");
                }
            }

            if (!targetHit)
            {
                Debug.LogWarning($"[DamageItemManager] Rocket 아이템이 타일({tilePosition})에서 아무 적도 맞추지 못함.");
            }
        }

        selectedItemID = 0;
    }
}
