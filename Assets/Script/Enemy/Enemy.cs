using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public int EnemyID;
    public EnemySO enemyStats;
    public bool isDead = false;
    private bool isAttacking = false;
    private bool isSlowed = false;
    private bool isStunned = false;
    private float health;
    private float attackPower;
    private float originalSpeed;
    private float attackSpeed;
    public float MovementSpeed;
    public Tower currentTarget;
    public EnemyAnimatorController enemyAnimatorController;
    private Rigidbody2D rb;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 1.5f;
    private List<GameObject> bossTargets = new List<GameObject>(); // 보스는 다중 타겟 가능
    private void Start()
    {
        enemyAnimatorController = GetComponent<EnemyAnimatorController>() ?? GetComponentInChildren<EnemyAnimatorController>();

        if (enemyAnimatorController == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: EnemyAnimatorController를 찾을 수 없습니다! 애니메이션 실행 불가.");
        }
        else
        {
            Debug.Log($"[Enemy] {gameObject.name}: EnemyAnimatorController 정상 할당됨.");
            enemyAnimatorController.gameObject.SetActive(true);
        }

        enemyAnimatorController.SetWalkingState(true);
        StartCoroutine(AttackLoop());
    }

    public void Initialize(EnemySO enemyData, EnemyType type)
    {
        if (enemyData == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: enemyStats가 NULL입니다! 초기화 실패.");
            return;
        }

        enemyStats = enemyData;
        enemyStats.Type = type;

        health = enemyStats.Health;
        attackPower = enemyStats.AttackPower;
        attackSpeed = enemyStats.AttackSpeed;
        MovementSpeed = enemyStats.MovementSpeed;

        transform.localScale = new Vector3(-1, 1, 1);

        Debug.Log($"[Enemy] {gameObject.name}: 초기화 완료! 타입: {enemyStats.Type}, 체력: {health}, 공격력: {attackPower}, 이동속도: {MovementSpeed}");
    }


    private void Update()
    {
        if (!isDead && !isAttacking && !isStunned)
        {
            transform.Translate(Vector3.left * MovementSpeed * Time.deltaTime);
        }

        if (!isDead)
        {
            FindTarget();
        }
    }

    private void FindTarget()
    {
        float detectionRange = 10f;
        float maxYOffset = 0.5f; // 일반 적 탐지 Y축 허용 범위
        Collider2D[] towers = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistanceX = float.MaxValue;
        Tower closestTower = null;

        bossTargets.Clear(); // 보스 타겟 리스트 초기화

        foreach (var tower in towers)
        {
            if (tower.CompareTag("Tower"))
            {
                float xDifference = Mathf.Abs(tower.transform.position.x - transform.position.x);
                float yDifference = Mathf.Abs(tower.transform.position.y - transform.position.y);
                Tower towerComponent = tower.GetComponent<Tower>(); // Tower 컴포넌트 가져오기

                if (towerComponent == null)
                    continue;

                if (tower.CompareTag("Boss"))
                {
                    // 보스는 Y축 관계없이 모든 감지된 타워 저장
                    bossTargets.Add(tower.gameObject);
                }
                else
                {
                    // 일반 타워는 X축 기준 가장 가까운 대상만 선택 (Y축 오차 허용)
                    if (xDifference < detectionRange && yDifference <= maxYOffset && xDifference < closestDistanceX)
                    {
                        closestDistanceX = xDifference;
                        closestTower = towerComponent; // Tower 타입으로 저장
                    }
                }
            }
        }

        if (bossTargets.Count > 0)
        {
            Debug.Log($"[Enemy] 보스가 {bossTargets.Count}개의 타워를 감지함.");
        }

        // 일반 타워 타겟 설정
        currentTarget = closestTower;
    }
    private IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            if (isStunned)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            if (CompareTag("Boss") && bossTargets.Count > 0)
            {
                foreach (GameObject target in bossTargets)
                {
                    if (target != null)
                    {
                        AttackTarget(target);
                    }
                }
            }
            else if (currentTarget != null)
            {
                AttackTarget(currentTarget.gameObject);
            }

            yield return new WaitForSeconds(attackSpeed);
        }
    }

    private void AttackTarget(GameObject target)
    {
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        if (enemyStats.Type == EnemyType.Melee && distanceToTarget < 1.0f)
        {
            StartMeleeAttack(target);
        }
        else if (enemyStats.Type == EnemyType.Ranged && distanceToTarget <= attackRange)
        {
            StartRangedAttack(target);
        }
    }

    private void StartMeleeAttack(GameObject target)
    {
        if (isAttacking) return;

        isAttacking = true;
        MovementSpeed = 0;
        enemyAnimatorController.SetWalkingState(false);
        enemyAnimatorController.SetAttackState(true);

        StartCoroutine(MeleeAttackRoutine());
    }
    private IEnumerator MeleeAttackRoutine()
    {
        yield return new WaitForSeconds(enemyStats.AttackSpeed);

        if (CompareTag("Boss") && bossTargets.Count > 0)
        {
            foreach (GameObject target in bossTargets)
            {
                AttackTarget(target);
            }
        }
        else if (currentTarget != null)
        {
            AttackTarget(currentTarget.gameObject);
        }

        isAttacking = false;
        enemyAnimatorController.SetWalkingState(true);
        enemyAnimatorController.SetAttackState(false);
    }


    private void StartRangedAttack(GameObject target)
    {
        if (isAttacking) return;

        isAttacking = true;
        enemyAnimatorController.SetAttackState(true);

        StartCoroutine(RangedAttackRoutine());
    }

    private IEnumerator RangedAttackRoutine()
    {
        yield return new WaitForSeconds(enemyStats.AttackSpeed);

        if (CompareTag("Boss"))
        {
            Debug.Log("[Enemy] 보스는 원거리 공격을 하지 않습니다.");
        }
        else if (currentTarget != null)
        {
            FireProjectile(currentTarget.gameObject);
        }

        isAttacking = false;
        enemyAnimatorController.SetWalkingState(true);
        enemyAnimatorController.SetAttackState(false);
    }

    private void FireProjectile(GameObject target)
    {
        if (target == null || projectilePrefab == null || firePoint == null) return;

        Vector3 shootDirection = (target.transform.position - firePoint.position).normalized;

        GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectileObj.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(this, shootDirection);
            Debug.Log($"[Enemy] {gameObject.name}가 {target.name}에게 투사체 발사!");
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndLine"))
        {
            Debug.Log($"[Enemy] {gameObject.name}: EndLine에 도달! 게임 종료.");

            // **GameManager를 직접 찾음**
            GameManager gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();

            // **GameManager가 존재하는 경우만 GameOver 호출**
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
            else
            {
                Debug.LogError("[Enemy] GameManager 인스턴스를 찾을 수 없습니다!");
            }
        }
    }



    public void ApplySlow(float slowFactor, float duration)
    {
        if (!isSlowed)
        {
            originalSpeed = MovementSpeed;
            float adjustedSlowFactor = Mathf.Clamp(1f - slowFactor, 0.1f, 1f);
            MovementSpeed *= adjustedSlowFactor;
            attackSpeed /= adjustedSlowFactor;
            isSlowed = true;
        }
        Invoke(nameof(EndSlow), duration);
    }

    private void EndSlow()
    {
        MovementSpeed = originalSpeed;
        attackSpeed = enemyStats.AttackSpeed;
        isSlowed = false;
    }

    public void ApplyStun(float duration)
    {
        if (isDead || isStunned) return;

        isStunned = true;
        MovementSpeed = 0f;
        attackSpeed = 0f;
        isAttacking = false;

        if (enemyAnimatorController != null)
        {
            enemyAnimatorController.SetWalkingState(false);
            enemyAnimatorController.SetAttackState(false);
        }

        StopAllCoroutines(); // **모든 코루틴 중단 (특히 AttackLoop)**

        Debug.Log($"[Enemy] {gameObject.name}: 스턴 적용 - {duration}초 동안 행동 불가");

        // **스턴 해제 예약**
        StartCoroutine(StunDuration(duration));
    }

    private IEnumerator StunDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        EndStun();
    }

    private void EndStun()
    {
        if (isDead) return; // **이미 죽었다면 스턴 해제 불필요**

        isStunned = false;
        MovementSpeed = originalSpeed;
        attackSpeed = enemyStats.AttackSpeed;

        if (enemyAnimatorController != null)
        {
            enemyAnimatorController.SetWalkingState(true);
            enemyAnimatorController.SetAttackState(false);
        }

        Debug.Log($"[Enemy] {gameObject.name}: 스턴 해제됨");

        // **현재 타겟이 존재할 경우 AttackLoop 다시 실행**
        if (currentTarget != null && !isAttacking)
        {
            Debug.Log($"[Enemy] {gameObject.name}: 스턴 해제 후 공격 재개");
            StartCoroutine(AttackLoop());
        }
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
        enemyAnimatorController.SetWalkingState(false);
        enemyAnimatorController.PlayDeathAnimation();

        // WaveManager에 적 처치 알림
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.Clear(this); // 적 객체 전달
        }
        // **골드 지급 기능 추가**
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(enemyStats.RewardMoney, true); // 적의 보상금 지급
        }

        Destroy(gameObject, 1.5f); // 애니메이션 후 제거
    }


    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
}
