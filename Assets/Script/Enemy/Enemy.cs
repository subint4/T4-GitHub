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
    private int health;
    private int rewardMoney;
    public int attackPower;
    public float attackSpeed;
    public float movementSpeed;
    public float originalSpeed;
    public bool isAttacking = false;
    public bool isSlowed = false;

    public Tower currentTarget;
    private ProjectileSO projectileStats;
    public EnemyAnimatorController controller;

    public event Action<GameObject> OnEnemyDeath;

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
                StartCoroutine(AttackLoop());
            }
        }
    }

    /// <summary>
    /// 공격 루프를 실행하여 `attackSpeed` 간격으로 반복
    /// </summary>
    private IEnumerator AttackLoop()
    {
        isAttacking = true;

        while (!isDead && currentTarget != null)
        {
            // 타겟이 파괴되지 않았는지 다시 확인
            if (currentTarget.IsDestroyed())
            {
                Debug.Log($"[Enemy] {gameObject.name}: 타겟이 이미 파괴됨 -> 공격 종료");
                StopAttack();
                yield break;
            }

            Debug.Log($"[Enemy] {gameObject.name}: 공격 시작!");


            if (controller != null)
            {
                controller.SetAttackState(true);  // 공격 애니메이션 실행
            }

            yield return new WaitForSeconds(attackSpeed); // 공격 딜레이

            if (controller != null)
            {
                controller.SetAttackState(false);  // 애니메이션 종료
            }

            yield return new WaitForSeconds(0.1f); // 짧은 대기 후 반복
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

            Debug.Log($"[Enemy] {gameObject.name}: 공격 종료, 이동 시작");
        }
    }
    private void RestartAttack()
    {
        if (!isDead && currentTarget != null)
        {
            // 타겟이 파괴되지 않았는지 다시 확인
            if (currentTarget.IsDestroyed())
            {
                Debug.Log($"[Enemy] {gameObject.name}: 타겟이 이미 파괴됨 -> 이동 시작");
                StopAttack();
                return;
            }

            Debug.Log($"[Enemy] {gameObject.name}: 공격 재개!");

            if (controller != null)
            {
                controller.SetAttackState(true);  // 공격 애니메이션 다시 실행
            }

            StartCoroutine(AttackLoop());
        }
    }




    // `AttackLoop` 실행을 위한 중간 메서드
    private void StartAttackLoop()
    {
        if (!isDead && currentTarget != null && !currentTarget.IsDestroyed())
        {
            StartCoroutine("AttackLoop");
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

    /// <summary>
    /// 슬로우 효과 적용 (이동 속도 및 공격 속도 조정)
    /// </summary>
    public void ApplySlow(ProjectileSO projectileStats)
    {
        if (!isSlowed)
        {
            originalSpeed = movementSpeed;
            float slowFactor = Mathf.Clamp(1f - projectileStats.SlowEffect, 0.1f, 1f);
            movementSpeed = Mathf.Max(movementSpeed * slowFactor, 0.1f);
            attackSpeed /= slowFactor; // 공격 속도 조정
            isSlowed = true;
        }
        Invoke("EndSlow", projectileStats.SlowDuration);
    }

    public void EndSlow()
    {
        movementSpeed = originalSpeed;
        attackSpeed = enemyStats.AttackSpeed; // 원래 공격 속도로 복구
        isSlowed = false;
    }

    /// <summary>
    /// 스턴 효과 적용 (공격 중단 및 이동 불가)
    /// </summary>
    public void ApplyStun(ProjectileSO projectileStats)
    {
        originalSpeed = movementSpeed;
        movementSpeed = 0f;
        enemyRigidbody.isKinematic = false;
        isAttacking = false;
        StopCoroutine(AttackLoop()); // 공격 중단
        Invoke("EndStun", projectileStats.StunDuration);
    }

    public void EndStun()
    {
        movementSpeed = originalSpeed;
        enemyRigidbody.isKinematic = true;
        if (currentTarget != null && !currentTarget.IsDestroyed() && !isDead)
        {
            StartCoroutine(AttackLoop()); // 스턴 후 다시 공격 시작
        }
    }
}
