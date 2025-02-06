using System.Collections;
using Unity.Android.Types;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemySO enemyStats;
    public Rigidbody2D enemyRigidbody;
    private bool isDead = false;
    private Collider2D enemyCollider;
    private int health;
    private int rewardMoney;
    private int attackPower;
    [HideInInspector] public float attackSpeed;
    [HideInInspector] public float movementSpeed;
    [HideInInspector] public float originalSpeed;
    public bool isAttacking = false;
    public bool isSlowed = false;

    public Tower currentTarget;
    private ProjectileSO projectileStats;
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

    public IEnumerator AttackTower()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        movementSpeed = 0f;
        enemyRigidbody.isKinematic = true;

        if (controller != null)
        {
            controller.SetAttackState(true); // 공격 애니메이션 실행
            Debug.Log($"[Enemy] {gameObject.name}: 공격 애니메이션 실행!");
        }

        yield return new WaitUntil(() => isAttacking == false);
        // 타워가 파괴되었는지 다시 확인
        if (currentTarget == null || currentTarget.IsDestroyed())
        {
            StopAttack();
            yield break;
        }
        StartCoroutine(ResetAttack());
    }


    public IEnumerator ResetAttack()
    {
        Debug.Log($"[Enemy] {gameObject.name}: ResetAttack() 호출됨");

        yield return new WaitForSeconds(attackSpeed); // 공격 속도만큼 대기
        isAttacking = false; // 공격 상태 초기화


        if (controller != null)
        {
            controller.SetAttackState(false); // 공격 종료 애니메이션
        }
        if (currentTarget == null || currentTarget.IsDestroyed())
        {
            Debug.Log($"[Enemy] {gameObject.name}: 타겟이 사라짐, StopAttack() 실행");
            StopAttack();
            yield break;
        }
        StartCoroutine(AttackTower());
    }


    public void StopAttack()
    {
        isAttacking = false;
        movementSpeed = originalSpeed;
        enemyRigidbody.isKinematic = false;

        if (controller != null)
        {
            controller.SetAttackState(false); // 공격 상태 종료
        }
        if (currentTarget != null && !currentTarget.IsDestroyed())
        {
            Debug.Log($"[Enemy] {gameObject.name}: 이동 후 다시 공격 시도");
            StartCoroutine(AttackTower()); // 이동 후 다시 공격 실행
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
    public void ApplyDamage()
    {
        currentTarget.towerStats.Health-= attackPower;
        Debug.Log($"타겟 : {currentTarget} 체력 : {currentTarget.towerStats.Health}");
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
            movementSpeed *= (1f - projectileStats.SlowEffect);
            isSlowed = true;
        }
        Invoke("EndSlow", projectileStats.SlowDuration);
    }
    public void EndSlow()
    {
        if (isDead) return;

        if (isSlowed)
        {
            movementSpeed = originalSpeed;
            isSlowed = false;
        }
    }
    public void ApplyStun(ProjectileSO projectileStats)
    {
        if (isDead) return;
        originalSpeed = movementSpeed;
        movementSpeed = 0f;
        enemyRigidbody.isKinematic = false;
        isAttacking = false;
        Invoke("EndStun", projectileStats.StunDuration);

    }
    public void EndStun()
    {
        movementSpeed = originalSpeed;
        enemyRigidbody.isKinematic = true;
    }

}
