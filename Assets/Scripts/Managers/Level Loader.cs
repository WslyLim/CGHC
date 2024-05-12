using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	public Animator transitioner;
	
	public float transitionTime = 1f;
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
		{
			LoadLevel();
		}
    }
	
	public void LoadLevel()
	{
		StartCoroutine(LevelLoad(SceneManager.GetActiveScene().buildIndex + 1));
	}
	
	IEnumerator LevelLoad(int levelIndex)
	{
		//Play animation
		transitioner.SetTrigger("Start");
		
		//Code Delay
		yield return new WaitForSeconds(transitionTime);
		
		//Load Scene
		SceneManager.LoadScene(levelIndex);
	}
}
