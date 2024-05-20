using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;

    [SerializeField] PartyMemberUI[] memberSlots;
    List<Monster> monsters;
    MonsterParty party;

    [SerializeField] int selection = 0;

    public Monster SelectedMember => monsters[selection];
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();

        party = MonsterParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow)) 
            ++selection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selection -= 2;

        selection = Mathf.Clamp(selection, 0, monsters.Count - 1);

        UpdatePartySelection();
        //if (selection != prevSelection)

        if (Input.GetKeyDown(KeyCode.Z))
            onSelected?.Invoke();
        else if(Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();
            
    }

    public void UpdatePartySelection()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (i == selection)
            {
                memberSlots[i].MonsterName.color = Color.blue;
                memberSlots[i].Background.color = Color.yellow;
            }
            else
            {
                memberSlots[i].MonsterName.color = Color.black;
                memberSlots[i].Background.color = Color.white;
            }
        }
    }

    public void SetPartyData()
    {
        monsters = party.Monsters;
        for (int i = 0; i < memberSlots.Length; i++) 
        {
            if (i < monsters.Count)
                memberSlots[i].SetData(monsters[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Monster";
    }
}
