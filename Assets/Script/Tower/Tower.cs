using UnityEngine;
using System.Collections;
public class Tower : MonoBehaviour
{
    public TowerSO towerStats;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public TowerAnimatorController animatorController;
    public Tiles currentTiles;
    public bool isDead = false;
    private UpgradeSystem upgradeSystem;
    private UpgradeUI upgradeUI;

    public int Health;
    private float attackCooldown = 0f;
    private GameObject currentTarget;
    private float detectionRange = 1000f;
    [SerializeField] private LayerMask enemyLayer;

    private void Start()
    {
        if (animatorController == null)
        {
            animatorController = GetComponent<TowerAnimatorController>();
            if (animatorController == null)
            {
            Animator animator = GetComponent<Animator>();
                if(animator != null)
                {
                    animatorController = animator.GetComponent<TowerAnimatorController>();
                }
            }
            Debug.LogError($"{gameObject}:null");
        }
        if (towerStats != null)
        {
            Health = towerStats.Health;
        }
        upgradeSystem = FindObjectOfType<UpgradeSystem>();
        upgradeUI = FindObjectOfType<UpgradeUI>();
    }

    private void OnMouseDown()
    {
        if(upgradeSystem != null && upgradeUI != null)
        {
            upgradeSystem.SelectTower(this);
            upgradeUI.ShowUpgradeButton(this);
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
        projectileScript.SetDamage(towerStats.AttackPower);
    }


public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log($"타워가 {damage}를 받았습니다. 현재 체력 : {Health}");

        if (Health <= 0)
        {
            isDead = true;
            animatorController.PlayDeathAnimation();
        }
    }
    public void DestroyTower()
    {
        if(Health <= 0)
        {

        Debug.Log("타워가 파괴되었습니다.");
        if (currentTiles != null)
        {
            currentTiles.isOccupied = false; // 타워가 파괴되면 타일의 점유 상태 해제
            currentTiles.currentTower = null;
        }
        if(animatorController!=null)
            {
                animatorController.PlayDeathAnimation();
                StartCoroutine(DestroyAfterAnimation());
            }
        }
    }
    public bool UpgradeTower()
    {
        if (ResourceManager.Instance.SpendGoldForTower(towerStats, true)) // 업그레이드 비용 차감
        {
            Debug.Log($"타워 업그레이드 완료! 업그레이드 비용: {towerStats.UpgradeCost}");
            return true;
        }
        else
        {
            Debug.Log("골드 부족으로 타워 업그레이드 불가!");
            return false;
        }
    }
    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);   
    }
}
