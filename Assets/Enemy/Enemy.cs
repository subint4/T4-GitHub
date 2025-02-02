using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;

public class Enemy : MonoBehaviour
{
    public EnemySO enemyStats;
    private Tower tower;
    private bool isDead = false;

    private Rigidbody2D rb;
    private int health;
    private int rewardMoney;
    private int attackPower;
    private float attackSpeed;
    private float movementSpeed;
    private bool isAttacking = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D 초기화 추가

        if (enemyStats != null)
        {
            // EnemySO에서 초기화
            health = enemyStats.Health;
            rewardMoney = enemyStats.RewardMoney;
            attackPower = enemyStats.AttackPower;
            attackSpeed = enemyStats.AttackSpeed; 
            movementSpeed = enemyStats.MovementSpeed;

            Debug.Log($"적 초기화 완료. 체력: {health}, 보상: {rewardMoney}");
        }
        else
        {
            Debug.LogError("적 스탯이 연결되지 않았습니다!");
        }
    }
    private void Update()
    {
        if (!isAttacking)
        {
        transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("SpawnableArea"))
        {
            Debug.Log($"{gameObject.name}이(가) {collision.gameObject.name} (Layer: SpawnableArea)을(를) 무시함.");
            return; // 아무 동작도 하지 않고 리턴
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Debug.Log($"{gameObject.name}이(가) {collision.gameObject.name} (Layer: Projectile)을(를) 무시함.");
            return; // 아무 동작도 하지 않고 리턴
        }
        Debug.Log($"충돌 감지됨: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if(collision.CompareTag("EndLine"))
        {
            Debug.Log($"{gameObject.name}이(가) EndLine에 도달하여 게임 오버!");
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
            Destroy(gameObject);
        }
        Tower tower = collision.GetComponent<Tower>();
        if (tower == null) tower = collision.GetComponentInParent<Tower>();
        if (tower == null) tower = collision.GetComponentInChildren<Tower>();

        if (tower != null)
        {
            Debug.Log("Tower 컴포넌트를 찾았습니다!");
            StartCoroutine(AttackTower(tower));
        }
        else
        {
            Debug.LogError($"오류: Tower 컴포넌트가 존재하지 않습니다! 충돌한 오브젝트: {collision.gameObject.name}");
        }
    }
    private IEnumerator AttackTower(Tower tower)
    {
        isAttacking = true;
        movementSpeed = 0;  // 공격 중에는 이동 멈춤
        rb.velocity = Vector2.zero;

        while (tower != null && health > 0)
        {
            tower.TakeDamage(attackPower);
            Debug.Log($"타워에 {attackPower}의 데미지를 입힘");
            yield return new WaitForSeconds(1f); // 1초마다 공격 실행
        }
        movementSpeed = enemyStats.MovementSpeed;
        isAttacking = false;
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        if(health<=0)
        {
            Die();
        }
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        if(PlayerSystem.instance != null)
        {
        PlayerSystem.instance.AddMoney(rewardMoney);
        }
        StopCoroutine(AttackTower(tower)); // 공격 루틴 종료
        Destroy(gameObject);
    }
}
