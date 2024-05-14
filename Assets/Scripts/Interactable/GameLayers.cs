using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    public LayerMask waterLayer;
    public LayerMask interactableLayer;
    public LayerMask solidObjectLayer;
    public LayerMask encounterLayer;
    public LayerMask playerLayer;
    public LayerMask fovLayer;
    public LayerMask portalLayer;
    public LayerMask checkpointLayer;

    public static GameLayers i { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        i = this;
    }

    public LayerMask WaterLayer => waterLayer;

    public LayerMask InteractableLayer => interactableLayer;

    public LayerMask SolidObjectLayer => solidObjectLayer;

    public LayerMask EncounterLayer => encounterLayer;

    public LayerMask PlayerLayer => playerLayer;

    public LayerMask FovLayer => fovLayer;

    public LayerMask PortalLayer => portalLayer;

    public LayerMask CheckPointLayer => checkpointLayer;

    public LayerMask TriggerableLayer => encounterLayer | fovLayer | portalLayer | checkpointLayer | waterLayer;
}
