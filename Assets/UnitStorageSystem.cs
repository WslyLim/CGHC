using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStorageSystem : MonoBehaviour
{
    public List<GameObject> Storage;
    public TeamSlot[] slots;
    public List<HealthSystem> slotsList;
    public RectTransform storageSlotUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SendToStorage(GameObject monster)
    {
        foreach (var slot in slots) 
        {
            if (!slot.isOccupied && !Storage.Contains(monster)) 
            {
                Debug.Log("Sending Monster to Storage");
                Storage.Add(monster);
                monster.transform.parent = slot.transform;
            }
            else
            {
                Debug.Log("Monster is already in storage.");
            }
        }
        

        //bool found = false; // Flag to indicate if the monster is found in the list

        //foreach (var item in Storage)
        //{
        //    if (item == monster)
        //    {
        //        Debug.Log("Same Monster");
        //        found = true;
        //        break; // Exit the loop early since we found the monster
        //    }
        //}

        //if (!found)
        //{
        //    Debug.Log("Sending Monster to Storage");
        //    Storage.Add(monster);
        //    monster.transform.position = transform.position;
        //}
        //else if (found)
        //{
        //    monster.transform.position = transform.position;
        //}
    }
}
