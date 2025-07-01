using System.Collections.Generic;
using UnityEngine;
using static EffectManager;

public class EffectManager : MonoBehaviour
{

    public static EffectManager Instance { get; private set; }

    public GameObject effectPrefab; // 하나의 이펙트 프리팹만 사용

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayEffect(Vector3 position, string effectType)
    {
        if (effectPrefab == null)
        {
            Debug.LogError("[EffectManager] 이펙트 프리팹이 없습니다!");
            return;
        }

        GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
        Animator effectAnimator = effect.GetComponent<Animator>();

        if (effectAnimator != null)
        {
            // 애니메이션 트리거 실행
            switch (effectType)
            {
                case "Bomb":
                    effectAnimator.SetInteger("EffectType", 0);
                    break;
                case "Rocket":
                    effectAnimator.SetInteger("EffectType", 1);
                    break;
                case "Stun":
                    effectAnimator.SetInteger("EffectType", 2);
                    break;
            }
            effectAnimator.SetTrigger("PlayEffect");
        }
        else
        {
            Debug.LogError("[EffectManager] Animator가 없습니다!");
        }

        Destroy(effect, 1.5f); // 1.5초 후 삭제
    }
}
