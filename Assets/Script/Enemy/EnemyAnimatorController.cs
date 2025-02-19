using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public Enemy enemy;
    public Animator enemyAnimator;
    private bool isDead = false;

    private void Start()
    {
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<Animator>();
            if (enemyAnimator == null)
            {
                Debug.LogError($"[EnemyAnimator] {gameObject.name}: Animator를 찾을 수 없습니다!");
            }
        }

        if (enemy == null)
        {
            enemy = GetComponentInParent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError($"[EnemyAnimator] {gameObject.name}: Enemy 스크립트를 찾을 수 없습니다!");
            }
        }
    }

    public void SetAttackState(bool attacking)
    {
        if (enemyAnimator == null || isDead) return;

        enemyAnimator.SetBool("isAttacking", attacking);

        if (attacking)
        {
            if (AnimatorHasParameter(enemyAnimator, "Attack"))
            {
                enemyAnimator.ResetTrigger("Attack");
                enemyAnimator.SetTrigger("Attack");

                Debug.Log($"[EnemyAnimator] {gameObject.name}: Attack 애니메이션 실행!");
            }
            else
            {
                Debug.LogError($"[EnemyAnimator] {gameObject.name}: 'Attack' 파라미터가 Animator에 없습니다!");
            }
        }
        else
        {
            Debug.Log($"[EnemyAnimator] {gameObject.name}: 공격 종료, Idle 상태 전환");
        }
    }

    public bool IsPlayingAttackAnimation()
    {
        if (enemyAnimator == null) return false;

        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = stateInfo.IsName("Attack");

        Debug.Log($"[EnemyAnimator] {gameObject.name}: 현재 애니메이션 상태 - {isPlaying}, " +
                  $"NormalizedTime: {stateInfo.normalizedTime}, Length: {stateInfo.length}");

        return isPlaying && stateInfo.normalizedTime < 1.0f;
    }

    private bool AnimatorHasParameter(Animator animator, string paramName)
    {
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
