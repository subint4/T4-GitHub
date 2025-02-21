using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public Enemy enemy;
    public Animator enemyAnimator;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool isWalking = true;
    private bool isStunned = false;

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

            if (AnimatorHasParameter(enemyAnimator, attackTrigger))
            {
                enemyAnimator.SetTrigger(attackTrigger);
                enemyAnimator.SetBool("isAttacking", true);
                Debug.Log($"[EnemyAnimator] {gameObject.name}: {attackTrigger} 애니메이션 실행!");
            }
            else
            {
                Debug.LogError($"[EnemyAnimator] {gameObject.name}: {attackTrigger} 트리거가 존재하지 않습니다!");
            }
        }
        else
        {
            enemyAnimator.SetBool("isAttacking", false);
            SetWalkingState(true);
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 종료, Walking 상태로 복귀");
        }
    }

    public void SetWalkingState(bool walking)
    {
        if (enemyAnimator == null || isStunned) return;

        isWalking = walking;
        enemyAnimator.SetBool("isWalking", walking);

        Debug.Log($"[EnemyAnimator] {gameObject.name}: Walking 상태 = {walking}");
    }

    public void SetStunnedState(bool stunned)
    {
        if (enemyAnimator == null) return;

        isStunned = stunned;

        if (stunned)
        {
            enemyAnimator.speed = 0f; // 애니메이션 정지
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 스턴 상태 진입! 애니메이션 정지");
        }
        else
        {
            enemyAnimator.speed = 1f; // 애니메이션 정상 속도로 복구
            SetWalkingState(true);
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 스턴 상태 해제, Walking 애니메이션 재개");
        }
    }

    public void ApplySlowAnimation(bool isSlowed)
    {
        if (enemyAnimator == null) return;

        enemyAnimator.SetBool("isSlowed", isSlowed);
        Debug.Log($"[EnemyAnimator] {gameObject.name}: Slow 애니메이션 적용 = {isSlowed}");
    }

    public bool IsPlayingAttackAnimation()
    {
        if (enemyAnimator == null)
        {
            Debug.LogError($"[EnemyAnimator] {gameObject.name}: Animator가 NULL입니다!");
            return false;
        }

        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = (stateInfo.IsName("attack1") || stateInfo.IsName("attack2")) && stateInfo.normalizedTime < 1.0f;

        Debug.Log($"[EnemyAnimator] {gameObject.name}: 현재 애니메이션 상태 - {isPlaying}, NormalizedTime: {stateInfo.normalizedTime}");
        return isPlaying;
    }

    public void PlayDeathAnimation()
    {
        if (enemyAnimator != null && !isDead)
        {
            isDead = true;
            enemyAnimator.SetTrigger("isDead");
            SetWalkingState(false);
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 사망 애니메이션 실행!");
        }
    }

    public void OnDeathAnimationEnd()
    {
        if (enemy != null)
        {
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 사망 애니메이션 종료, 적 제거");
            Destroy(enemy.gameObject);
        }
    }

    private bool AnimatorHasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}
