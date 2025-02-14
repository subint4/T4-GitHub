using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int TowerID;  // **SO와 연결될 ID (프리팹에 직접 설정 가능)**
    private int currentLevel;

    public TowerSO towerStats;
    public Tiles currentTile;
    public TowerAnimatorController towerAnimatorController;
    public bool isDead = false;
    private float health;
    private GameObject currentTarget; // 현재 공격 대상 저장

    private void Awake()
    {
        AssignTowerSO(); // 자동 SO 연결
        if (towerStats == null)
        {
            Debug.LogError($"TowerStats가 {gameObject.name}에서 할당되지 않았습니다! 프리팹을 확인하세요.");
        }
        else
        {
            Debug.Log($"{gameObject.name}의 TowerStats가 올바르게 설정됨. Tower ID: {towerStats.ID}");
        }
    }

    private void AssignTowerSO()
    {
        if (TowerID <= 0)
        {
            Debug.LogError($"{gameObject.name}의 TowerID가 올바르지 않습니다! (현재 ID: {TowerID})");
            return;
        }

        towerStats = DataManager.GetTowerData(TowerID);

        if (towerStats == null)
        {
            Debug.LogError($"{gameObject.name}에서 TowerID {TowerID}에 해당하는 SO를 찾을 수 없습니다!");
        }
        else
        {
            Debug.Log($"{gameObject.name}에 TowerSO({towerStats.Name})가 정상 할당됨 (ID: {TowerID})");
        }
    }
    private void Start()
    {
        if (towerStats != null)
        {
            health = towerStats.Health;
        }
    }

    private void Update()
    {
        if (!isDead)
        {
            FindTarget();
        }
    }

    private void FindTarget()
    {
        float detectionRange = 1000f; // 탐지 거리
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                currentTarget = enemy.gameObject;
                if (towerAnimatorController != null)
                {
                    towerAnimatorController.SetAttackState(true);
                }
                return;
            }
        }

        currentTarget = null;
        if (towerAnimatorController != null)
        {
            towerAnimatorController.SetAttackState(false);
        }
    }
    // 타워 클릭 시 업그레이드 UI 활성화
    private void OnMouseDown()
    {
        if (UpgradeUI.Instance != null)
        {
            UpgradeUI.Instance.OpenUpgradeUI(this);
        }
        else
        {
            Debug.LogError("UpgradeUI 인스턴스를 찾을 수 없습니다!");
        }
    }

    // 업그레이드 실행
    public void UpgradeTower(TowerSO newStats)
    {
        if (newStats == null)
        {
            Debug.LogError("새로운 타워 데이터가 없습니다!");
            return;
        }

        towerStats = newStats;
        transform.localScale *= 1.3f; // 30% 크기 증가
        Debug.Log($"{towerStats.Name} 업그레이드 완료! 새로운 공격력: {towerStats.AttackPower}");
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (towerAnimatorController != null)
        {
            towerAnimatorController.PlayDeathAnimation();
        }
    }

    public void Attack()
    {
        if (currentTarget != null)
        {
            Enemy enemy = currentTarget.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(towerStats.AttackPower);
            }
        }
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

}
