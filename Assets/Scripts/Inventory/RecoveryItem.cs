using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("SP")]
    [SerializeField] int spAmount;
    [SerializeField] bool restoreMaxSP;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    [Header("Repel")]
    [SerializeField] int repelAmount;

    public override bool Use(Monster monster)
    {
        if (hpAmount > 0)
        {
            if (monster.HP == monster.MaxHealth) 
            {
                return false;
            }

            monster.IncreaseHP(hpAmount);
        }
        return true;
    }
}
