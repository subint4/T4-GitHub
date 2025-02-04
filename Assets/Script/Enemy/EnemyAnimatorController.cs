using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public Enemy enemy;
    public Animator enemyAnimator;
    private bool isAttacking = false;
    private bool isDead = false;

    private void Start()
    {
        enemyAnimator.applyRootMotion = false;
    }

    public void SetAttackState(bool attacking)
    {
        if (enemyAnimator != null && !isDead)
        {
            if (isAttacking != attacking)
            {
                isAttacking = attacking;
                enemyAnimator.SetBool("isAttacking", attacking);

                if (attacking)
                {
                    enemyAnimator.SetTrigger("Attack"); // 공격 애니메이션 실행
                }
                else
                {
                    enemyAnimator.SetTrigger("Walk"); // 공격 후 Walk 애니메이션 실행
                }
            }
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

    //공격 애니메이션이 끝날 때 실행됨 (애니메이션 이벤트에서 호출)
    public void OnAttackAnimationEnd()
    {
        if (!isDead && enemy != null)
        {
            enemy.ApplyDamage(); // 타워에 데미지 적용
            enemy.OnAttackAnimationEnd(); // 이동 속도 복구
            SetAttackState(false); // 공격 종료 후 이동 재개
        }
    }
}
