using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public int EnemyID;
    public EnemySO enemyStats;
    public bool isDead = false;
    private bool isAttacking = false;
    private bool isSlowed = false;
    private bool isStunned = false;
    private float health;
    private float attackPower;
    private float originalSpeed;
    private float attackSpeed;
    public float MovementSpeed;
    public Tower currentTarget;
    public EnemyAnimatorController enemyAnimatorController;
    private Rigidbody2D rb;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 1.5f;
    private List<GameObject> bossTargets = new List<GameObject>(); // 보스는 다중 타겟 가능
    private Coroutine stunCoroutine; // 현재 실행 중인 스턴 코루틴 저장
    private bool isStunImmune = false; // 스턴 면역 여부
    public Sprite defaultSprite;
    private Animator animator;

    private void Start()
    {
        enemyAnimatorController = GetComponent<EnemyAnimatorController>() ?? GetComponentInChildren<EnemyAnimatorController>();
        if (enemyAnimatorController.enemyAnimator == null)
            enemyAnimatorController.enemyAnimator = GetComponent<Animator>();

        if (!enemyAnimatorController.enemyAnimator.isActiveAndEnabled)
        {
            Debug.LogError($"[EnemyAnimator] {gameObject.name}: Animator가 비활성화되어 있음! 활성화합니다.");
            enemyAnimatorController.enemyAnimator.enabled = true;
        }
        if (enemyAnimatorController == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: EnemyAnimatorController를 찾을 수 없습니다! 애니메이션 실행 불가.");
        }
        else
        {
            Debug.Log($"[Enemy] {gameObject.name}: EnemyAnimatorController 정상 할당됨.");
            enemyAnimatorController.gameObject.SetActive(true);
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (spriteRenderer.sprite == null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;  // 기본 스프라이트 할당
        }
        else if (spriteRenderer.sprite == null && defaultSprite == null)
        {
            Debug.LogWarning("Sprite가 설정되지 않았습니다: " + gameObject.name);
        }
        enemyAnimatorController.SetWalkingState(true);
        StartCoroutine(AttackLoop());
    }

    public void Initialize(EnemySO enemyData, EnemyType type)
    {
        if (enemyData == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: enemyStats가 NULL입니다! 초기화 실패.");
            return;
        }

        enemyStats = enemyData;
        enemyStats.Type = type;

        health = enemyStats.Health;
        attackPower = enemyStats.AttackPower;
        attackSpeed = enemyStats.AttackSpeed;
        MovementSpeed = enemyStats.MovementSpeed;

        transform.localScale = new Vector3(-1, 1, 1);

        Debug.Log($"[Enemy] {gameObject.name}: 초기화 완료! 타입: {enemyStats.Type}, 체력: {health}, 공격력: {attackPower}, 이동속도: {MovementSpeed}");
    }


    private void Update()
    {
        if (!isDead && !isAttacking && !isStunned)
        {
            transform.Translate(Vector3.left * MovementSpeed * Time.deltaTime);
        }

        if (!isDead)
        {
            FindTarget();
        }
    }

    private void FindTarget()
    {
        float detectionRange = attackRange;
        float maxYOffset = 0.3f; // 일반 적 탐지 Y축 허용 범위
        Collider2D[] towers = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistanceX = float.MaxValue;
        Tower closestTower = null;

        bossTargets.Clear(); // 보스 타겟 리스트 초기화

        foreach (var tower in towers)
        {
            if (tower.CompareTag("Tower"))
            {
                float xDifference = Mathf.Abs(tower.transform.position.x - transform.position.x);
                float yDifference = Mathf.Abs(tower.transform.position.y - transform.position.y);
                Tower towerComponent = tower.GetComponent<Tower>(); // Tower 컴포넌트 가져오기

                if (towerComponent == null)
                    continue;

                if (CompareTag("Boss"))
                {
                    // 보스는 Y축 관계없이 모든 감지된 타워 저장
                    bossTargets.Add(tower.gameObject);
                }
                else
                {
                    // 일반 타워는 X축 기준 가장 가까운 대상만 선택 (Y축 오차 허용)
                    if (xDifference < detectionRange && yDifference <= maxYOffset && xDifference < closestDistanceX)
                    {
                        closestDistanceX = xDifference;
                        closestTower = towerComponent; // Tower 타입으로 저장
                    }
                }
            }
        }

        if (CompareTag("Boss") && bossTargets.Count > 0)
        {
            Debug.Log($"[Boss] {gameObject.name}이(가) {bossTargets.Count}개의 타워를 감지함.");
        }

        // 일반 타워 타겟 설정
        currentTarget = closestTower;
    }

    private IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            if (isStunned)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            if (CompareTag("Boss") && bossTargets.Count > 0)
            {
                Debug.Log($"[Boss] {gameObject.name}이(가) {bossTargets.Count}개의 타워를 동시에 공격!");

                // 보스는 감지한 모든 타워를 동시에 공격
                AttackMultipleTargets();

                yield return new WaitForSeconds(enemyStats.AttackSpeed);
            }
            else
            {
                if (currentTarget == null || !IsTargetInRange(currentTarget.gameObject))
                {
                    FindTarget();
                }

                if (currentTarget != null && IsTargetInRange(currentTarget.gameObject) && !isAttacking)
                {
                    Debug.Log($"[Enemy] {gameObject.name} 공격 실행! 대상: {currentTarget.name}");
                    isAttacking = true;
                    AttackTarget(currentTarget.gameObject);
                    yield return new WaitForSeconds(enemyStats.AttackSpeed);
                    isAttacking = false;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void AttackMultipleTargets()
    {
        if (bossTargets.Count == 0) return;

        foreach (var target in bossTargets)
        {
            if (target != null && IsTargetInRange(target))
            {
                AttackTarget(target);
            }
        }
    }

    private void AttackTarget(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: 공격 대상이 NULL입니다!");
            return;
        }

        Tower targetTower = target.GetComponent<Tower>();

        if (targetTower == null)
        {
            Debug.LogError($"[Enemy] {gameObject.name}: {target.name}에서 Tower 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        if (enemyStats.Type == EnemyType.Melee)
        {
            Debug.Log($"[Enemy] {gameObject.name} (Melee)이(가) {target.name}에게 근접 공격 실행!");
            StartMeleeAttack(target);
        }
        else if (enemyStats.Type == EnemyType.Ranged)
        {
            Debug.Log($"[Enemy] {gameObject.name} (Ranged)이(가) {target.name}에게 원거리 공격 실행!");
            StartRangedAttack(target);
        }
        else
        {
            Debug.LogError($"[Enemy] {gameObject.name}: 알 수 없는 공격 유형 ({enemyStats.Type})");
        }
    }





    private bool IsTargetInRange(GameObject target)
    {
        if (target == null) return false;

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        bool inRange = distanceToTarget <= attackRange;

        Debug.Log($"[Enemy] {gameObject.name} -> {target.name} 거리: {distanceToTarget}, 공격 범위 내 여부: {inRange}");
        return inRange;
    }





    private void StartMeleeAttack(GameObject target)
    {
        Debug.Log($"공격상태 확인 {isAttacking}");

        MovementSpeed = 0;
        enemyAnimatorController.SetWalkingState(false);
        enemyAnimatorController.SetAttackState(true); // 애니메이션 실행

        Debug.Log($"[Enemy] {gameObject.name} StartMeleeAttack 호출됨! Target: {target.name}");

        StartCoroutine(MeleeAttackRoutine(target));
    }


    private IEnumerator MeleeAttackRoutine(GameObject target)
    {
        if (target == null)
        {
            isAttacking = false;
            yield break;
        }

        // 공격 애니메이션 실행
        enemyAnimatorController.SetAttackState(true);
        MovementSpeed = 0;

        // 공격 속도의 절반 동안 대기 (애니메이션 초기 모션)
        yield return new WaitForSeconds(enemyStats.AttackSpeed / 2);

        // 대상이 여전히 범위 내에 있는지 확인하고 데미지 적용
        if (target != null && IsTargetInRange(target))
        {
            Tower targetTower = target.GetComponent<Tower>();
            if (targetTower != null)
            {
                targetTower.TakeDamage(attackPower);
                Debug.Log($"[Enemy] {gameObject.name}이(가) {target.name}에게 {attackPower}의 피해를 입힘!");

                if (targetTower.IsDestroyed())
                {
                    Debug.Log($"[Enemy] {target.name}가 파괴됨. 새로운 타겟 탐색.");
                    FindTarget();
                }
            }
        }

        // 공격 속도의 나머지 절반 동안 대기 (공격 후 딜레이)
        yield return new WaitForSeconds(enemyStats.AttackSpeed / 2);

        // 공격 종료 후 상태 복구
        isAttacking = false;
        MovementSpeed = enemyStats.MovementSpeed;
        enemyAnimatorController.SetWalkingState(true);
        enemyAnimatorController.SetAttackState(false);
    }




    private void StartRangedAttack(GameObject target)
    {
        if (isAttacking || target == null) return; // 이미 공격 중이거나 대상이 없으면 실행 안 함

        isAttacking = true;
        enemyAnimatorController.SetAttackState(true);

        Debug.Log($"[Enemy] {gameObject.name} StartRangedAttack 호출됨! Target: {target.name}");

        StartCoroutine(RangedAttackRoutine(target));
    }



    private IEnumerator RangedAttackRoutine(GameObject target)
    {
        if (target == null)
        {
            isAttacking = false;
            yield break;
        }

        yield return new WaitForSeconds(enemyStats.AttackSpeed / 2); // 공격 모션 시작

        if (!CompareTag("Boss")) // 보스는 원거리 공격 안 함
        {
            FireProjectile(target);
        }

        yield return new WaitForSeconds(enemyStats.AttackSpeed / 2); // 공격 종료 대기

        isAttacking = false;
        enemyAnimatorController.SetWalkingState(true);
        enemyAnimatorController.SetAttackState(false);
    }


    private void FireProjectile(GameObject target)
    {
        if (target == null || projectilePrefab == null || firePoint == null) return;

        GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectileObj.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            Vector3 direction = (target.transform.position - firePoint.position).normalized;
            projectileScript.Initialize(attackDamage: enemyStats.AttackPower, direction: direction);

            Debug.Log($"[Enemy] {gameObject.name}가 {target.name}에게 투사체 발사!");
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndLine"))
        {
            Debug.Log($"[Enemy] {gameObject.name}: EndLine에 도달! 게임 종료.");

            // **GameManager를 직접 찾음**
            GameManager gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();

            // **GameManager가 존재하는 경우만 GameOver 호출**
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
            else
            {
                Debug.LogError("[Enemy] GameManager 인스턴스를 찾을 수 없습니다!");
            }
        }
    }



    public void ApplySlow(float slowFactor, float duration)
    {
        if (!isSlowed)
        {
            originalSpeed = MovementSpeed;
            float adjustedSlowFactor = Mathf.Clamp(1f - slowFactor, 0.1f, 1f);
            MovementSpeed *= adjustedSlowFactor;
            attackSpeed /= adjustedSlowFactor;
            isSlowed = true;
        }
        Invoke(nameof(EndSlow), duration);
    }

    private void EndSlow()
    {
        MovementSpeed = originalSpeed;
        attackSpeed = enemyStats.AttackSpeed;
        isSlowed = false;
    }


    public void ApplyStun(float duration, bool isFromItem = false)
    {
        if (isDead) return; // 이미 죽은 상태라면 스턴 적용하지 않음

        // 공격으로 거는 스턴은 보스에게 적용되지 않음
        if (!isFromItem && CompareTag("Boss")) return;

        // 이미 스턴 중이거나 스턴 면역이면 적용 안 함
        if (isStunned || isStunImmune) return;

        Debug.Log($"[Enemy] {gameObject.name}: 스턴 적용 - {duration}초 동안 행동 불가 (출처: {(isFromItem ? "아이템" : "공격")})");

        isStunned = true;
        MovementSpeed = 0f;
        isAttacking = false;

        if (enemyAnimatorController != null)
        {
            enemyAnimatorController.SetWalkingState(false);
            enemyAnimatorController.SetAttackState(false);
        }

        // 기존 스턴이 걸려있는 경우, 새로운 스턴 적용 안 함
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        // 새로운 스턴 코루틴 시작
        stunCoroutine = StartCoroutine(StunDuration(duration));
    }

    private IEnumerator StunDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        isStunned = false;
        MovementSpeed = enemyStats.MovementSpeed; // 원래 이동 속도로 복구
        isAttacking = false;

        if (enemyAnimatorController != null)
        {
            enemyAnimatorController.SetWalkingState(true);
        }

        Debug.Log($"[Enemy] {gameObject.name}: 스턴 해제됨");

        // **스턴 해제 후 5초간 스턴 면역 적용**
        StartCoroutine(StunImmunityCooldown(2f));
    }

    private IEnumerator StunImmunityCooldown(float immunityDuration)
    {
        isStunImmune = true;
        Debug.Log($"[Enemy] {gameObject.name}: 스턴 면역 시작 ({immunityDuration}초)");

        yield return new WaitForSeconds(immunityDuration);

        isStunImmune = false;
        Debug.Log($"[Enemy] {gameObject.name}: 스턴 면역 종료");
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        Debug.Log("TakeDamage 호출됨!");
        Debug.Log("BlinkTrigger 실행됨!");

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        enemyAnimatorController.SetWalkingState(false);
        enemyAnimatorController.PlayDeathAnimation();


        

        // **골드 지급 기능 추가**
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(enemyStats.RewardMoney, true); // 적의 보상금 지급
        }

        Destroy(gameObject, 1.5f); // 애니메이션 후 제거
    }

    public bool IsPlayingAttackAnimation()
    {
        if (enemyAnimatorController.enemyAnimator == null)
        {
            Debug.LogError($"[EnemyAnimator] {gameObject.name}: Animator가 NULL입니다!");
            return false;
        }

        if (!enemyAnimatorController.isActiveAndEnabled)
        {
            Debug.LogError($"[EnemyAnimator] {gameObject.name}: Animator가 비활성화 상태입니다!");
            return false;
        }

        if (enemyAnimatorController.enemyAnimator.runtimeAnimatorController == null)
        {
            Debug.LogError($"[EnemyAnimator] {gameObject.name}: AnimatorController가 설정되지 않았습니다!");
            return false;
        }

        // `GetCurrentAnimatorStateInfo()` 대체 코드
        AnimatorClipInfo[] clipInfo = enemyAnimatorController.enemyAnimator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            string currentAnimation = clipInfo[0].clip.name;
            bool isPlaying = currentAnimation.Contains("Attack");

            Debug.Log($"[EnemyAnimator] {gameObject.name}: 현재 애니메이션 상태 - {currentAnimation}, 실행 중: {isPlaying}");
            return isPlaying;
        }

        Debug.LogWarning($"[EnemyAnimator] {gameObject.name}: 현재 실행 중인 애니메이션이 없습니다!");
        return false;
    }

    public void OnDeathAnimationEnd()
    {        // WaveManager에 적 처치 알림
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.ClearEnemy(this); // 적 객체 전달
        }
        Destroy(gameObject);
    }
}
