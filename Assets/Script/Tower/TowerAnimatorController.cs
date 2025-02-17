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
            if (isAttacking != attacking)
            {
                isAttacking = attacking;
                towerAnimator.SetBool("isAttacking", attacking);
            }
        }
    }

    public bool IsPlayingAttackAnimation()
    {
        if (towerAnimator == null) return false;
        AnimatorStateInfo stateInfo = towerAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Throwing") && stateInfo.normalizedTime < 1.0f;
    }

    public void OnAttackAnimationEnd()
    {
        if (tower == null || isDead) return;

        isAttacking = false;
        SetAttackState(false);

        tower.FireProjectile(); // 애니메이션 종료 시 투사체 발사 보장

        Debug.Log($"[TowerAnimator] {tower.gameObject.name}: 공격 애니메이션 종료, 다음 공격 준비.");

        // 공격 루프 강제 실행
        tower.Invoke("RestartAttack", tower.towerStats.AttackSpeed);
    }



    public void PlayDeathAnimation()
    {
        if (towerAnimator != null && !isDead)
        {
            isDead = true;
            towerAnimator.SetTrigger("isDead");
        }
    }

    public void OnDeathAnimationEnd()
    {
        if (tower != null && tower.isDead)
        {
            tower.OnDeathAnimationEnd();
        }
    }
}
