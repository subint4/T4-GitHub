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
                Debug.Log($"[TowerAnimator] {tower.gameObject.name}: 공격 애니메이션 실행!");
            }
            else
            {
                Debug.Log($"[TowerAnimator] {tower.gameObject.name}: 공격 종료, Idle 상태 전환");
            }
        }
        else
        {
            Debug.LogError($"[TowerAnimator] {tower.gameObject.name}: 공격 애니메이션 실행 실패! Animator 또는 isDead 상태 확인 필요.");
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

    public void OnDeathAnimationEnd()
    {
        tower.OnDeathAnimationEnd();
    }

    public void OnAttackAnimationEnd()
    {
        tower.OnAttackAnimationEnd();
    }
}
