using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public MoveBase MoveBase { get; set; }

    public Move (MoveBase moveBase)
    {
        this.MoveBase = moveBase;
    }
}
