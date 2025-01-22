using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMob ", menuName = "Mob/Mob Data")]

public class EnemySO : ScriptableObject
{
    public string MobName;
    public int baseDamage;
}
