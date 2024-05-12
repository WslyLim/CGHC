using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeamSlot : MonoBehaviour, IDropHandler
{
    public bool isOccupied = false;
    public GameObject currentMonster; // Reference to the Pokémon currently assigned to this slot
    UnitStorageSystem playerTeamClass;

    private void Start()
    {
        playerTeamClass = FindObjectOfType<UnitStorageSystem>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (isOccupied) 
        {
            RemoveMonsterFromSlot();
        }

        if (eventData.pointerDrag != null) 
        {
            currentMonster = eventData.pointerDrag;
            DragDrop draggableMonsster = currentMonster.GetComponent<DragDrop>();
            draggableMonsster.parentAfterDrag = transform;
            
            AssignMonsterToSlot(currentMonster);
        }

    }

    public void AssignMonsterToSlot(GameObject monster)
    {
        if (!isOccupied)
        {
            currentMonster = monster;
            isOccupied = true;
            Debug.Log(currentMonster);
        }
        else
        {
            Debug.LogWarning("Cannot assign Pokémon. Slot is already occupied.");
        }
    }

    public void RemoveMonsterFromSlot()
    {
        if (currentMonster != null)
        {
            playerTeamClass.SendToStorage(currentMonster);
            currentMonster = null;
            isOccupied = false;
            Debug.Log("Successfully Removed");
        }
    }

    
}
