using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create new Monster")]
public class MonsterBase : ScriptableObject
{
    [SerializeField] string monsterName;
    [SerializeField] string description;

    [SerializeField] public Sprite leftSprite;
    [SerializeField] public Sprite rightSprite;

    [SerializeField] MonsterType monsterType1;
    [SerializeField] MonsterType monsterType2;
    [SerializeField] GrowthRate growthRate;

    // Base Stats
    [SerializeField] int health;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speed;

    [SerializeField] int expYield;

    [SerializeField] List<LearnableMove> learnableMoves;


    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast) 
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast) 
        {
            return level * level * level;
        }

        return -1;
    }


    #region Get Stats (Read Only)
    public string MonsterName
    {
        get { return monsterName; }
    }

    public string Description
    {
        get { return description; }
    }

    public MonsterType Type1
    {
        get { return monsterType1; }
    }

    public MonsterType Type2
    {
        get { return monsterType2; }
    }

    public GrowthRate GrowthRate 
    { 
        get { return growthRate; } 
    }

    // Get Base Stats (Read Only)
    public int MaxHealth
    {
        get { return health; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public int ExpYield => expYield;

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
    #endregion

}

public enum MonsterType
{
    None,
    Physical,
    Magical
}

public enum Stat
{
    Attack,
    Defense,
    Speed
}

public enum GrowthRate
{
    Fast,
    MediumFast
}


public class TypeChart
{
    static float[][] chart =
    {
        //                           Phy    Mgc                 
        /* Physical */ new float[] { 1.5f,  0.5f},
        /* Magical  */ new float[] { 0.5f,  1.5f}
    };

    public static float GetEffectiveness(MonsterType attackType, MonsterType defenseType)
    {
        if (attackType == MonsterType.None || defenseType == MonsterType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}



[System.Serializable] //To display the property of this class to inspector
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int levelRequirement;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int LevelRequirement
    {
        get { return levelRequirement; }
    }
}

