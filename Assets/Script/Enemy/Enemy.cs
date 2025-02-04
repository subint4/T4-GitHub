using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemySO enemyStats;
    private bool isDead = false;
    private Rigidbody2D rb;
    private int health;
    private int rewardMoney;
    private int attackPower;
    private float attackSpeed;
    private float movementSpeed; //이동 속도 값 유지
    private float originalSpeed; //공격 종료 후 원래 속도로 복구
    private bool isAttacking = false;

    private Tower currentTarget;
    public EnemyAnimatorController controller;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemyStats != null)
        {
            health = enemyStats.Health;
            rewardMoney = enemyStats.RewardMoney;
            attackPower = enemyStats.AttackPower;
            attackSpeed = enemyStats.AttackSpeed;
            movementSpeed = enemyStats.MovementSpeed; //이동 속도 가져오기
            originalSpeed = movementSpeed; //원래 속도 저장
        }
        else
        {
            Debug.LogError("적 스탯이 연결되지 않았습니다!");
        }

        //방향 고정 (왼쪽으로 이동)
        transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Update()
    {
        if (!isAttacking && !isDead)
        {
            //이동 속도가 유지되도록 설정
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("SpawnableArea") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            return;
        }

        if (collision.CompareTag("EndLine"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
            Destroy(gameObject);
        }

        Tower tower = collision.GetComponent<Tower>();
        if (tower == null) tower = collision.GetComponentInParent<Tower>();
        if (tower == null) tower = collision.GetComponentInChildren<Tower>();

        if (tower != null && !isAttacking)
        {
            currentTarget = tower;
            controller.SetAttackState(true);
            isAttacking = true;

            //공격 시 이동 멈춤
            movementSpeed = 0;
        }
    }

    //공격 애니메이션이 끝날 때 실행됨 (애니메이션 이벤트에서 호출)
    public void ApplyDamage()
    {
        if (currentTarget != null && !isDead)
        {
            currentTarget.TakeDamage(attackPower);
            Debug.Log($"타워에 {attackPower}의 데미지를 입힘");
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

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        controller.PlayDeathAnimation();

        if (PlayerSystem.instance != null)
        {
            PlayerSystem.instance.AddMoney(rewardMoney);
        }

        Destroy(gameObject, 1.5f);
    }

    //공격 애니메이션이 끝나면 이동 속도 복구
    public void OnAttackAnimationEnd()
    {
        if (!isDead)
        {
            movementSpeed = originalSpeed; //원래 이동 속도로 복구
            isAttacking = false;
        }
    }
}
