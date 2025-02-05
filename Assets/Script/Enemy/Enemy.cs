using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemySO enemyStats;
    private bool isDead = false;
    private Rigidbody2D enemyRigidbody;
    private Collider2D enemyCollider;
    private int health;
    private int rewardMoney;
    private int attackPower;
    private float attackSpeed;
    private float movementSpeed;
    private float originalSpeed;
    private bool isAttacking = false;

    private Tower currentTarget;
    public EnemyAnimatorController controller;

    private void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<EnemyAnimatorController>();
        }

        enemyRigidbody = GetComponent<Rigidbody2D>();

        if (enemyStats != null)
        {
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
        isAttacking = true;
        movementSpeed = 0f;
        enemyRigidbody.isKinematic = true;

        if (controller != null)
        {
            controller.SetAttackState(true); // 랜덤 공격 애니메이션 실행
            Debug.Log($"[Enemy] {gameObject.name}: 공격 애니메이션 실행!");
        }

        yield return new WaitForSeconds(attackSpeed); // 공격 간격 유지

        if (!isDead && currentTarget != null && !currentTarget.IsDestroyed())
        {
            Debug.Log($"[Enemy] {gameObject.name}: 타워에 {attackPower}의 데미지를 입힘!");
            currentTarget.TakeDamage(attackPower);
        }

        StartCoroutine(ResetAttack()); // 다음 공격 실행 준비
    }

    public IEnumerator ResetAttack()
    {
        Debug.Log($"[Enemy] {gameObject.name}: 공격 대기 중...");

        yield return new WaitForSeconds(0.1f); // 애니메이션 트랜지션을 고려한 짧은 대기 시간

        isAttacking = false;

        if (controller != null)
        {
            controller.SetAttackState(false); // 공격 종료 애니메이션
        }

        yield return new WaitForSeconds(attackSpeed); // 공격 속도 반영하여 대기

        if (!isDead && currentTarget != null && !currentTarget.IsDestroyed())
        {
            Debug.Log($"[Enemy] {gameObject.name}: 다음 공격 시작!");
            StartCoroutine(AttackTower()); // 다음 공격 실행
        }
        else
        {
            StopAttack();
        }
    }

    private void StopAttack()
    {
        isAttacking = false;
        movementSpeed = originalSpeed;
        enemyRigidbody.isKinematic = false;

        if (controller != null)
        {
            controller.SetAttackState(false); // 공격 상태 종료
        }

        Debug.Log("공격 종료, 이동 상태 복구");
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


        if(enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        if(enemyRigidbody != null)
        {
            enemyRigidbody.simulated = false;
        }
        if (controller != null)
        {
            controller.PlayDeathAnimation();
        }

        Debug.Log($"[Enemy] {gameObject.name}: 사망 애니메이션 실행!");
    }

    public void DestroyEnemy()
    {
        Debug.Log($"[Enemy] {gameObject.name}: 사망 애니메이션 종료 후 제거됨.");
        Destroy(gameObject);
    }
}
