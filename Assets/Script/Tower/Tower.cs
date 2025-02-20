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

    private void Start()
    {
        if (towerStats != null)
        {
            health = towerStats.Health;
        }
        StartCoroutine(AttackLoop());
    }

    public void Initialize(TowerSO towerData)
    {
        towerStats = towerData;
        if (towerStats != null)
        {
            health = towerStats.Health;
            Debug.Log($"[Tower] {gameObject.name}: {towerStats.Name} 데이터로 초기화 완료!");
        }
        else
        {
            Debug.LogError($"[Tower] {gameObject.name}: 타워 데이터가 NULL입니다! 초기화 실패.");
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
        float detectionRange = 100000f; // 감지 범위 설정
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                float xDifference = Mathf.Abs(enemy.transform.position.x - transform.position.x);
                float yDifference = Mathf.Abs(enemy.transform.position.y - transform.position.y);

                // X좌표가 일정 범위 내 && 같은 Y축(같은 가로줄)
                if (xDifference < detectionRange && yDifference < 0.5f)
                {
                    float distance = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy.gameObject;
                    }
                }
            }
        }

        // 가장 가까운 적을 타겟으로 설정
        currentTarget = closestEnemy;
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

                    // 공격 애니메이션 실행
                    towerAnimatorController.SetAttackState(true);
                    Debug.Log($"[Tower] {gameObject.name}: 공격 애니메이션 실행!");

                    // 공격 속도에 맞춰 대기 후 다음 공격
                    yield return new WaitForSeconds(towerStats.AttackSpeed);

                    // 애니메이션 종료 후 Idle 전환
                    towerAnimatorController.SetAttackState(false);

                    // 다음 공격을 위해 약간의 대기 후 초기화
                    yield return new WaitForSeconds(0.1f);
                    isAttacking = false;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // 애니메이션 이벤트에서 호출될 함수
    public void FireProjectile()
    {
        if (currentTarget != null && projectilePrefab != null && firePoint != null)
        {
            Debug.Log($"[Tower] {gameObject.name}: 애니메이션과 동기화된 투사체 발사!");

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
            StopCoroutine(AttackLoop());
            StartCoroutine(AttackLoop());
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

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        towerAnimatorController?.PlayDeathAnimation();
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
    public bool IsDestroyed()
    {
        return isDead;
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        towerAnimatorController.SetAttackState(false);
    }
}
