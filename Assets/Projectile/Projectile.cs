using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction = Vector3.right;
    private int attackPower;
    private float speed = 10f;



    public void SetAttackPower(int power)
    {
        attackPower = power;
    }
    public void Launch()
    {
        Debug.Log("투사체 발사");
    }
    private void Update()
    {
            transform.position += direction * speed * Time.deltaTime;
            // 충돌시 삭제
            
            // 일정 거리 이상 이동하면 삭제
            if (Vector3.Distance(transform.position, Vector3.zero) > 50f) // 50f는 예시 거리
            {
                Destroy(gameObject);
            }
            return;
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