using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    public int EnemyID;
    public EnemySO enemyStats;
    public Rigidbody2D enemyRigidbody;
    public bool isDead = false;
    private Collider2D enemyCollider;
    private float health;
    private int rewardMoney;
    public float attackPower;
    public float attackSpeed;
    public float movementSpeed;
    public float originalSpeed;
    public bool isAttacking = false;
    public bool isSlowed = false;
    public float currentSpeed;

    public Tower currentTarget;
    private ProjectileSO projectileStats;
    public EnemyAnimatorController controller;

    public event Action<GameObject> OnEnemyDeath;

    public void Initialize(EnemySO stats)
    {
        enemyStats = stats;
        if (enemyStats != null)
        {
            currentSpeed = enemyStats.MovementSpeed;
            movementSpeed = enemyStats.MovementSpeed;
            health = enemyStats.Health;
            rewardMoney = enemyStats.RewardMoney;
        }
        transform.localScale = new Vector3(-1, 1, 1); // 왼쪽으로 이동
    }

    private void Start()
    {
        if (enemyStats == null)
        {
            Debug.LogError($"Enemy ({gameObject.name}): EnemySO가 할당되지 않았습니다!");
        }
    }

    private void Update()
    {
        if (!isAttacking && !isDead)
        {

            float moveSpeed = Mathf.Abs(movementSpeed);  // 항상 양수 값 보장
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        if (currentTarget != null && currentTarget.IsDestroyed())
        {
            StopAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndLine"))
        {
            GameManager.Instance?.GameOver();
            Destroy(gameObject);
            return;
        }

        Tower tower = collision.GetComponent<Tower>();
        if (tower != null && !isDead)
        {
            currentTarget = tower;
            if (!isAttacking)
            {
                StartCoroutine(AttackLoop());
            }
        }
    }

    private IEnumerator AttackLoop()
    {
        isAttacking = true;

        while (!isDead && currentTarget != null)
        {
            if (currentTarget.IsDestroyed())
            {
                StopAttack();
                yield break;
            }

            if (controller != null)
            {
                controller.SetAttackState(true);
            }

            yield return new WaitForSeconds(attackSpeed);

            if (controller != null)
            {
                controller.SetAttackState(false);
            }

            yield return new WaitForSeconds(0.1f);
        }

        isAttacking = false;
    }

    public void StopAttack()
    {
        if (!isDead)
        {
            isAttacking = false;
            movementSpeed = Mathf.Max(originalSpeed, 0.1f);
            enemyRigidbody.isKinematic = false;
            if (controller != null)
            {
                controller.SetAttackState(false);
            }
        }
    }

    public void TakeDamage(int damage)
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
        movementSpeed = 0f;
        enemyRigidbody.velocity = Vector2.zero;
        enemyRigidbody.isKinematic = true;

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        if (enemyRigidbody != null)
        {
            enemyRigidbody.simulated = false;
        }
        if (controller != null)
        {
            controller.PlayDeathAnimation();
        }

        ResourceManager.Instance?.AddGold(rewardMoney);
        OnEnemyDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }

    public void ApplySlow(float slowFactor, float duration)
    {
        if (!isSlowed)
        {
            originalSpeed = movementSpeed;
            float adjustedSlowFactor = Mathf.Clamp(1f - slowFactor, 0.1f, 1f);
            movementSpeed = Mathf.Max(movementSpeed * adjustedSlowFactor, 0.1f);
            attackSpeed /= adjustedSlowFactor;
            isSlowed = true;
        }
        Invoke("EndSlow", duration);
    }

    public void EndSlow()
    {
        movementSpeed = originalSpeed;
        attackSpeed = enemyStats.AttackSpeed;
        isSlowed = false;
    }

    public void ApplyStun(float duration)
    {
        if (isDead) return;

        originalSpeed = movementSpeed;
        movementSpeed = 0f;
        enemyRigidbody.isKinematic = false;
        isAttacking = false;
        StopCoroutine(AttackLoop());
        Invoke("EndStun", duration);
    }

    public void EndStun()
    {
        movementSpeed = originalSpeed;
        enemyRigidbody.isKinematic = true;
        if (currentTarget != null && !currentTarget.IsDestroyed() && !isDead)
        {
            StartCoroutine(AttackLoop());
        }
    }
}
