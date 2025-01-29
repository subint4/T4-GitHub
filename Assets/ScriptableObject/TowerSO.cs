using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "NewTower", menuName = "Tower/Tower Data")]
public class TowerSO : ScriptableObject
{
        public string TowerName;
        public TowerStat BaseStat;
        public string Description;

    [System.Serializable]
    public class TowerStat
    {
        public int TowerBaseHealth;           // 기본 체력
        public int TowerBaseDamage;           // 기본 공격력
        public int TowerBaseDeployCost;       // 기본 배치 비용
        public float TowerBaseAttackSpeed;    // 기본 공격 속도
    }

}
