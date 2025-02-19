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
        float detectionRange = 1000f;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy") && enemy.transform.position.x > transform.position.x)
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
                    towerAnimatorController.SetAttackState(true);

                    Debug.Log($"[Tower] {gameObject.name}: 공격 애니메이션 실행 요청!");

                    // 공격 애니메이션이 실행될 때까지 대기
                    yield return new WaitUntil(() => towerAnimatorController.IsPlayingAttackAnimation());

                    Debug.Log($"[Tower] {gameObject.name}: 공격 애니메이션 실행됨!");

                    yield return new WaitForSeconds(0.1f);

                    FireProjectile();
                    Debug.Log($"[Tower] {gameObject.name}: 투사체 발사!");

                    yield return new WaitForSeconds(towerStats.AttackSpeed);

                    isAttacking = false;
                    towerAnimatorController.SetAttackState(false);
                    Debug.Log($"[Tower] {gameObject.name}: 공격 종료, 다음 공격 준비.");
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void FireProjectile()
    {
        if (currentTarget == null)
        {
            Debug.LogError($"[Tower] {gameObject.name}: 투사체 발사 실패! 타겟이 NULL입니다.");
            return;
        }

        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError($"[Tower] {gameObject.name}: 투사체 프리팹 또는 발사 위치가 설정되지 않음.");
            return;
        }

        Debug.Log($"[Tower] {gameObject.name}: 투사체 발사 - 타겟: {currentTarget.name}");
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDamage(towerStats.AttackPower);
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

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        towerAnimatorController.SetAttackState(false);
    }
}
