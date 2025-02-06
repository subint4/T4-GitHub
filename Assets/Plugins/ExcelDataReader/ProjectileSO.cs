using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileSO", menuName = "Projectiles/ProjectileSO", order = 2)]
public class ProjectileSO : ScriptableObject
{
    public string ProjectileName;
    public float Speed;
    public int Damage;
    public bool CanPierce;
    public int PierceCount;
    public bool CanStun;
    public float StunDuration;
    public bool CanSlow;
    public float SlowEffect;
    public float SlowDuration;
}
