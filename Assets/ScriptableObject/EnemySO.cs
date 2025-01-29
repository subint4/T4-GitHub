using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy",menuName = "Enemy/Enemy Data")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    public int baseHealth;
    public int baseDamage;
    public float moveSpeed;
    public bool giveRewardOnDeath;
    public int rewardAmount;
}
