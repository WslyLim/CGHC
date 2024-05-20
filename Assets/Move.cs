using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public MoveBase MoveBase { get; set; }
    public int SP { get; set; }
    public bool InfiniteSP { get; set; }

    public Move (MoveBase moveBase)
    {
        this.MoveBase = moveBase;
        this.SP = moveBase.SP;
        this.InfiniteSP = moveBase.InfiniteSP;
    }

    public void IncreaseSP(int amount)
    {
        SP = Mathf.Clamp(SP + amount, 0, MoveBase.SP);
    }
}
