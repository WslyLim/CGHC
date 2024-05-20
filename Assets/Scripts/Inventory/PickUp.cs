using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, Interactable
{
    [SerializeField] ItemBase item;


    public bool Used { get; set; } = false;
    public IEnumerator Interact(Transform initiator)
    {
        if (!Used) 
        {
            initiator.GetComponent<Inventory>().AddItem(item);

            Used = true;

            LootsManager.Instance.SetStatusToTrue(this);

            yield return DialogManager.Instance.ShowDialogText($"You Found {item.ItemName}!");

            gameObject.SetActive(false);
        }
    }
}
