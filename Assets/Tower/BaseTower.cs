using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTower : MonoBehaviour
{
    [Header("Tower Settings")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;
    public LayerMask targetLayer;

    public float lastAttackTime = 0f;
    private Transform currentTarget;

    private void Update()
    {
    if(Time.time>lastAttackTime+attackCooldown)
        {
                FindAndAttackEnemy();
        }
    }
    private void FindAndAttackEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

        if (hitColliders.Length > 0)
        {
            Transform nearestEnemy = GetNearestEnemy(hitColliders);

            if (nearestEnemy != null && nearestEnemy.gameObject != null)
            {
                Shoot(nearestEnemy);
                lastAttackTime = Time.time;
            }
        }
        else 
        {
            Debug.Log("No enemies in range");
        }
    }
    private Transform GetNearestEnemy(Collider2D[] enemies)
    {
        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D enemyCollider in enemies)
        {
            if(enemyCollider==null||enemyCollider.transform==null)
            {
                Debug.LogWarning("Null Enemy detected in getnearestenemy");
                continue;
            }
            float distance = Vector2.Distance(transform.position,enemyCollider.transform.position);
            if(distance<shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemyCollider.transform;
            }
        }
        return nearestEnemy;
    }

    private void Shoot(Transform target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("projectile prefab is null");
            return;
        }

        if(target == null)
        {
            Debug.LogWarning("cant shoot : target is null");
            return;
        }
        GameObject projectileObject = Instantiate(projectilePrefab,firePoint.position,Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        Debug.Log("projectile fired");

        if(projectile != null)
        {
            projectile.Initialize(target);
            Debug.Log($"projectile fired at target: {target.name}");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}