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
    private Coroutine stunCoroutine; // 현재 실행 중인 스턴 코루틴 저장
    private bool isStunImmune = false; // 스턴 면역 여부


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
        float detectionRange = 3f;
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

            if (!isAttacking) // 공격 중이 아닐 때만 실행
            {
                if (CompareTag("Boss") && bossTargets.Count > 0)
                {
                    foreach (GameObject target in bossTargets)
                    {
                        if (target != null && IsTargetInRange(target)) // 거리 검사 추가
                        {
                            StartMeleeAttack(target);
                        }
                    }
                }
                else if (currentTarget != null && IsTargetInRange(currentTarget.gameObject)) // 거리 검사 추가
                {
                    StartMeleeAttack(currentTarget.gameObject);
                }
                else
                {
                    FindTarget(); // 새로운 타겟을 찾도록 추가
                }
            }

            yield return new WaitForSeconds(0.1f); // 다음 루프까지 대기
        }
    }
    private bool IsTargetInRange(GameObject target)
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        return distanceToTarget <= attackRange; // 공격 범위 내에 있는지 확인
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
        if (isAttacking) return; // 이미 공격 중이면 실행하지 않음

        isAttacking = true;
        MovementSpeed = 0;
        enemyAnimatorController.SetWalkingState(false);
        enemyAnimatorController.SetAttackState(true);

        StartCoroutine(MeleeAttackRoutine(target));
    }


    private IEnumerator MeleeAttackRoutine(GameObject target)
    {
        yield return new WaitForSeconds(enemyStats.AttackSpeed);

        if (CompareTag("Boss") && bossTargets.Count > 0)
        {
            Debug.Log($"[Boss] {bossTargets.Count}개의 타겟을 공격합니다.");
            foreach (GameObject bossTarget in bossTargets)
            {
                if (bossTarget != null)
                {
                    AttackTarget(bossTarget);
                }
            }
        }
        else if (target != null)
        {
            Debug.Log($"[Enemy] 일반 적이 {target.name}을 공격합니다.");
            AttackTarget(target);
        }
        else if (currentTarget != null)
        {
            Debug.Log($"[Enemy] currentTarget {currentTarget.name}을(를) 공격합니다.");
            AttackTarget(currentTarget.gameObject);
        }
        else
        {
            Debug.LogWarning("[Enemy] 공격할 대상이 없습니다.");
        }

        yield return new WaitForSeconds(0.2f); // 공격 간격 추가

        isAttacking = false; // 공격 종료 후 다시 공격 가능하도록 설정
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
        if (isDead) return; // 이미 죽은 상태라면 스턴 적용하지 않음
        if (CompareTag("Boss")) return; // 보스는 스턴에 면역
        if (isStunned || isStunImmune) return; // 이미 스턴 중이거나 스턴 면역 상태라면 적용 안 함

        Debug.Log($"[Enemy] {gameObject.name}: 스턴 적용 - {duration}초 동안 행동 불가");

        isStunned = true;
        MovementSpeed = 0f;
        isAttacking = false;

        if (enemyAnimatorController != null)
        {
            enemyAnimatorController.SetWalkingState(false);
            enemyAnimatorController.SetAttackState(false);
        }

        // 기존 스턴이 걸려있는 경우, 새로운 스턴 적용 안 함
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        // 새로운 스턴 코루틴 시작
        stunCoroutine = StartCoroutine(StunDuration(duration));
    }

    private IEnumerator StunDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        isStunned = false;
        MovementSpeed = enemyStats.MovementSpeed; // 원래 이동 속도로 복구
        isAttacking = false;

        if (enemyAnimatorController != null)
        {
            enemyAnimatorController.SetWalkingState(true);
        }

        Debug.Log($"[Enemy] {gameObject.name}: 스턴 해제됨");

        // **스턴 해제 후 5초간 스턴 면역 적용**
        StartCoroutine(StunImmunityCooldown(2f));
    }

    private IEnumerator StunImmunityCooldown(float immunityDuration)
    {
        isStunImmune = true;
        Debug.Log($"[Enemy] {gameObject.name}: 스턴 면역 시작 ({immunityDuration}초)");

        yield return new WaitForSeconds(immunityDuration);

        isStunImmune = false;
        Debug.Log($"[Enemy] {gameObject.name}: 스턴 면역 종료");
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
