using System.Collections;
using UnityEngine;

public class DieEffectManager : MonoBehaviour
{
    public static DieEffectManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
        
    public void PlayBlinkEffect(Transform target, int blinkCount = 3, float blinkInterval = 0.1f)
    {
        SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            if (spriteRenderer.sprite == null)
            {
                string spritePath = "Sprites/" + target.name.Replace("(Clone)", "").Trim();
                Sprite loadedSprite = Resources.Load<Sprite>(spritePath);

                if (loadedSprite != null)
                {
                    spriteRenderer.sprite = loadedSprite;
                    Debug.Log(" 스프라이트 자동 할당: " + spritePath);
                }
                else
                {
                    Debug.LogWarning("Sprite가 없어서 깜박거림을 실행하지 않습니다: " + target.name);
                    return;
                }
            }

            StartCoroutine(BlinkCoroutine(spriteRenderer, blinkCount, blinkInterval));
        }
        else
        {
            Debug.LogWarning("SpriteRenderer가 없습니다: " + target.name);
        }
    }

    private IEnumerator BlinkCoroutine(SpriteRenderer sprite, int blinkCount, float interval)
    {
        Color originalColor = sprite.color;

        for (int i = 0; i < blinkCount; i++)
        {
            Debug.Log("깜박거림 중: " + sprite.gameObject.name);
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);  // 반투명 (30%)
            yield return new WaitForSeconds(interval);
            sprite.color = originalColor;
            yield return new WaitForSeconds(interval);
        }
    }
}
