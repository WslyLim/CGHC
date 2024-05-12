using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monster/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonsterType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int healingPercentage;
    [SerializeField] MoveCategory moveCategory;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;
    //public bool isSupportSkill;
    public string MoveName
    { get { return moveName; } }

    public string Description
    { get { return description; } }

    public MonsterType Type
    { get { return type; } }

    public int Power
    { get { return power; } }

    public int Accuracy
    { get { return accuracy; } }

    public int HealingPercentage
    { get { return healingPercentage; } }

    public MoveCategory MoveCategory
    { get { return moveCategory; } }

    public MoveEffects Effects 
    { get { return effects; } }

    public MoveTarget Target
    { get { return target; } }
}

public enum MoveCategory
{
    Damage,
    Status,
    Heal
}

public enum MoveTarget
{
    Enemy,
    Self,
    Ally
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts {
        get { return boosts; } 
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

