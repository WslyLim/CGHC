using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapArea : MonoBehaviour
{
    [SerializeField] List<Monster> wildMonsters;
    [SerializeField] int monsterAmount;
    [SerializeField] int minimumLevel;
    [SerializeField] int maximumLevel;
    public Monster GetRandomWildMonsters()
    {
        var wildMonster =  wildMonsters[Random.Range(0, wildMonsters.Count)];
        var newMonster = new Monster(wildMonster); // Create a new instance using the copy constructor
        newMonster.Level = GetRandomLevel();
        newMonster.Init();
        return newMonster;
    }

    public int MonsterAmount
    {
        get 
        { 
            return monsterAmount; 
        }
    }

    public int GetRandomLevel()
    {
        return Random.Range(minimumLevel, maximumLevel + 1); // +1 because Random.Range for integers is exclusive of the max value
    }
}
