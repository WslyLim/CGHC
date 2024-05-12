using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

    private Vector2 input;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;


            // Check for holding down the Shift key to increase movement speed
            if (Input.GetKey(KeyCode.LeftShift))
                character.moveSpeed = 20; // Set the movement speed to a higher value
            else
                character.moveSpeed = 10; // Set the movement speed back to the default value


            if (input != Vector2.zero)
                StartCoroutine(character.MoveChar(input, OnMoveOver));
        }

        character.HandleUpdate();


        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine(Interact());
    }
    
    IEnumerator Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var targetPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(targetPos, 0.3f, GameLayers.i.InteractableLayer | GameLayers.i.waterLayer);
        if (collider != null) 
        {
            yield return collider.gameObject.GetComponent<Interactable>()?.Interact(gameObject.transform);
        }
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffSetY), 0.2f, GameLayers.i.TriggerableLayer);
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null) 
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public Character Character => character;
}
