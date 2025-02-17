using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int TowerID;
    private int currentLevel;

    public TowerSO towerStats;
    public Tiles currentTile;
    public TowerAnimatorController towerAnimatorController;
    public bool isDead = false;
    private float health;
    private GameObject currentTarget;
    private bool isAttacking = false;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private void Awake()
    {
        AssignTowerSO();
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
        StartCoroutine(AttackLoop());
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
        float detectionRange = 1000f;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                currentTarget = enemy.gameObject;
                return;
            }
        }
        currentTarget = null;
    }

    private IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            if (currentTarget != null)
            {
                if (!isAttacking)
                {
                    isAttacking = true;

                    // 애니메이션을 강제로 다시 실행
                    towerAnimatorController.SetAttackState(false);
                    yield return new WaitForSeconds(0.1f);
                    towerAnimatorController.SetAttackState(true);

                    yield return new WaitUntil(() => towerAnimatorController.IsPlayingAttackAnimation());
                    yield return new WaitForSeconds(0.1f); // 애니메이션 실행 대기
                    FireProjectile();
                    yield return new WaitForSeconds(towerStats.AttackSpeed);
                    isAttacking = false;
                    towerAnimatorController.SetAttackState(false);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    public void FireProjectile()
    {
        if (currentTarget != null && projectilePrefab != null && firePoint != null)
        {
            Debug.Log($"[Tower] {gameObject.name}: 투사체 발사!");
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDamage(towerStats.AttackPower);
            }
        }
        else
        {
            Debug.LogError($"[Tower] {gameObject.name}: 투사체 발사 실패! currentTarget: {currentTarget}, projectilePrefab: {projectilePrefab}, firePoint: {firePoint}");
        }
    }
    public void RestartAttack()
    {
        if (!isDead)
        {
            Debug.Log($"[Tower] {gameObject.name}: 공격 재개");
            StopCoroutine(AttackLoop()); // 기존 AttackLoop 중지
            StartCoroutine(AttackLoop()); // 다시 실행

            // 공격 애니메이션을 강제로 다시 실행
            towerAnimatorController.SetAttackState(false);
            towerAnimatorController.SetAttackState(true);
        }
    }


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

    public void UpgradeTower(TowerSO newStats)
    {
        if (newStats == null)
        {
            Debug.LogError("새로운 타워 데이터가 없습니다!");
            return;
        }

        towerStats = newStats;
        transform.localScale *= 1.3f;
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



    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
}
