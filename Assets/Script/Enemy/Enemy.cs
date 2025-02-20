using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int EnemyID;
    public EnemySO enemyStats;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    public bool IsAttacking => isAttacking;
    public float MovementSpeed { get; private set; }

    public Tower currentTarget;
    public bool isSlowed = false;
    public bool isStunned = false;
    public bool isAttacking = false;

    private float currentHealth;
    private bool isDead = false;
    private Rigidbody2D enemyRigidbody;
    private float attackSpeed;
    private float originalSpeed;
    private EnemyAnimatorController enemyAnimatorController;

    private void Start()
    {
        enemyAnimatorController = GetComponentInChildren<EnemyAnimatorController>();

        if (enemyAnimatorController == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: EnemyAnimatorController를 찾을 수 없습니다! 애니메이션 실행 불가.");
        }
        else
        {
            Debug.Log($"[Enemy] {gameObject.name}: EnemyAnimatorController 정상 할당됨.");
        }

        transform.localScale = new Vector3(-1, 1, 1); // 왼쪽 바라보기
    }

    public void Initialize(EnemySO stats)
    {
        enemyStats = stats;
        if (enemyStats != null)
        {
            currentHealth = enemyStats.Health;
            MovementSpeed = enemyStats.MovementSpeed;
            attackSpeed = enemyStats.AttackSpeed;
        }
        else
        {
            Debug.LogError($"[Enemy] {gameObject.name}: enemyStats가 NULL입니다! 데이터 로딩 실패!");
        }
    }

    private void Update()
    {
        if (!isDead && !isAttacking && !isStunned)
        {
            transform.Translate(Vector3.left * MovementSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower") && currentTarget == null)
        {
            currentTarget = collision.GetComponent<Tower>();
            if (currentTarget != null && !isAttacking)
            {
                Debug.Log($"[Enemy] {gameObject.name}: 타워 충돌 -> {currentTarget.name} 공격 시작");
                StartCoroutine(AttackLoop());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower") && currentTarget != null && collision.gameObject == currentTarget.gameObject)
        {
            Debug.Log($"[Enemy] {gameObject.name}: {currentTarget.name} 벗어남, 이동 재개");
            StopAttack();
            currentTarget = null;
        }
    }
    private IEnumerator AttackLoop()
    {
        while (!isDead && currentTarget != null)
        {
            if (currentTarget.IsDestroyed())
            {
                Debug.Log($"[Enemy] {gameObject.name}: 타겟이 파괴됨, 이동 재개");
                StopAttack();
                yield break;
            }

            isAttacking = true;

            if (enemyAnimatorController != null)
            {
                enemyAnimatorController.SetAttackState(true);
                Debug.Log($"[Enemy] {gameObject.name}: 공격 애니메이션 실행");
            }

            float elapsedTime = 0f;
            while (!enemyAnimatorController.IsPlayingAttackAnimation() && elapsedTime < 1.0f)
            {
                yield return new WaitForSeconds(0.1f);
                elapsedTime += 0.1f;
            }

            if (!enemyAnimatorController.IsPlayingAttackAnimation())
            {
                Debug.LogError($"[Enemy] {gameObject.name}: 공격 애니메이션 실행 실패!");
            }

            Debug.Log($"[Enemy] {gameObject.name}: {currentTarget.name}을(를) 공격! 공격력: {enemyStats.AttackPower}");
            currentTarget.TakeDamage(enemyStats.AttackPower);

            yield return new WaitForSeconds(attackSpeed);

            if (enemyAnimatorController != null)
            {
                enemyAnimatorController.SetAttackState(false);
            }

            yield return new WaitUntil(() => !enemyAnimatorController.IsPlayingAttackAnimation());

            isAttacking = false;
            Debug.Log($"[Enemy] {gameObject.name}: 공격 종료, 다음 공격 준비");

            if (currentTarget != null && !currentTarget.IsDestroyed())
            {
                StartCoroutine(AttackLoop());
            }
            else
            {
                StopAttack();
            }
        }
    }

    public void StopAttack()
    {
        isAttacking = false;
        currentTarget = null;
    }

public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log($"[Enemy] {gameObject.name}: 사망!");

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyDefeated();
        }

        Destroy(gameObject);
    }


public void ApplySlow(float slowFactor, float duration)
    {
        if (!isSlowed)
        {
            originalSpeed = MovementSpeed;
            float adjustedSlowFactor = Mathf.Clamp(1f - slowFactor, 0.1f, 1f);
            MovementSpeed = Mathf.Max(MovementSpeed * adjustedSlowFactor, 0.1f);
            attackSpeed /= adjustedSlowFactor;
            isSlowed = true;
        }
        Invoke("EndSlow", duration);
    }

    public void EndSlow()
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
        if (enemyRigidbody != null)
        {
            enemyRigidbody.velocity = Vector2.zero;
        }

        Invoke("EndStun", duration);
    }

    public void EndStun()
    {
        isStunned = false;
        MovementSpeed = originalSpeed;
        attackSpeed = enemyStats.AttackSpeed;
    }

    public void SetMovementSpeed(float speed)
    {
        MovementSpeed = speed;
    }
}
