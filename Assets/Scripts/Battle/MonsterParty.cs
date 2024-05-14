using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;

    public event Action OnUpdated;

    public List<Monster> Monsters
    {
        get
        {
            return monsters;
        }
        set
        {
            monsters = value;
            OnUpdated?.Invoke();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var monster in monsters) 
        {
            monster.Init();
        }
    }

    //public Monster GetHealthyMonster()
    //{
    //    return monsters.Where(x => x.HP > 0).FirstOrDefault();
    //}

    public void AddMonster(Monster newMonster)
    {
        monsters.Add(newMonster);
        OnUpdated?.Invoke();
    }

    public static MonsterParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<MonsterParty>();
    }
}
