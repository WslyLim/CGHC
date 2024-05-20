using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class LootsManager : MonoBehaviour
{
    [SerializeField] PickUp[] pickUps;
    [SerializeField] List<Vector3> pickUpsPos;
    [SerializeField] List<bool> status;

    public static LootsManager Instance;


    // Collect all pickups in the current scene first
    // Store their position, make sure no clones or store existing ones
    // When storing, make sure to check the boolean, dont overwrite.
    // If boolean is true, then don't store into dictionary
    // When player collects, mark it with boolean as true.


    private void Awake()
    {
        Instance = this;
        pickUps = FindObjectsOfType<PickUp>();
        StorePickUpsPosition();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        pickUps = FindObjectsOfType<PickUp>();

        if (pickUps != null)
        {
            StorePickUpsPosition();
            CheckIfUsed();
        }


    }

    public void StorePickUpsPosition()
    {
        foreach (PickUp pickUp in pickUps) 
        {
            if (!pickUpsPos.Contains(pickUp.transform.position))
            {
                pickUpsPos.Add(pickUp.transform.position);
                status.Add(pickUp.Used);
            }
        }
    }

    public void CheckIfUsed()
    {
        foreach (PickUp pickUp in pickUps)
        {
            int index = pickUpsPos.IndexOf(pickUp.transform.position);
            if (index != -1 && status[index])
            {
                pickUp.Used = true;
                pickUp.gameObject.SetActive(false); // Hide the collected item
            }
        }
    }

    public void SetStatusToTrue(PickUp pickedItem)
    {
        for (int i = 0;  i < pickUpsPos.Count; i++) 
        {
            if (pickUpsPos[i] == pickedItem.transform.position)
                status[i] = true;
        }
    }
}
