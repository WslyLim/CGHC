using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Checkpoint : MonoBehaviour, IPlayerTriggerable
{

    SpriteRenderer sprite;

    [SerializeField] int sceneToLoad;
    [SerializeField] DestinationIdentifier destinationIdentifier;
    [SerializeField] Transform spawnPoint;
    public int cpOnUIIndex;
    public int cpOnSceneIndex;
    [SerializeField] CheckpointStatus[] status;

    public bool IsActive { get; set; } = false;

    public Transform SpawnPoint => spawnPoint;
    public DestinationIdentifier DestinationIdentifier => destinationIdentifier;

    
    public void OnPlayerTriggered(PlayerController player)
    {
        if (!CheckpointManager.Instance.HasActivated(transform.position)) 
        {
            IsActive = true;
            sprite.color = Color.white;
            Debug.Log("Triggered Checkpoint");
            CheckpointManager.Instance.CollectData(sceneToLoad, destinationIdentifier,transform.position, IsActive);

            MapManagement.Instance.OnOpenMap();

            status = FindObjectsOfType<CheckpointStatus>();
            foreach (var item in status)
            {
                item.SetAsActive(sceneToLoad, destinationIdentifier, IsActive);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }


}

