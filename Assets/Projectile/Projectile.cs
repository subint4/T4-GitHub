using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction = Vector3.right;
    private int attackPower;
    private float speed = 10f;
    private bool canPierce = false; // 관통 여부
    private bool canStun = false; // 폭발 효과 여부
    private float stunDuration = 0f;
    private float slowEffect = 0f; // 적을 느리게 만드는 효과
    private float slowDuration = 2f;
    private float rangeLimit = 50f; // 투사체 삭제 거리
    private List<Enemy> hitEnemies = new List<Enemy>(); // 관통 시 여러 적을 맞출 리스트

    public void SetAttackPower(int power)
    {
        attackPower = power;
    }

    public void SetProjectileProperties(float newSpeed, bool pierce, bool stun, float slow,float slowTime)
    {
        speed = newSpeed;
        canPierce = pierce;
        canStun = stun;
        slowEffect = slow;
        slowDuration = slowTime;
    }

    public void Launch()
    {
        Debug.Log("투사체 발사");
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // 일정 거리 이상 이동하면 삭제
        if (Vector3.Distance(transform.position, Vector3.zero) > rangeLimit)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Projectile 충돌 감지됨: {collision.gameObject.name}");

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemyScript = collision.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                if (!canPierce || !hitEnemies.Contains(enemyScript)) // 관통 여부 체크
                {
                    enemyScript.TakeDamage(attackPower);
                    hitEnemies.Add(enemyScript);

                    if (slowEffect > 0)
                    {
                        StartCoroutine(ApplySlowEffect(enemyScript));
                    }

                    if (canStun)
                    {
                        StartCoroutine(Stun(enemyScript));
                    }

                    if (!canPierce) // 관통형이 아니면 즉시 삭제
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    private IEnumerator ApplySlowEffect(Enemy enemy)
    {
        float originalSpeed = enemy.movementSpeed;
        enemy.movementSpeed *= (1f - slowEffect);

        yield return new WaitForSeconds(slowDuration);

        enemy.movementSpeed = originalSpeed;
    }
    private IEnumerator Stun(Enemy enemy)
    {
        enemy.StopCoroutine("AttackTower");
        enemy.movementSpeed = 0f;
        enemy.enemyRigidbody.isKinematic = true;
        enemy.isAttacking = false;

        yield return new WaitForSeconds(stunDuration);

        enemy.movementSpeed = enemy.originalSpeed;
        enemy.enemyRigidbody.isKinematic = false;
        enemy.isAttacking = true;

        enemy.StartCoroutine("AttackTower");
    }
}
