using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileSO projectileStats;
    private Vector3 direction = Vector3.right;
    private List<Enemy> hitEnemies = new List<Enemy>();
    private int damage;

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
