using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class CheckpointManager : MonoBehaviour
{
    [Header("Scene and Checkpoint Management")]
    PlayerController player;
    [SerializeField] CheckpointData cpData;
    [SerializeField] int sceneToLoad;
    [SerializeField] DestinationIdentifier destinationIdentifier;

    [Header("Map and Checkpoint Management")]
    [SerializeField] RectTransform circleSelection;
    public List<CheckpointUI> checkpointUI;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI checkpointText;

    public int availableCP = 4;
    public int travelledScene;

    public int currentSceneSelection;
    public int currentScene;
    public int currentCheckpoint;




    private Checkpoint[] currentSceneCP;
    public static CheckpointManager Instance { get; private set; }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // To solve FindObjectsOfType issue where it is not executed during changing scene 
        SetCheckpointAlpha();
        CheckSceneForMapVisibility();
    }

    void Start()
    {
        Instance = this;
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    public void HandleUpdate(Action onBack)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            currentSceneSelection--;

            currentSceneSelection = Mathf.Clamp(currentSceneSelection, 0, travelledScene);
            //currentSceneSelection = Mathf.Clamp(currentSceneSelection, 0, checkpointUIPosition.Count);
            MapManagement.Instance.UpdateCurrentMap(currentSceneSelection);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            currentSceneSelection++;

            currentSceneSelection = Mathf.Clamp(currentSceneSelection, 0, travelledScene);

            MapManagement.Instance.UpdateCurrentMap(currentSceneSelection);
        }



        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentCheckpoint--;

            //currentCheckpoint = Mathf.Clamp(currentCheckpoint, 0, checkpointUIPosition[currentScene].cpLocUI.Count);
            currentCheckpoint = Mathf.Clamp(currentCheckpoint, 0, availableCP);

        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentCheckpoint++;

            currentCheckpoint = Mathf.Clamp(currentCheckpoint, 0, availableCP);
        }

        SetMapUIText();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            destinationIdentifier = (DestinationIdentifier)currentCheckpoint;
            Debug.Log(destinationIdentifier.ToString());
            
            
            StartCoroutine(OnClickTeleport(currentSceneSelection, destinationIdentifier));
        }

        if (Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();
    }

    // Collect Checkpoint Data and store into cpData
    public void CollectData(int sceneIndex, DestinationIdentifier destinationIdentifier, Vector3 loc, bool isActive)
    {
        cpData.sceneToLoad.Add(sceneIndex);
        cpData.destinationIdentifier.Add(destinationIdentifier);
        cpData.cpLoc.Add(loc);
        cpData.activeStatus.Add(isActive);
    }




    // Teleport for local map
    public IEnumerator OnClickTeleport(int index, DestinationIdentifier destination)
    {
        // Get Current Scene to compare
        Scene getCurrentScene = SceneManager.GetActiveScene();
        currentScene = getCurrentScene.buildIndex;
        Debug.Log(currentScene.ToString());

        //Check if the scene is the same and checkpoint is active or not
        if (currentScene == index)
        {
            // Get the correct Checkpoint Destination
            var destCheckpoint = FindObjectsOfType<Checkpoint>().First(x => x != this && x.DestinationIdentifier == destination);

            if (IsCPActive(destCheckpoint))
            {
                GameController.Instance.PauseGame(true);
                MapManagement.Instance.OnOpenMap();
                yield return StartCoroutine(EffectTransition.Instance.SceneTransition());
                player.Character.SetPositionAndSnapToTile(destCheckpoint.SpawnPoint.position);
                GameController.Instance.PauseGame(false);
            }
        }
        else if (currentScene != index)
        {
            CheckpointStatus[] grabCPStatus = FindObjectsOfType<CheckpointStatus>();

            foreach (var cp in grabCPStatus)
            {
                if (cp.identifier == destination && cp.sceneToLoad == index && cp.isActive)
                    StartCoroutine(SwitchScene(index, destination));
            }
        }
    }

    // Teleport across the scene
    public IEnumerator SwitchScene(int index, DestinationIdentifier identifier)
    {
        sceneToLoad = index;

        GameController.Instance.PauseGame(true);
        MapManagement.Instance.OnOpenMap();

        yield return StartCoroutine(EffectTransition.Instance.SceneTransition());
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destCheckpoint = FindObjectsOfType<Checkpoint>().First(x => x.DestinationIdentifier == identifier && x.IsActive);
        player.Character.SetPositionAndSnapToTile(destCheckpoint.SpawnPoint.position);

        GameController.Instance.PauseGame(false);
    }




    // For Image Alpha Management
    public bool HasActivated(Vector3 locToCheck)
    {
        List<Vector3> collectedLocData = cpData.cpLoc;
        foreach (var data in collectedLocData)
        {
            if (data == locToCheck)
                return true;
        }
        return false;
    }

    // For cp in local scene and check status
    public bool IsCPActive(Checkpoint cp)
    {
        List<Vector3> collectedLocData = cpData.cpLoc;

        foreach (var data in collectedLocData)
        {
            if (cp.SpawnPoint.position == data && cp.IsActive)
                return true;
        }
        return false;
    }

    public void SetCheckpointAlpha()
    {
        List<Vector3> collectedLocData = cpData.cpLoc;
        currentSceneCP = FindObjectsOfType<Checkpoint>();
        foreach (var cp in currentSceneCP)
        {
            foreach (var data in collectedLocData)
            {
                if (cp.gameObject.transform.position == data)
                {
                    cp.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255); //RGBA
                    cp.IsActive = true;
                }
            }
        }
    }

    public void SetMapUIText()
    {
        var totalCpInScene = checkpointUI[currentSceneSelection].cpLocUI.Count-1;
        Debug.Log(totalCpInScene);
        if (totalCpInScene < currentCheckpoint)
            currentCheckpoint = totalCpInScene;

        var selectedCpUI = checkpointUI[currentSceneSelection].cpLocUI[currentCheckpoint].transform;
        circleSelection.SetParent(selectedCpUI);
        circleSelection.localPosition = new Vector3(0,0);

        levelText.text = $"<Level {currentSceneSelection + 1}>";

        destinationIdentifier = (DestinationIdentifier)currentCheckpoint;
        checkpointText.text = $"<Checkpoint {destinationIdentifier}>";

        CheckpointStatus[] cpStatus = FindObjectsOfType<CheckpointStatus>();

        foreach (var status in cpStatus)
        {
            if (status.sceneToLoad == currentSceneSelection && status.identifier == destinationIdentifier)
            {
                Debug.Log("Found!");
                if (status.isActive)
                    circleSelection.GetComponent<Image>().color = Color.green;
                else
                    circleSelection.GetComponent<Image>().color = Color.red;
            }
        }
    }

    public void CheckSceneForMapVisibility()
    {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex > travelledScene)
            travelledScene = currentSceneIndex;
    }
}

[System.Serializable]
public class CheckpointData
{
    public List<int> sceneToLoad;
    public List<DestinationIdentifier> destinationIdentifier;
    public List<Vector3> cpLoc;
    public List<bool> activeStatus;
}

[System.Serializable]
public class CheckpointUI
{
    public List<GameObject> cpLocUI;
}
