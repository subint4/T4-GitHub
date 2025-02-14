using UnityEngine;

public class TowerAnimatorController : MonoBehaviour
{
    public Tower tower;
    public Animator towerAnimator;
    private bool isAttacking = false;
    private bool isDead = false;

    public void FireProjectile()
    {
        if (tower != null)
        {
            tower.Attack();
        }
    }

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
        return towerAnimator != null && towerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throwing");
    }

    public void OnAttackAnimationEnd()
    {
        if (tower == null || isDead) return;

        isAttacking = false;
        SetAttackState(false);
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
