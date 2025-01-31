using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject target;
    private int attackPower;
    private float speed = 10f;

    public void SetTarget(GameObject enemy, int damage)
    {
        target = enemy;
        attackPower = damage;
    }
    private void Update()
    {
        if (target == null)
        {
            // 타겟이 null이면 계속 앞으로 이동 후 일정 거리에서 파괴
            transform.position += transform.right * speed * Time.deltaTime;

            // 일정 거리 이상 이동하면 삭제
            if (Vector3.Distance(transform.position, Vector3.zero) > 50f) // 50f는 예시 거리
            {
                Destroy(gameObject);
            }
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Projectile 충돌 감지됨: {collision.gameObject.name}");

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemyScript = collision.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackPower);
            }

            Debug.Log($"{collision.gameObject.name}에게 {attackPower}의 피해를 입힘! 투사체 제거");
            Destroy(gameObject); // 충돌 시 제거
        }
    }
}