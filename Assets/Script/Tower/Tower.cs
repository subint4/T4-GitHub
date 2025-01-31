using OfficeOpenXml.Drawing.Chart.ChartEx;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerSO towerStats;
    public Transform firePoint;
    public GameObject projectilePrefab;

    private int Health;
    private float attackCooldown = 0f;

    private float detectionRange = 1000f;
    [SerializeField] private LayerMask enemyLayer;
    private void Start()
    {
        if (towerStats != null)
        {
            Health = towerStats.Health;
            Debug.Log($"타워 초기화 완료. 체력 :{Health}");
        }
        else
        {
            Debug.LogError("타워 스탯이 연결되지 않았습니다.");
        }
    }
    public void Update()
    {
        attackCooldown -= Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right,detectionRange,enemyLayer);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            if (attackCooldown <= 0f)
            {
                Debug.Log($"{hit.collider.gameObject.name}을(를) 감지하여 공격!");

                Attack(hit.collider.gameObject);
                attackCooldown = towerStats.AttackSpeed;
            }
        }
    }
    private void Attack(GameObject target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("투사체 프리팹이 연결되지 않았습니다!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetTarget(target, towerStats.AttackPower);
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
            Destroy(gameObject);
        }
    } 

