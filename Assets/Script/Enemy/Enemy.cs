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
                Debug.Log($"[Enemy] {gameObject.name} - Ÿ�� ���� ����: {currentTarget.name}, ���� ����");
                StartCoroutine(AttackLoop());
            }
        }
    }
        private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == currentTarget)
        {
            Debug.Log($"[Enemy] {gameObject.name} - {currentTarget.name}���� ���, 1�� �� ��Ȯ��");
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
            Debug.Log($"[Enemy] {gameObject.name} - ���� ����, �̵� �簳");
        }
    }
    private IEnumerator AttackLoop()
    {
        if (isAttacking || isStunned) yield break;
        isAttacking = true;

        if (enemyAnimatorController == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: AttackLoop ���� �� enemyAnimatorController�� NULL!");
            yield break;
        }

        enemyAnimatorController.SetAttackState(true);
        Debug.Log($"[Enemy] {gameObject.name}: ���� �ִϸ��̼� ����!");

        float waitTime = 0f;
        while (!enemyAnimatorController.IsPlayingAttackAnimation() && waitTime < 1.0f) // �ִ� 1�� ���
        {
            yield return null;
            waitTime += Time.deltaTime;
        }

        if (!enemyAnimatorController.IsPlayingAttackAnimation())
        {
            Debug.LogError($"[Enemy] {gameObject.name}: ���� �ִϸ��̼� ���� ����!");
            isAttacking = false;
            yield break;
        }

        yield return new WaitForSeconds(attackSpeed);

        // Ÿ���� ��������� �ٽ� ���� ����
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
            Debug.Log($"[Enemy] {gameObject.name}: ���� ����, �̵� �簳");
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
