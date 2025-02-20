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

    private void Start()
    {
        enemyAnimatorController = GetComponent<EnemyAnimatorController>();

        if (enemyAnimatorController == null)
        {
            enemyAnimatorController = GetComponentInChildren<EnemyAnimatorController>();
        }

        if (enemyAnimatorController == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: EnemyAnimatorController를 찾을 수 없습니다! 애니메이션 실행 불가.");
        }
        else
        {
            Debug.Log($"[Enemy] {gameObject.name}: EnemyAnimatorController 정상 할당됨.");
            enemyAnimatorController.gameObject.SetActive(true);
        }
    }



    public void Initialize(EnemySO enemyData)
    {
        enemyStats = enemyData;

        if (enemyStats != null)
        {
            health = enemyStats.Health;
            attackPower = enemyStats.AttackPower;
            attackSpeed = enemyStats.AttackSpeed;
            MovementSpeed = enemyStats.MovementSpeed;

            transform.localScale = new Vector3(-1, 1, 1);

            Debug.Log($"[Enemy] {gameObject.name}: 초기화 완료! 체력: {health}, 공격력: {attackPower}, 이동속도: {MovementSpeed}");
        }
        else
        {
            Debug.LogError($"[Enemy] {gameObject.name}: enemyStats가 NULL입니다! 초기화 실패.");
        }
    }


    private void Update()
    {
        if (!isDead && !isAttacking && !isStunned)
        {
            transform.Translate(Vector3.left * MovementSpeed * Time.deltaTime);
        }
        else if (isAttacking)
        {
            // 공격 중일 때 이동 멈춤
            transform.Translate(Vector3.zero);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Enemy] {gameObject.name} - 충돌 감지: {collision.gameObject.name} ({collision.tag})");

        if (collision.CompareTag("Tower"))
        {
            float xDifference = Mathf.Abs(collision.transform.position.x - transform.position.x);
            Debug.Log($"[Enemy] {gameObject.name} - 감지된 타워와 X축 차이: {xDifference}");

            if (xDifference < 0.5f) // 감지 범위 확장
            {
                if (currentTarget == null || currentTarget.isDead) // 기존 타겟이 죽었을 때 새로 감지
                {
                    currentTarget = collision.GetComponent<Tower>();
                    Debug.Log($"[Enemy] {gameObject.name} - 타워 감지: {currentTarget.name}, 공격 시작");
                    StartCoroutine(AttackLoop());
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            if (currentTarget == null || currentTarget.isDead)
            {
                currentTarget = collision.GetComponent<Tower>();
                Debug.Log($"[Enemy] {gameObject.name} - 타워 지속 감지: {currentTarget.name}, 공격 시작");
                StartCoroutine(AttackLoop());
            }
        }
    }
        private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == currentTarget)
        {
            Debug.Log($"[Enemy] {gameObject.name} - {currentTarget.name}에서 벗어남, 1초 후 재확인");
            StartCoroutine(ResetTargetAfterDelay(0.1f));
        }
    }

    private IEnumerator ResetTargetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentTarget != null)
        {
            currentTarget = null;
            isAttacking = false;
            enemyAnimatorController.SetAttackState(false);
            Debug.Log($"[Enemy] {gameObject.name} - 공격 중지, 이동 재개");
        }
    }
    private IEnumerator AttackLoop()
    {
        if (isAttacking || isStunned) yield break;
        isAttacking = true;

        if (enemyAnimatorController == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: AttackLoop 시작 전 enemyAnimatorController가 NULL!");
            yield break;
        }

        enemyAnimatorController.SetAttackState(true);
        Debug.Log($"[Enemy] {gameObject.name}: 공격 애니메이션 실행!");

        float waitTime = 0f;
        while (!enemyAnimatorController.IsPlayingAttackAnimation() && waitTime < 1.0f) // 최대 1초 대기
        {
            yield return null;
            waitTime += Time.deltaTime;
        }

        if (!enemyAnimatorController.IsPlayingAttackAnimation())
        {
            Debug.LogError($"[Enemy] {gameObject.name}: 공격 애니메이션 실행 실패!");
            isAttacking = false;
            yield break;
        }

        yield return new WaitForSeconds(attackSpeed);

        // 타겟이 살아있으면 다시 공격 실행
        if (currentTarget != null && !currentTarget.isDead)
        {
            StartCoroutine(AttackLoop());
        }
    }

    public void StartAttack()
    {
        if (!isAttacking && currentTarget != null && !isDead)
        {
            StartCoroutine(AttackLoop());
        }
    }

    public void StopAttack()
    {
        if (!isDead)
        {
            isAttacking = false;
            currentTarget = null;
            enemyAnimatorController.SetAttackState(false);
            Debug.Log($"[Enemy] {gameObject.name}: 공격 중지, 이동 재개");
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

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (enemyAnimatorController == null)
        {
            Debug.LogWarning($"[Enemy] {gameObject.name}: enemyAnimatorController가 NULL! 강제 할당 시도.");
            enemyAnimatorController = GetComponentInChildren<EnemyAnimatorController>();

            if (enemyAnimatorController == null)
            {
                Debug.LogError($"[Enemy] {gameObject.name}: 사망 애니메이션 실행 실패! EnemyAnimatorController를 찾을 수 없습니다.");
                Destroy(gameObject);
                return;
            }
        }

        enemyAnimatorController.PlayDeathAnimation();
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
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

        StopCoroutine(AttackLoop());
        enemyAnimatorController.SetAttackState(false);

        Invoke(nameof(EndStun), duration);
    }

    private void EndStun()
    {
        isStunned = false;
        MovementSpeed = originalSpeed;
        attackSpeed = enemyStats.AttackSpeed;

        if (currentTarget != null)
        {
            StartCoroutine(AttackLoop());
        }
    }
}
