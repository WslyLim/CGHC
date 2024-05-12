using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster Encounter Manager")]
public class MonsterEncounterBase : ScriptableObject
{
    [SerializeField] List<MonsterBase> monsterBaseList;
    [SerializeField] int minimumLevel;
    [SerializeField] int maximumLevel;

    [Range(1, 5)]
    [SerializeField] int numberOfEnemy;

}
