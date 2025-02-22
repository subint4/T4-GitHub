using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public ProjectileSO projectileStats;
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    private float damage;
    private Vector3 moveDirection;
    private float distanceTraveled = 0f; // 이동 거리 추적
    private Vector3 startPosition;
    private const float MaxRange = 1000f; // 최대 이동 거리 고정

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        MoveStraight();
    }

    public void Initialize(Tower tower, Vector3 direction)
    {
        if (tower != null && tower.towerStats != null)
        {
            damage = tower.towerStats.AttackPower;
            moveDirection = direction.normalized; // 방향 설정
            Debug.Log($"[Projectile] {tower.name}에서 발사됨! 데미지: {damage}, 방향: {moveDirection}");
        }
    }

    public void Initialize(Enemy enemy, Vector3 direction)
    {
        if (enemy != null && enemy.enemyStats != null)
        {
            damage = enemy.enemyStats.AttackPower;
            moveDirection = direction.normalized; // 방향 설정
            Debug.Log($"[Projectile] {enemy.name}에서 발사됨! 데미지: {damage}, 방향: {moveDirection}");
        }
    }

    private void MoveStraight()
    {
        transform.position += moveDirection * projectileStats.Speed * Time.deltaTime;
        distanceTraveled = Vector3.Distance(startPosition, transform.position);

        // 최대 사거리 1000f 초과 시 자동 제거
        if (distanceTraveled >= MaxRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                enemy.TakeDamage(damage);
                hitEnemies.Add(enemy);

                if (projectileStats.CanSlow)
                    enemy.ApplySlow(projectileStats.SlowEffect, projectileStats.SlowDuration);

                if (projectileStats.CanStun)
                    enemy.ApplyStun(projectileStats.StunDuration);

                if (!projectileStats.CanPierce)
                    Destroy(gameObject);
            }
        }
    }
}
