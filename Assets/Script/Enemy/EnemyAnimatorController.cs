using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public Enemy enemy;
    public Animator enemyAnimator;

    private bool isAttacking = false;
    private bool isDead = false;

    public void SetAttackState(bool attacking)
    {
        if (enemyAnimator == null)
        {
            Debug.LogError($"[EnemyAnimator] {gameObject.name}: Animator가 NULL입니다!");
            return;
        }

        isAttacking = attacking;

        if (attacking)
        {
            string attackTrigger = (Random.value > 0.5f) ? "Attack1" : "Attack2";
            enemyAnimator.SetTrigger(attackTrigger);
            enemyAnimator.SetBool("isAttacking", true);
            Debug.Log($"[EnemyAnimator] {gameObject.name}: {attackTrigger} 애니메이션 실행!");
        }
        else
        {
            enemyAnimator.SetBool("isAttacking", false);
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 종료, Idle 상태 전환");
        }
    }

    public bool IsPlayingAttackAnimation()
    {
        if (enemyAnimator == null)
        {
            Debug.LogError($"[EnemyAnimator] {gameObject.name}: Animator가 NULL입니다!");
            return false;
        }

        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = isAttacking && (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2")) && stateInfo.normalizedTime < 1.0f;

        Debug.Log($"[EnemyAnimator] {gameObject.name}: 현재 애니메이션 상태 - {isPlaying}, NormalizedTime: {stateInfo.normalizedTime}");

        return isPlaying;
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

    public void OnDeathAnimationEnd()
    {
        enemy.OnDeathAnimationEnd();
    }
    public void OnAttackAnimationEnd()
    {
        if (!isDead && enemy.currentTarget != null)
        {
            if (enemy.currentTarget.IsDestroyed())
            {
                Debug.Log($"[EnemyAnimator] {gameObject.name}: 타겟이 이미 파괴됨 -> 공격 취소");
                enemy.StopAttack();
                return;
            }

            Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 애니메이션 종료 감지됨! {enemy.enemyStats.AttackPower} 피해!");
            enemy.currentTarget.TakeDamage(enemy.enemyStats.AttackPower);

        }
    }

}
