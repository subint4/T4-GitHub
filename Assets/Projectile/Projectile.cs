using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [SerializeField]public float speed = 10f;
    [SerializeField]public int damage = 10;
    private Transform target;

    public void Initialize(Transform target)
    {
        this.target = target;
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
            var health = target.GetComponent<HealthSystem>();
            if (health != null) 
            {
                health.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}