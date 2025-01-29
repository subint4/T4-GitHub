using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemySO enemyData;
    private HealthSystem _healthSystem;
    protected float moveSpeed;
    private bool isAttacking = false;
    private bool isStopped = false;

    protected virtual void Start()
    {
        if(enemyData != null)
        {
            _healthSystem = new HealthSystem(enemyData: enemyData, onDeath: Die); 
            moveSpeed =enemyData.moveSpeed;
            
        }
        else
        {
            Debug.LogError("EnemySO가 열결되지 않았습니다.");
        }
    }

    public void Update()
    {
        if (!isStopped)
        {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name}died");

        if(enemyData != null && enemyData.giveRewardOnDeath)
        {
            GiveReward(enemyData.rewardAmount);
        }
        Destroy(gameObject);
    }

    private void GiveReward(int amount)
    {
        PlayerSystem.Instance.AddMoney(enemyData.rewardAmount);
    }

    public void TakeDamage(int damage)
    { 
        _healthSystem.TakeDamage(damage);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Tower"))
        {
            BaseTower tower = collision.GetComponent<BaseTower>();
            if(tower != null && !isAttacking)
            {
                Debug.Log("타워 감지됨");
                isAttacking = true;
                isStopped = true;
                StartCoroutine(AttackTower(tower));
            }
        }
    }   
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            Debug.Log(" 타워에서 벗어남. 이동 다시 시작");
            isAttacking = false;
            isStopped = false; //  이동 다시 시작
            StopAllCoroutines(); // 공격 중지
        }
    }
    private IEnumerator AttackTower(BaseTower tower)
    {
        while(isAttacking)
        {
            if(tower != null)
            {
                tower.TakeDamage(enemyData.baseDamage);
            
                if(tower == null)
                {
                    isStopped = false;
                    yield break;
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
