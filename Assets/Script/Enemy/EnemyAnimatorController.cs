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
    }

    public void SetAttackState(bool attacking)
    {
        if (enemyAnimator != null && !isDead)
        {
            enemyAnimator.SetBool("isAttacking", attacking);
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 애니메이터 isAttacking = {attacking}");
        }
    }

    public void PlayDeathAnimation()
    {
        if (enemyAnimator != null)
        {
            isDead = true;
            enemyAnimator.SetTrigger("isDead");
        }
    }

    public void OnAttackAnimationEnd()
    {
        if (enemy != null)
        {
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 애니메이션 종료 감지됨!");
            enemy.StartCoroutine(enemy.ResetAttack()); // 공격 반복 실행
        }
    }
}
