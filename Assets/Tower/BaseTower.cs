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
    private int attackDamage;
    public LayerMask targetLayer;

    public TowerStatHandler towerStatHandler;
    public float lastAttackTime = 0f;
    private Transform currentTarget;
    private UpgradeSystem.TowerStat currentStats;
    public UpgradeSystem upgradeSystem;

    private void Start()
    {
        if(towerStatHandler==null)
        {
            Debug.LogError("TowerStatHandler is not assigned");
            return;
        }
        if(towerStatHandler.CurrentStat==null)
        {
            Debug.LogError("TowerStatHandler.CurrentStat is not initialized.");
            return;
        }
        attackDamage = towerStatHandler.CurrentStat.TowerBaseDamage;
        Debug.Log($"Initialized attackDamage: {attackDamage} (Expected: {towerStatHandler.CurrentStat.TowerBaseDamage})");

    }
    private void Update()
    {
        if(upgradeSystem != null)
        {
            var currentStats = upgradeSystem.GetCurrentStats();
            attackDamage = currentStats.damage;
            attackCooldown = 1f / currentStats.attackSpeed;
        }
        //float attackSpeed = towerStatHandler.CurrentStat.TowerBaseAttackSpeed;
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
            int damageToApply = attackDamage;
            projectile.Initialize(target,damageToApply);
            Debug.Log($"projectile fired at target: {target.name} with damage: {attackDamage}");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}