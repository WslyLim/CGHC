using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Monster> wildMonsters;
    [SerializeField] int monsterAmount;

    public Monster GetRandomWildMonsters()
    {
        var wildMonster =  wildMonsters[Random.Range(0, wildMonsters.Count)];
        wildMonster.Init();
        return wildMonster;
    }

    public int MonsterAmount
    {
        get 
        { 
            return monsterAmount; 
        }
    }

}
