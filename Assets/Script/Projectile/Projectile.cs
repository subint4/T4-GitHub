using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class Projectile : MonoBehaviour
{
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    private float damage;
    private float speed;
    private bool canPierce;
    private int pierceCount;
    private bool canSlow;
    private float slowEffect;
    private float slowDuration;
    private bool canStun;
    private float stunDuration;

    private Vector3 moveDirection;
    private float distanceTraveled = 0f;
    private Vector3 startPosition;
    private const float MaxRange = 1000f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        LoadProjectileDataFromJson();
        if (rb != null)
        {
            rb.velocity = moveDirection * speed; // 초기 속도 설정
        }
    }

    public void Initialize(float attackDamage, Vector3 direction)
    {
        damage = attackDamage;
        moveDirection = direction.normalized;
        Debug.Log($"[Projectile] {gameObject.name} 발사됨! 데미지: {damage}, 속도: {speed}, 방향: {moveDirection}");
    }

    private void Update()
    {
        if (rb == null)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }

        distanceTraveled = Vector3.Distance(startPosition, transform.position);
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

                if (canSlow)
                {
                    enemy.ApplySlow(slowEffect, slowDuration);
                }

                if (canStun)
                {
                    enemy.ApplyStun(stunDuration);
                }

                if (canPierce)
                {
                    pierceCount--; // 관통 횟수 감소
                    Debug.Log($"[Projectile] 관통 발생! 남은 관통 횟수: {pierceCount}");

                    if (pierceCount <= 0)
                    {
                        Debug.Log($"[Projectile] 관통 횟수 소진! 삭제됨: {gameObject.name}");
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void LoadProjectileDataFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JsonData/ProjectileData");
        if (jsonFile == null)
        {
            Debug.LogError("[Projectile] JSON 파일을 찾을 수 없습니다! 경로: 'Resources/JsonData/ProjectileData.json'");
            return;
        }

        Debug.Log("[Projectile] JSON 파일 로드 성공!");

        ProjectileDataContainer projectileDataContainer = JsonConvert.DeserializeObject<ProjectileDataContainer>(jsonFile.text);
        if (projectileDataContainer == null || projectileDataContainer.Data == null)
        {
            Debug.LogError("[Projectile] JSON 데이터를 불러오지 못했습니다.");
            return;
        }

        string projectileName = gameObject.name.Replace("(Clone)", "").Trim();
        Debug.Log($"[Projectile] 현재 프리팹 이름: {gameObject.name} → 검색할 이름: {projectileName}");

        ProjectileStats projectileStats = projectileDataContainer.Data.Find(p => p.Name == projectileName);
        if (projectileStats == null)
        {
            Debug.LogError($"[Projectile] '{projectileName}'에 대한 데이터를 JSON에서 찾을 수 없습니다!");
            return;
        }

        speed = projectileStats.Speed;
        canPierce = projectileStats.CanPierce;
        pierceCount = projectileStats.PierceCount;
        canSlow = projectileStats.CanSlow;
        slowEffect = projectileStats.SlowEffect;
        slowDuration = projectileStats.SlowDuration;
        canStun = projectileStats.CanStun;
        stunDuration = projectileStats.StunDuration;

        Debug.Log($"[Projectile] {projectileName} 데이터 적용 완료: Speed={speed}, CanPierce={canPierce}, PierceCount={pierceCount}");
    }
}

[System.Serializable]
public class ProjectileStats
{
    public int id;
    public string Name;
    public float Speed;
    public bool CanPierce;
    public int PierceCount;
    public bool CanStun;
    public float StunDuration;
    public bool CanSlow;
    public float SlowEffect;
    public float SlowDuration;
}

[System.Serializable]
public class ProjectileDataContainer
{
    public List<ProjectileStats> Data;
}
