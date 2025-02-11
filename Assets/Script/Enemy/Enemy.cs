using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    public int EnemyID;
    public EnemySO enemyStats;
    public Rigidbody2D enemyRigidbody;
    private bool isDead = false;
    private Collider2D enemyCollider;
    private int health;
    private int rewardMoney;
    private int attackPower;
    public float attackSpeed;
    public float movementSpeed;
    public float originalSpeed;
    public bool isAttacking = false;
    public bool isSlowed = false;

    private Tower currentTarget;
    private ProjectileSO projectileStats;
    public EnemyAnimatorController controller;

    public event Action<GameObject> OnEnemyDeath;
    private float attackCooldown = 0f; // 공격 쿨다운 추가

    private void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<EnemyAnimatorController>();
        }

        enemyRigidbody = GetComponent<Rigidbody2D>();

        if (enemyStats != null)
        {
            EnemyID = enemyStats.EnemyID;
            health = enemyStats.Health;
            rewardMoney = enemyStats.RewardMoney;
            attackPower = enemyStats.AttackPower;
            attackSpeed = enemyStats.AttackSpeed;
            movementSpeed = enemyStats.MovementSpeed;
            originalSpeed = movementSpeed;
        }
        else
        {
            Debug.LogError("적 스탯이 연결되지 않았습니다!");
        }

        transform.localScale = new Vector3(-1, 1, 1); // 왼쪽으로 이동
    }

    private void Update()
    {
        if (!isAttacking && !isDead)
        {
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (currentTarget != null && currentTarget.IsDestroyed())
        {
            Debug.Log($"[Enemy] {gameObject.name}: 타겟이 파괴됨 -> 이동 시작");
            StopAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndLine"))
        {
            GameManager.instance?.GameOver();
            Debug.Log("적이 EndLine에 도달하여 게임 종료!");
            Destroy(gameObject);
            return;
        }

        Tower tower = collision.GetComponent<Tower>();
        if (tower != null && !isDead)
        {
            currentTarget = tower;
            if (!isAttacking)
            {
                StartCoroutine(AttackTower());
            }
        }
    }

    private IEnumerator AttackTower()
    {
        if (isAttacking) yield break; // 중복 실행 방지

        isAttacking = true;
        movementSpeed = 0f;
        enemyRigidbody.isKinematic = true;

        if (controller != null)
        {
            controller.SetAttackState(true);
            Debug.Log($"[Enemy] {gameObject.name}: 공격 애니메이션 실행!");
        }

        while (currentTarget != null && !currentTarget.IsDestroyed() && !isDead)
        {
            if (attackCooldown <= 0)
            {
                attackCooldown = attackSpeed;
                currentTarget.TakeDamage(attackPower);
                Debug.Log($"[Enemy] {gameObject.name}: {currentTarget.name}에게 {attackPower} 피해를 입힘");

                yield return new WaitForSeconds(attackSpeed);
            }
            else
            {
                yield return null;
            }
        }

        Debug.Log($"[Enemy] {gameObject.name}: 타겟이 파괴됨 -> 이동 시작");
        StopAttack();
    }

    public IEnumerator ResetAttack()
    {
        Debug.Log($"[Enemy] {gameObject.name}: 공격 대기 중...");

        yield return new WaitForSeconds(0.1f);

        isAttacking = false;

        if (controller != null)
        {
            controller.SetAttackState(false);
        }

        attackCooldown = 0; // 공격 쿨다운 초기화

        yield return new WaitForSeconds(attackSpeed);

        if (!isDead && currentTarget != null && !currentTarget.IsDestroyed())
        {
            Debug.Log($"[Enemy] {gameObject.name}: 다음 공격 시작!");
            StartCoroutine(AttackTower());
        }
        else
        {
            StopAttack();
        }
    }

    private void StopAttack()
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

            Debug.Log($"[Enemy] {gameObject.name}: 공격 종료, 이동 시작");
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
        Debug.Log($"[Enemy] {gameObject.name}: 사망 애니메이션 실행!");

        OnEnemyDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }

    public void DestroyEnemy()
    {
        Debug.Log($"[Enemy] {gameObject.name}: 사망 애니메이션 종료 후 제거됨.");
        Destroy(gameObject);
    }

    public void ApplySlow(ProjectileSO projectileStats)
    {
        if (!isSlowed)
        {
            originalSpeed = movementSpeed;
            float slowFactor = Mathf.Clamp(1f - projectileStats.SlowEffect, 0.1f, 1f);
            movementSpeed = Mathf.Max(movementSpeed * slowFactor, 0.1f);
            attackCooldown *= slowFactor; // 공격 속도에 슬로우 효과 적용
            isSlowed = true;
        }
        Invoke("EndSlow", projectileStats.SlowDuration);
    }

    public void EndSlow()
    {
        movementSpeed = originalSpeed;
        attackCooldown = attackSpeed;
        isSlowed = false;
    }

    public void ApplyStun(ProjectileSO projectileStats)
    {
        originalSpeed = movementSpeed;
        movementSpeed = 0f;
        enemyRigidbody.isKinematic = false;
        isAttacking = false;
        attackCooldown += projectileStats.StunDuration; // 스턴 동안 공격 쿨다운 증가
        Invoke("EndStun", projectileStats.StunDuration);
    }

    public void EndStun()
    {
        movementSpeed = originalSpeed;
        enemyRigidbody.isKinematic = true;
    }
}
