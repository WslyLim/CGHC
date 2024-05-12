using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    Character character;
    [SerializeField] Dialog dialog;

    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    [SerializeField] int currentPattern;
    [SerializeField] string npcName;

    NPCState npcState;
    float idleTimer; 
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (npcState == NPCState.Idle) 
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern) 
            {
                idleTimer = 0;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        npcState = NPCState.Walking;

        yield return character.MoveChar(movementPattern[currentPattern]);
        currentPattern = (currentPattern + 1) % movementPattern.Count;

        npcState = NPCState.Idle;
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (npcState == NPCState.Idle)
        {
            npcState = NPCState.Dialog;
            character.LookTowards(initiator.position);
            yield return DialogManager.Instance.ShowDialog(dialog, npcName,() => 
            {
                idleTimer = 0f;
                npcState = NPCState.Idle; 
            });
        }
    }
}

public enum NPCState { Idle, Walking, Dialog}
