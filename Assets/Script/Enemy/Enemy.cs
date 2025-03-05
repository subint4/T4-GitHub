using System.Collections;
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
        float maxYOffset = 0.5f;
        Collider2D[] towers = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistanceX = float.MaxValue;
        Tower closestTower = null;

        foreach (var tower in towers)
        {
            if (tower.CompareTag("Tower"))
            {
                float xDifference = Mathf.Abs(tower.transform.position.x - transform.position.x);
                float yDifference = Mathf.Abs(tower.transform.position.y - transform.position.y);

                if (xDifference < detectionRange && yDifference <= maxYOffset && xDifference < closestDistanceX)
                {
                    closestDistanceX = xDifference;
                    closestTower = tower.GetComponent<Tower>();
                }
            }
        }

        currentTarget = closestTower;
    }

    private IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            if (currentTarget != null && !currentTarget.isDead && !isStunned)
            {
                float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);

                if (enemyStats.Type == EnemyType.Melee && distanceToTarget < 1.0f)
                {
                    StartMeleeAttack();
                }
                else if (enemyStats.Type == EnemyType.Ranged && distanceToTarget <= attackRange)
                {
                    StartRangedAttack();
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void StartMeleeAttack()
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

        if (currentTarget != null)
        {
            currentTarget.TakeDamage(enemyStats.AttackPower);
        }

        isAttacking = false;
        MovementSpeed = enemyStats.MovementSpeed;
        enemyAnimatorController.SetWalkingState(true);
        enemyAnimatorController.SetAttackState(false);
    }

    private void StartRangedAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        enemyAnimatorController.SetWalkingState(false);
        enemyAnimatorController.SetAttackState(true);
        StartCoroutine(RangedAttackRoutine());
    }

    private IEnumerator RangedAttackRoutine()
    {
        yield return new WaitForSeconds(enemyStats.AttackSpeed);

        if (currentTarget != null && projectilePrefab != null && firePoint != null)
        {
            Vector3 shootDirection = -transform.right;
            GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectileScript = projectileObj.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.Initialize(this,shootDirection); // Projectile이 적의 공격력을 자동으로 설정
            }
        }

        isAttacking = false;
        enemyAnimatorController.SetWalkingState(true);
        enemyAnimatorController.SetAttackState(false);
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
