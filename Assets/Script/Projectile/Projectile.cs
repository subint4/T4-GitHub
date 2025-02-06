using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileSO projectileStats; // 투사체 데이터 저장

    private Vector3 direction = Vector3.right;
    private List<Enemy> hitEnemies = new List<Enemy>();

    int damage;
    private void Start()
    {
        if (projectileStats == null)
        {
            Debug.LogError("투사체 데이터가 없습니다!");
            return;
        }
    }
    public void SetDamage(int towerDamage)
    {
        damage = towerDamage;
    }

    private void Update()
    {
        transform.position += direction * projectileStats.Speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, Vector3.zero) > 50f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        Debug.Log($"충돌 대상 : {collision}");
        if (enemy != null)
        {
                    enemy.TakeDamage(projectileStats.Damage);
                    hitEnemies.Add(enemy);

                    if (projectileStats.CanSlow)
                        enemy.ApplySlow(projectileStats);
                        
                    else if (projectileStats.CanStun)
                        enemy.ApplyStun(projectileStats);
                    if (!projectileStats.CanPierce)
                        Destroy(gameObject);
                   
        }
    }

    //private IEnumerator Stun(Enemy enemy)
    //{
    //    float originalSpeed = enemy.movementSpeed;
    //    enemy.movementSpeed = 0f;
    //    enemy.enemyRigidbody.isKinematic = false;
    //    enemy.isAttacking = false;

    //    yield return new WaitForSeconds(projectileStats.stunDuration);

    //    enemy.movementSpeed = originalSpeed;
    //    enemy.enemyRigidbody.isKinematic = false;
    //    enemy.isAttacking = true;

    //}

    //private IEnumerator ApplySlowEffect(Enemy enemy)
    //{
    //    float originalSpeed = enemy.movementSpeed;
    //    enemy.movementSpeed *= (1f - projectileData.SlowEffect);

    //    yield return new WaitForSeconds(projectileData.SlowDuration);

    //    enemy.movementSpeed = originalSpeed;
    //}
}

