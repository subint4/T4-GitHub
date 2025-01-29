using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossController : EnemyController
{
    public BossSO bossData;
    protected override void Start()
    {
        base.Start();

        transform.localScale *= bossData.scaleMultiplier;

        var collider = GetComponent<Collider2D>();
        if(collider != null)
        {
            collider.offset *= bossData.scaleMultiplier;
            collider.transform.localScale *= bossData.scaleMultiplier;
        }
        moveSpeed = bossData.moveSpeed; // BossSO의 moveSpeed 값으로 덮어쓰기

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var tower = collision.GetComponent<BaseTower>();
        if(tower != null)
        {
            tower.TakeDamage(bossData.areaDamage);
        }

    }
}
