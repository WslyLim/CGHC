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
        if (revive || maxRevive)
        {
            if (monster.HP > 0)
                return false;

            if (revive)
            {
                monster.IncreaseHP(monster.MaxHealth / 2);
            }
            else if (maxRevive)
            {
                monster.IncreaseHP(monster.MaxHealth);

            }

            return true;
        }

        if (monster.HP == 0) return false;

        if (restoreMaxHP || hpAmount > 0)
        {
            if (monster.HP == monster.MaxHealth) 
            {
                return false;
            }

            if (restoreMaxHP)
                monster.IncreaseHP(monster.MaxHealth);
            else
                monster.IncreaseHP(hpAmount);
        }


        // Restore SP
        if (restoreMaxSP)
        {
            monster.Moves.ForEach(m => m.IncreaseSP(m.MoveBase.SP));
        }
        else if (spAmount > 0) 
        {
            monster.Moves.ForEach(m => m.IncreaseSP(spAmount));
        }
             
        return true;
    }
}
