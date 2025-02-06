using UnityEngine;

public class TowerAnimatorController : MonoBehaviour
{
    public Tower tower;
    public Animator towerAnimator;
    private bool isAttacking = false;
    private bool isDead = false;
    // 애니메이션 이벤트에서 호출되는 투사체 발사 메서드
    public void FireProjectile()
    {
        if (tower != null)
        {
            tower.Attack();
        }
    }

    // 애니메이션 상태 설정
    public void SetAttackState(bool attacking)
    {
        if (towerAnimator != null)
        {
            if (isAttacking != attacking)
            {
                isAttacking = attacking;
                towerAnimator.SetBool("isAttacking", attacking);
            }
        }
    }

    // 애니메이션 실행 중인지 확인하는 메서드
    public bool IsPlayingAttackAnimation()
    {
        return towerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throwing");
    }

    // 애니메이션이 끝났을 때 호출 (애니메이션 이벤트에서 실행)
    public void OnAttackAnimationEnd()
    {
        SetAttackState(false);
    }
    public void PlayDeathAnimation()
    {
        if (towerAnimator != null && !isDead)
        {
            isDead = true;
            towerAnimator.SetTrigger("isDead");
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 사망 애니메이션 실행!");
        }
    }
    public void OnDeathAnimationEnd()
    {
        if (tower != null)
        {
            tower.DestroyTower();
        }
    }

}
