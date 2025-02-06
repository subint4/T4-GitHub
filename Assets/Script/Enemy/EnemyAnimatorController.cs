using Unity.VisualScripting;
using System.Collections;

using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public Enemy enemy;
    public Animator enemyAnimator;
    private bool isDead = false;

    private void Start()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.applyRootMotion = false;
        }

        if (enemy == null)
        {
            enemy = GetComponent<Enemy>(); // Enemy 스크립트 자동 연결
        }
    }


    public void SetAttackState(bool attacking)
    {
        if (enemyAnimator != null && !isDead)
        {
            if (attacking)
            {
                // 50% 확률로 Attack1 또는 Attack2 실행
                int attackType = Random.Range(0, 2); // 0 또는 1 반환
                if (attackType == 0)
                {
                    enemyAnimator.SetTrigger("Attack1");
                    Debug.Log($"[EnemyAnimator] {gameObject.name}: Attack1 실행!");
                }
                else
                {
                    enemyAnimator.SetTrigger("Attack2");
                    Debug.Log($"[EnemyAnimator] {gameObject.name}: Attack2 실행!");
                }
            }
            enemyAnimator.SetBool("isAttacking", attacking);
        }
    }

    public void PlayDeathAnimation()
    {
        if (enemyAnimator != null && !isDead)
        {
            isDead = true;
            enemyAnimator.SetTrigger("isDead");
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 사망 애니메이션 실행!");
        }
    }

    public void OnAttackAnimationEnd()
    {
        if (!isDead && enemy != null)
        {
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 애니메이션 종료 감지됨!");
            enemy.StartCoroutine(enemy.ResetAttack()); // 공격 반복 실행

            enemy.StartCoroutine(DelayedApplyDamage(0.1f));
        }
    }

    public void OnDeathAnimationEnd()
    {
        if (enemy != null)
        {
            enemy.DestroyEnemy();
        }
    }
    private IEnumerator DelayedApplyDamage(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isDead && enemy.currentTarget != null && !enemy.currentTarget.IsDestroyed())
        {
            enemy.ApplyDamage();
        }
    }
}
