using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] BattleHUD hud;
    [SerializeField] MonsterBase monsterBase;
    [SerializeField] int level;
    public bool isPlayerUnit;
    public bool HasTakenTurn;

    public Monster Monster { get; set; }

    public BattleHUD HUD { get { return hud; } }

    // Start is called before the first frame update
    public void Setup(Monster monster)
    {
        Monster = monster;
        if (isPlayerUnit)
        {
            GetComponent<Image>().sprite = Monster.Base.leftSprite;
        }
        else 
        {
            GetComponent<Image>().sprite = Monster.Base.rightSprite;
        }

        hud.SetData(monster);
        hud.SetExp();
        hud.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void CleanUp()
    {
        HasTakenTurn = false;
        hud.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
