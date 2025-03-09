using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public Enemy enemy;
    public Animator enemyAnimator;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool isWalking = true;
    private bool isStunned = false;

    private void Update()
    {
        if (isAttacking && !IsPlayingAttackAnimation())
        {
            isAttacking = false;
            enemyAnimator.SetBool("isAttacking", false);
            SetWalkingState(true);
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 애니메이션 종료, Walking 상태로 복귀");
        }
    }
    public void SetAttackState(bool attacking)
    {
        if (enemyAnimator == null)
        {
            Debug.LogError($"[EnemyAnimator] {gameObject.name}: Animator가 NULL입니다!");
            return;
        }

        isAttacking = attacking;
        enemyAnimator.SetBool("isAttacking", attacking);

        if (attacking)
        {
            string attackAnimation = (Random.value > 0.5f) ? "Attack1" : "Attack2";

            if (AnimatorHasParameter(enemyAnimator, attackAnimation))
            {
                enemyAnimator.ResetTrigger("Attack1"); // 트리거 초기화
                enemyAnimator.ResetTrigger("Attack2");

                enemyAnimator.enabled = true;
                enemyAnimator.SetTrigger(attackAnimation);
                Debug.Log($"[EnemyAnimator] {gameObject.name}: {attackAnimation} 애니메이션 실행!");
            }
            else
            {
                Debug.LogError($"[EnemyAnimator] {gameObject.name}: {attackAnimation} 애니메이션이 존재하지 않습니다!");
            }
        }
        else
        {
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 종료, Walking 상태로 복귀");
            enemyAnimator.SetBool("isAttacking", false);
            SetWalkingState(true);
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

        // 애니메이션이 진행 중인지 체크 (normalizedTime이 1.0 이상이면 종료된 것으로 간주)
        bool isPlaying = (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2")) && stateInfo.normalizedTime < 1.0f;

        Debug.Log($"[EnemyAnimator] {gameObject.name}: 현재 애니메이션 상태 - 실행 중: {isPlaying}, NormalizedTime: {stateInfo.normalizedTime}");

        return isPlaying;
    }




    public float GetCurrentAnimationTime()
    {
        if (enemyAnimator == null)
        {
            Debug.LogError($"[EnemyAnimator] {gameObject.name}: Animator가 NULL입니다!");
            return 0.5f; // 기본값
        }

        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length; // 현재 실행 중인 애니메이션 길이 반환
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
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        enemyAnimator.SetBool("isAttacking", false);
        SetWalkingState(true);
        Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 애니메이션 종료 후 Walking 상태 복귀");
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
        if (animator == null) return false;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }
}
