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
        if (towerAnimator != null && !isDead) // 사망 상태에서는 공격 애니메이션 실행 X
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
        if (towerAnimator == null) return false; // Null 체크 추가
        return towerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throwing");
    }

    // 애니메이션이 끝났을 때 호출 (애니메이션 이벤트에서 실행)
    public void OnAttackAnimationEnd()
    {
        if (tower == null || isDead) return; // 타워가 사망했거나 삭제된 경우 실행 X

        isAttacking = false;
        SetAttackState(false);
        Debug.Log($"[TowerAnimator] {gameObject.name}: 공격 애니메이션 종료됨.");
    }

    public void PlayDeathAnimation()
    {
        if (towerAnimator != null && !isDead)
        {
            isDead = true;
            towerAnimator.SetTrigger("isDead");
            Debug.Log($"[TowerAnimator] {gameObject.name}: 사망 애니메이션 실행!");
        }
    }

    public void OnDeathAnimationEnd()
    {
        if (tower != null && tower.isDead) // 사망 상태 확인 후 실행
        {
            Debug.Log($"[TowerAnimator] {gameObject.name}: 사망 애니메이션 종료 후 타워 삭제!");
            tower.DestroyTower();
        }
    }
}
