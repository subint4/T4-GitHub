using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour, IPointerClickHandler
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
        float detectionRange = 1000f; // 감지 거리
        float enemyMaxYDifference = 0.5f; // 일반 적 감지 Y축 차이 허용 범위
        float bossMaxYDifference = 0.5f; // 보스 감지 Y축 차이 허용 범위
        Vector2 direction = Vector2.right; // 기본 감지 방향

        // 기본 오른쪽 방향 감지
        RaycastHit2D[] hitEnemies = Physics2D.RaycastAll(transform.position, direction, detectionRange);

        // 보스 감지를 위한 추가 레이케스트 (위쪽, 아래쪽)
        RaycastHit2D[] hitBossesUp = Physics2D.RaycastAll(transform.position + Vector3.up * bossMaxYDifference, direction, detectionRange);
        RaycastHit2D[] hitBossesDown = Physics2D.RaycastAll(transform.position + Vector3.down * bossMaxYDifference, direction, detectionRange);

        List<RaycastHit2D> totalHits = new List<RaycastHit2D>();
        totalHits.AddRange(hitEnemies);
        totalHits.AddRange(hitBossesUp);
        totalHits.AddRange(hitBossesDown);

        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;

        foreach (var hit in totalHits)
        {
            Collider2D enemy = hit.collider;

            if (enemy.CompareTag("Enemy") || enemy.CompareTag("Boss")) // "Enemy" 또는 "Boss" 태그가 있는 적만 감지
            {
                float yDifference = Mathf.Abs(enemy.transform.position.y - transform.position.y);
                float maxYDifference = enemy.CompareTag("Boss") ? bossMaxYDifference : enemyMaxYDifference;

                // 보스는 확장된 감지 범위를 사용하므로 별도 체크 필요 없음
                if (!enemy.CompareTag("Boss") && yDifference > maxYDifference) continue;

                float distance = Vector2.Distance(transform.position, enemy.transform.position);

                // 가장 가까운 적을 타겟으로 설정
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.gameObject;
                }
            }
        }

        currentTarget = closestEnemy;

        if (currentTarget != null)
        {
            Debug.Log($"[Tower] 타겟 변경: {currentTarget.name} (Y축 차이 허용: {(currentTarget.CompareTag("Boss") ? bossMaxYDifference : enemyMaxYDifference)})");
        }
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
            GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Initialize(
                    attackDamage: towerStats.AttackPower,
                    direction: (currentTarget.transform.position - firePoint.position).normalized
                );

                Debug.Log($"[Tower] {gameObject.name}가 {currentTarget.name}에게 투사체 발사!");
            }
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"타워 클릭됨: {gameObject.name}");

        if (!isDead && UpgradeUI.Instance != null)
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
        transform.localScale *= 1.1f;
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
        currentTile.isOccupied = false;
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
