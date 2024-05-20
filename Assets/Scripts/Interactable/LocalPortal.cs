using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] DestinationIdentifier destinationIdentifier;
    [SerializeField] Transform spawnPoint;
    PlayerController player;
    public Transform SpawnPoint => spawnPoint;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport()
    {
        GameController.Instance.PauseGame(true);

        yield return StartCoroutine(EffectTransition.Instance.SceneTransition());

        var destPortal = FindObjectsOfType<LocalPortal>().First(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        GameController.Instance.PauseGame(false);
    }
}
