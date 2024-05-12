using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public event EventHandler OnTrigger;

    private void Update()
    {
        OnTrigger?.Invoke(this, EventArgs.Empty);
    }
}
