using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    public CutsceneManager cutsceneManager;
    public PlayableDirector director;
    public bool isPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        director = GetComponent<PlayableDirector>();
        cutsceneManager = FindObjectOfType<CutsceneManager>();
    }

    public PlayableDirector GetDirector()
    { return director; }

    public void PlayAnimation()
    {
        director.Play();
    }

    public void OnTriggered(object sender, EventArgs e)
    {
        if (!isPlayed)
        {
            PlayAnimation();
            cutsceneManager.OnTrigger -= OnTriggered;
            isPlayed = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Triggered");
            cutsceneManager.OnTrigger += OnTriggered;
        }
    }
}
