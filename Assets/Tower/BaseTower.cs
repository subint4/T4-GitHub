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
        // 같은 GameObject에서 UpgradeSystem 컴포넌트 검색
        upgradeSystem = GetComponent<UpgradeSystem>();

        // 다른 GameObject에 연결된 UpgradeSystem 검색 (필요할 경우)
        if (upgradeSystem == null)
        {
            upgradeSystem = FindObjectOfType<UpgradeSystem>();
        }

        if (upgradeSystem == null)
        {
            Debug.LogError("UpgradeSystem을 찾을 수 없습니다!");
        }
        UpdateAttackDamage();

    }
    private void Update()
    {
        if(upgradeSystem != null)
        {
            var currentStats = upgradeSystem.GetCurrentStats();
            attackDamage = currentStats.damage;
        }
        //float attackSpeed = towerStatHandler.CurrentStat.TowerBaseAttackSpeed;
    if(Time.time>lastAttackTime+attackCooldown)
        {
                FindAndAttackEnemy();
        }
    }
    private void UpdateAttackDamage()
    {
        if(upgradeSystem != null)
        {
            var currentStats = upgradeSystem.GetCurrentStats();
            if(currentStats != null)
            {
                attackDamage = currentStats.damage;
                Debug.Log($"현재 공격력 : {attackDamage}");
            }
            else
            {
                Debug.LogWarning("currentStats가 초기화되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning("upgradeSystem이 연결되지 않았습니다.");
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