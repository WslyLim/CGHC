using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
    [Header("References")]
    [SerializeField] MonsterParty enemyParty;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Dialog dialogBeforeBattle; // for Battle
    [SerializeField] Dialog dialogAfterBattle; // for Conversation
    [SerializeField] string trainersName;
    Character character;

    public bool BattleLost { get; set; }

    public string TrainerName => trainersName;
    public GameObject Fov => fov;
    

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>(); 
        
        SetFovRotation(character.Animator.DefaultDirection);
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Left)
            angle = 270f;
        else if (dir == FacingDirection.Up)
            angle = 180f;

        fov.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public IEnumerator TriggerBattle(PlayerController player)
    {
        // Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);


        // Walk towards to player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;

        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.MoveChar(moveVec);

        // Show Dialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialogBeforeBattle, trainersName,() =>
        {
            GameController.Instance.ForceStartBattle(this);
        }));
    }

    public IEnumerator Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);
        if (!BattleLost)
        {
            yield return DialogManager.Instance.ShowDialog(dialogBeforeBattle, trainersName ,() =>
            {
                GameController.Instance.ForceStartBattle(this);
            });
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle, trainersName);
        }
        
    }

    public void OnBattleLoss()
    {
        BattleLost = true;
        fov.SetActive(false);
        StoreTrainersData.Instance.SetTrainerBattleLost(trainersName, BattleLost);

    }
}
