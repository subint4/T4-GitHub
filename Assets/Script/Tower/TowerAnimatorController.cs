using UnityEngine;

public class TowerAnimatorController : MonoBehaviour
{
    public Tower tower;
    public Animator towerAnimator;
    private bool isAttacking = false;
    private bool isDead = false;
    public void SetAttackState(bool attacking)
    {
        if (towerAnimator != null && !isDead)
        {
            isAttacking = attacking;
            towerAnimator.SetBool("isAttacking", attacking);

            if (attacking)
            {
                // 애니메이션 강제 실행 (기존 트리거 방식에서 직접 실행으로 변경)
                towerAnimator.Play("Attack", 0, 0f);
                Debug.Log($"[TowerAnimator] {tower.gameObject.name}: 공격 애니메이션 실행!");
            }
            else
            {
                Debug.Log($"[TowerAnimator] {tower.gameObject.name}: 공격 종료, Idle 상태 전환");
            }
        }
    }

    public bool IsPlayingAttackAnimation()
    {
        if (towerAnimator == null)
        {
            Debug.LogError($"[TowerAnimator] {tower.gameObject.name}: Animator가 NULL입니다!");
            return false;
        }

        AnimatorStateInfo stateInfo = towerAnimator.GetCurrentAnimatorStateInfo(0);

        // Attack 애니메이션이 실행 중인지 체크 (0.98f 대신 1.0f로 정확하게 끝날 때 감지)
        bool isPlaying = stateInfo.IsName("Attack") && stateInfo.normalizedTime < 1.0f;

        Debug.Log($"[TowerAnimator] {tower.gameObject.name}: 현재 애니메이션 상태 - {isPlaying}, NormalizedTime: {stateInfo.normalizedTime}");

        return isPlaying;
    }



    public void PlayDeathAnimation()
    {
        if (towerAnimator != null && !isDead)
        {
            isDead = true;
            towerAnimator.SetTrigger("isDead");
            Debug.Log($"[TowerAnimator] {tower.gameObject.name}: 사망 애니메이션 실행!");
        }
    }
    public void OnAttackAnimationEvent()
    {
        if (tower != null)
        {
            tower.FireProjectile();
        }
    }

    public void OnDeathAnimationEnd()
    {
        tower.OnDeathAnimationEnd();
    }
    public void OnAttackAnimationEnd()
    {
        if (tower != null)
        {
            tower.OnAttackAnimationEnd();
        }
    }
}
