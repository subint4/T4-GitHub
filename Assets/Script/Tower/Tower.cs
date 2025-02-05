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

        if (projectileScript != null)
        {
            projectileScript.SetAttackPower(towerStats.AttackPower);
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
