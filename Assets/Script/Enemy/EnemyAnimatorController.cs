using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public Enemy enemy;
    public Animator enemyAnimator;
    private bool isDead = false;

    private void Start()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.applyRootMotion = false;
        }

        if (enemy == null)
        {
            enemy = GetComponent<Enemy>(); // Enemy 스크립트 자동 연결
        }
    }

    public void SetAttackState(bool attacking)
    {
        if (enemyAnimator != null && !isDead)
        {
            if (attacking)
            {
                // 50% 확률로 Attack1 또는 Attack2 실행
                int attackType = Random.Range(0, 2); // 0 또는 1 반환
                if (attackType == 0)
                {
                    enemyAnimator.SetTrigger("Attack1");
                    Debug.Log($"[EnemyAnimator] {gameObject.name}: Attack1 실행!");
                }
                else
                {
                    enemyAnimator.SetTrigger("Attack2");
                    Debug.Log($"[EnemyAnimator] {gameObject.name}: Attack2 실행!");
                }
            }
            enemyAnimator.SetBool("isAttacking", attacking);
        }
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

    public void OnAttackAnimationEnd()
    {
        if (!enemy.isDead && enemy.currentTarget != null)
        {
            // 타겟이 살아있는지 다시 확인
            if (enemy.currentTarget.IsDestroyed())
            {
                Debug.Log($"[EnemyAnimator] {gameObject.name}: 타겟이 이미 파괴됨 -> 공격 취소");
                enemy.StopAttack();  // 공격 중지
                return;
            }

            Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 애니메이션 종료 감지됨! {enemy.currentTarget.name}에게 {enemy.attackPower} 피해!");

            // 현재 타겟에게 피해를 줌 (여기서 null 체크 추가)
            enemy.currentTarget.TakeDamage(enemy.attackPower);

            // 공격 딜레이 후 다시 공격
            enemy.Invoke("RestartAttack", enemy.attackSpeed);
        }
    }

}
