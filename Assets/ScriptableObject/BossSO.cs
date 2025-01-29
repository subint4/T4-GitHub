using UnityEngine;

[CreateAssetMenu(fileName = "NewBoss", menuName = "Enemy/Boss Data")]
public class BossSO : EnemySO
{
    public float scaleMultiplier; // Boss 스케일 조절
    public int areaDamage;        // 충돌 시 피해량
}