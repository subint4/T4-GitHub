using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerSO towerStats;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public TowerAnimatorController animatorController;
    public Tiles currentTiles;

    private int Health;
    private float attackCooldown = 0f;
    private GameObject currentTarget;
    private float detectionRange = 1000f;
    [SerializeField] private LayerMask enemyLayer;

    private void Start()
    {
        if (towerStats != null)
        {
            Health = towerStats.Health;
        }
    }

    public void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (currentTarget == null || !currentTarget.activeInHierarchy)
        {
            currentTarget = null;
            animatorController.SetAttackState(false);
        }
        // 적 탐지
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, enemyLayer);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            if (currentTarget != hit.collider.gameObject)
            {
                currentTarget = hit.collider.gameObject;
            }

            // **애니메이션이 실행 중이 아닐 때만 새로운 공격 시작**
            if (!animatorController.IsPlayingAttackAnimation() && attackCooldown <= 0f)
            {
                attackCooldown = towerStats.AttackSpeed;
                animatorController.SetAttackState(true);
            }
        }
        else
        {
            if (!animatorController.IsPlayingAttackAnimation())
            {
                currentTarget = null;
            }
        }
    }

    public void Attack()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("투사체 프리팹이 연결되지 않았습니다!");
            return;
        }

        if (currentTarget == null)
        {
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        ProjectileDataLoader.ProjectileData projectileData = ProjectileDataLoader.GetProjectileData(towerStats.TowerType);

        projectileScript.SetProjectileProperties(
            projectileData.Speed,
            projectileData.CanPierce,
            projectileData.HasExplosion,
            projectileData.SlowEffect,
            projectileData.SlowDuration);

        if (projectileScript != null)
        {
            projectileScript.SetAttackPower(towerStats.AttackPower);
            if(towerStats.TowerType == TowerType.Explosive)
            {
                projectileScript.SetProjectileProperties(8f, false, true, 0f,0f);
            }
            else if(towerStats.TowerType == TowerType.Piercing)
            {
                projectileScript.SetProjectileProperties(12f, true, false, 0f,0f);
            }
            else if(towerStats.TowerType == TowerType.Slow)
            {
                projectileScript.SetProjectileProperties(10f, false, false, 0.5f, 2f);
            }
            else
            {
                projectileScript.SetProjectileProperties(10f, false, false, 0f, 0f);
            }
            
            projectileScript.Launch();
        }
    }


public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log($"타워가 {damage}를 받았습니다. 현재 체력 : {Health}");

        if (Health <= 0)
        {
            DestroyTower();
        }
    }
    private void DestroyTower()
    {

        Debug.Log("타워가 파괴되었습니다.");
        if (currentTiles != null)
        {
            currentTiles.isOccupied = false; // 타워가 파괴되면 타일의 점유 상태 해제
            currentTiles.currentTower = null;
        }

        Destroy(gameObject);
    }
}
