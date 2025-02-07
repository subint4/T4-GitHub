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
    public bool isStunned = false;

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
            // 타워가 적의 왼쪽에 있는지 확인
            if (tower.transform.position.x < transform.position.x)
            {
                if (currentTarget == null || currentTarget.IsDestroyed())
                {
                    currentTarget = tower;
                    Debug.Log($"[Enemy] {gameObject.name}: 새로운 타겟 설정 → {currentTarget.gameObject.name}");
                }

                if (currentTarget == tower && !isAttacking)
                {
                    StartCoroutine(AttackTower());
                }
            }
        }
    }




    public IEnumerator AttackTower()
    {
        if (isAttacking) yield break; // 이미 공격 중이라면 중복 실행 방지
        isAttacking = true;
        movementSpeed = 0f;
        enemyRigidbody.velocity = Vector2.zero;
        enemyRigidbody.isKinematic = true;

        if (currentTarget == null || currentTarget.IsDestroyed())
        {
            StopAttack();
            yield break;
        }

        if (controller != null)
        {
            controller.SetAttackState(true);
            Debug.Log($"[Enemy] {gameObject.name}: 공격 애니메이션 실행!");
        }

        yield return new WaitForSeconds(attackSpeed); // 애니메이션이 끝날 때까지 대기

        ApplyDamage(); // 공격 애니메이션이 끝난 후 데미지 적용

        isAttacking = false; // 공격 상태 초기화

        if (controller != null)
        {
            controller.SetAttackState(false); // 공격 종료 애니메이션 실행
        }

        yield return new WaitForSeconds(0.1f); // 타워 공격 후 짧은 대기 시간

        // 현재 타겟이 살아있으면 다시 공격, 아니면 이동
        if (currentTarget != null && !currentTarget.IsDestroyed())
        {
            StartCoroutine(AttackTower());
        }
        else
        {
            StopAttack();
        }
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
        if (!isStunned)
        {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(AttackTower());
        }
    }


    public void StopAttack()
    {
        isAttacking = false;
        currentTarget = null;

        if (!isDead)
        {
            if (!isSlowed && !isStunned)
            {
                movementSpeed = originalSpeed; // 정상 이동 속도로 복구
            }
        }

        enemyRigidbody.isKinematic = false;

        if (controller != null)
        {
            controller.SetAttackState(false); // 공격 상태 해제
        }

        // 새로운 타워 탐색 (가장 가까운 타워를 다시 찾기)
        Tower newTarget = FindClosestTower();
        if (newTarget != null)
        {
            currentTarget = newTarget;
            StartCoroutine(AttackTower());  
        }
        else
        {
            Debug.Log($"[Enemy] {gameObject.name}: 타워가 없으므로 이동 재개");
        }
    }


    private Tower FindClosestTower()
    {
        Tower[] towers = FindObjectsOfType<Tower>();
        Tower closestTower = null;
        float minDistance = Mathf.Infinity;

        foreach (Tower tower in towers)
        {
            if (tower == null || tower.isDead) continue; // 이미 파괴된 타워는 무시

            // 왼쪽에 있는 타워만 탐색
            if (tower.transform.position.x < transform.position.x)
            {
                float distance = Vector3.Distance(transform.position, tower.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTower = tower;
                }
            }
        }

        return closestTower;
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
        if(currentTarget == null || currentTarget.IsDestroyed())
        {
            StopAttack();
            return;
        }
        currentTarget.TakeDamage(attackPower);
        Debug.Log($"타겟 : {currentTarget} 체력 : {currentTarget.towerStats.Health}");
        
        if(currentTarget.towerStats.Health<=0)
        {
            StopAttack();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        movementSpeed = 0f;
        enemyRigidbody.velocity = Vector2.zero;
        enemyRigidbody.isKinematic = false;


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
        if(currentTarget==null || currentTarget.IsDestroyed())
        {
            Debug.Log($"[Enemy] {gameObject.name}: 슬로우 상태에서 타겟이 파괴됨. 이동 재개");
            ResetAttack();
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
            if(!isAttacking)
            {
                StopAttack();
            }
        }
    }
    public void ApplyStun(ProjectileSO projectileStats)
    {
        if (isDead) return;
        originalSpeed = movementSpeed;
        movementSpeed = 0f;
        enemyRigidbody.isKinematic = false;
        isAttacking = false;
        isStunned = true;
        if (currentTarget == null || currentTarget.IsDestroyed())
        {
            Debug.Log($"[Enemy] {gameObject.name}: 스턴 상태에서 타겟이 파괴됨. 이동 재개");
            StopAttack();
        }
        Invoke("EndStun", projectileStats.StunDuration);

    }
    public void EndStun()
    {
        movementSpeed = originalSpeed;
        enemyRigidbody.isKinematic = false;
        isStunned = false;
        if (!isAttacking)
        {
            StopAttack();
        }
    }

}
