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
            Debug.LogError($"[Enemy] {gameObject.name}: EnemyAnimatorController�� ã�� �� �����ϴ�! �ִϸ��̼� ���� �Ұ�.");
        }
        else
        {
            Debug.Log($"[Enemy] {gameObject.name}: EnemyAnimatorController ���� �Ҵ��.");
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

            Debug.Log($"[Enemy] {gameObject.name}: �ʱ�ȭ �Ϸ�! ü��: {health}, ���ݷ�: {attackPower}, �̵��ӵ�: {MovementSpeed}");
        }
        else
        {
            Debug.LogError($"[Enemy] {gameObject.name}: enemyStats�� NULL�Դϴ�! �ʱ�ȭ ����.");
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
            // ���� ���� �� �̵� ����
            transform.Translate(Vector3.zero);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Enemy] {gameObject.name} - �浹 ����: {collision.gameObject.name} ({collision.tag})");

        if (collision.CompareTag("Tower"))
        {
            float xDifference = Mathf.Abs(collision.transform.position.x - transform.position.x);
            Debug.Log($"[Enemy] {gameObject.name} - ������ Ÿ���� X�� ����: {xDifference}");

            if (xDifference < 0.5f) // ���� ���� Ȯ��
            {
                if (currentTarget == null || currentTarget.isDead) // ���� Ÿ���� �׾��� �� ���� ����
                {
                    currentTarget = collision.GetComponent<Tower>();
                    Debug.Log($"[Enemy] {gameObject.name} - Ÿ�� ����: {currentTarget.name}, ���� ����");
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

                if (!isAttacking)
                {
                    StartCoroutine(AttackLoop());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == currentTarget)
        {
            Debug.Log($"[Enemy] {gameObject.name} - {currentTarget.name}에서 벗어남, 공격 중단");
            StopAttack();
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
            Debug.Log($"[Enemy] {gameObject.name} - ���� ����, �̵� �簳");
        }
    }
    private IEnumerator AttackLoop()
    {
        if (isAttacking || isStunned || currentTarget == null || isDead) yield break;

        isAttacking = true;
        enemyAnimatorController.SetAttackState(true);
        Debug.Log($"[Enemy] {gameObject.name}: 공격 애니메이션 실행!");

        while (currentTarget != null && !isDead && !isStunned)
        {
            if (!enemyAnimatorController.IsPlayingAttackAnimation())
            {
                Debug.LogError($"[Enemy] {gameObject.name}: 공격 애니메이션 실행 실패!");
                break;
            }

            yield return new WaitForSeconds(attackSpeed);

            if (currentTarget != null && !currentTarget.isDead)
            {
                Debug.Log($"[Enemy] {gameObject.name}: {currentTarget.name}에게 {enemyStats.AttackPower} 피해!");
                currentTarget.TakeDamage(enemyStats.AttackPower);
            }
            else
            {
                Debug.Log($"[Enemy] {gameObject.name}: 타겟이 죽어서 공격 중지!");
                StopAttack();
                yield break;
            }
        }

        isAttacking = false;
        enemyAnimatorController.SetAttackState(false);
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
            Debug.LogWarning($"[Enemy] {gameObject.name}: enemyAnimatorController�� NULL! ���� �Ҵ� �õ�.");
            enemyAnimatorController = GetComponentInChildren<EnemyAnimatorController>();

            if (enemyAnimatorController == null)
            {
                Debug.LogError($"[Enemy] {gameObject.name}: ��� �ִϸ��̼� ���� ����! EnemyAnimatorController�� ã�� �� �����ϴ�.");
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
