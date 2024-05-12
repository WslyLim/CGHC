using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointStatus : MonoBehaviour
{
    public int sceneToLoad;
    public DestinationIdentifier identifier;
    public bool isActive;

    public void SetAsActive(int sceneToLoad, DestinationIdentifier identifier, bool isActive)
    {
        if (CheckForCP(sceneToLoad, identifier, isActive))
            this.isActive = isActive;
    }

    public bool CheckForCP(int sceneToLoad,DestinationIdentifier identifier, bool isActive)
    {
        if (this.sceneToLoad == sceneToLoad && this.identifier == identifier && isActive == true) 
        {
            return true;
        }
        else
            return false;
    }
}
