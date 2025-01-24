using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "NewTower", menuName = "Tower/Tower Data")]
public class TowerSO : ScriptableObject
{
        public string TowerName;
        public TowerStat BaseStat;
        public float AttackRange;
        public string Description;
}
