using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [SerializeField]public float speed = 10f;
    private int damage = 10;
    private Transform target;

    public void Initialize(Transform target,int damage)
    {
        this.target = target;
        this.damage = damage;
        if (target == null)
        {
            Debug.LogError("Projectile initialiezd with a null target");
            Destroy(gameObject);
            return;
        }
        Debug.Log($"Projectile initialized with target: {target.name}");
        Debug.Log($"Projectile initialized with damage: {damage}");
    }
    private void Update()
    {
        if(target == null)
        {
            Debug.LogWarning("target lost. destroying projectile");
            Destroy(gameObject);
            return;
        }
        Vector2 direction = (target.position - transform.position).normalized;
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if(target!=null)
        {
            var enemy = target.GetComponent<EnemyController>();
            var tower = target.GetComponent<BaseTower>();
            if (enemy != null) 
            {
                enemy.TakeDamage(damage);
            }
            else if(tower != null)
            {
                tower.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}