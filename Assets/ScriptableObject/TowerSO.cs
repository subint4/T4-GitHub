using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "NewTower", menuName = "Tower/Tower Data")]
public class TowerSO : ScriptableObject
{
        public string TowerName;
        public GameObject towerPrefab;
        public TowerStat BaseStat;
        public string Description;

    [System.Serializable]
    public class TowerStat
    {
        public int baseHealth;           // 기본 체력
        public int baseDamage;           // 기본 공격력
        public int baseDeployCost;       // 기본 배치 비용
        public float baseAttackSpeed;    // 기본 공격 속도
    }

}
