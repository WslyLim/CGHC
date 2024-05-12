using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SurfableWater : MonoBehaviour, Interactable, IPlayerTriggerable
{
    public bool TriggerRepeatedly => true;
    public IEnumerator Interact(Transform player)
    {
        var animator = player.GetComponent<CharacterAnimation>();

        if (animator.IsSurfing)
            yield break;

        var monsterWithTP = player.GetComponent<MonsterParty>().monsters.FirstOrDefault(x => x.Base.MonsterName == "Vermillion Bird");
        if (monsterWithTP != null)
        {
            Debug.Log("Can surf!");

            var dir = new Vector3(animator.MoveX, animator.MoveY);
            var targetPos = player.position + dir;

            yield return player.position = targetPos;
            animator.IsSurfing = true;
        }
        else
        {
            Debug.Log("Typing");
            yield return DialogManager.Instance.ShowDialogText("It is just a water");
        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (Random.Range(1, 101) <= 10)
        {
            player.Character.Animator.IsMoving = false;
            StartCoroutine(GameController.Instance.StartBattle(gameObject.GetComponent<MapArea>()));
        }
    }
}
