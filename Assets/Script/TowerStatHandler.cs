using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStatHandler : MonoBehaviour
{
    private TowerStat baseStat;
    public TowerStat CurrentStat {  get; private set; }

    public TowerSO CurrentTowerTier {  get; private set; }

    private List<StatModifier> modifiers;

    protected virtual void Awake()
    {
        modifiers = new List<StatModifier>();
        CurrentStat = new TowerStat();
    }
    public void Initialized(TowerStat baseStat)
    {
        this.baseStat = baseStat;
        CalculateFinalStat();    
    }

    public virtual void InitializeByKey(int key)
    {

    }

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        CalculateFinalStat();
    }

    public void AddModifier(StatModifier[] modifiersArray)
    {
        this.modifiers.AddRange(modifiersArray);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        modifiers.Remove(modifier);
        CalculateFinalStat();
    }

    public void RemoveModifier(StatModifier[] modifiersArray)
    {
        foreach (StatModifier modifier in modifiersArray)
        {
            modifiers.Remove(modifier);
        }
        CalculateFinalStat();
    }
    void CalculateFinalStat()
    {

    }
}
