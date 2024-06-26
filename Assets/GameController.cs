using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {FreeRoam, Bag, Map, Party, Battle, Dialog, Cutscene, Paused}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    TrainerController trainer;

    [SerializeField] GameState state;
    GameState stateBeforePaused;

    [SerializeField] List<GameObject> menu;

    public static GameController Instance {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        battleSystem.OnBattleOver += EndBattle;
        MapManagement.Instance.OpenMap += OpenMap;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforePaused = GameState.FreeRoam;
            state = GameState.Paused;
        }
        else
        {
            state = stateBeforePaused;
        }
    }

    public IEnumerator StartBattle(MapArea mapArea)
    {
        state = GameState.Battle;
        yield return StartCoroutine(EffectTransition.Instance.BattleTransition());
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<MonsterParty>();
        MapArea wildMonster = mapArea;

        battleSystem.StartBattle(playerParty, wildMonster);
    }

    public IEnumerator ForceStartBattle(TrainerController opponent)
    {
        state = GameState.Battle;
        yield return EffectTransition.Instance.BattleTransition();
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        trainer = opponent;
        var playerParty = playerController.GetComponent<MonsterParty>();
        var opponentParty = opponent.GetComponent<MonsterParty>();

        battleSystem.ForceBattle(playerParty, opponentParty);
    }

    void EndBattle(bool won)
    {

        if (trainer != null && won) 
        {
            StartCoroutine(trainer.OnBattleLoss());
            trainer = null;
        }

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    public void OpenMap()
    {
        if (state == GameState.Map)
            state = GameState.FreeRoam;
        else
            state = GameState.Map;
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerBattle(playerController));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
            Time.timeScale = 2f;
        else if (Input.GetKeyUp(KeyCode.RightShift))
            Time.timeScale = 1f;

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(StartBattle(FindAnyObjectByType<MapArea>()));
            state = GameState.Battle;
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            int index = 0; // referring to specific element in the list
            menu[index].SetActive(true);
            state = GameState.Bag;
        }
        else if (Input.GetKeyDown(KeyCode.M)) 
        {
            int index = 1; // referring to specific element in the list
            menu[index].SetActive(true);
            state = GameState.Map;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            int index = 2; // referring to specific element in the list
            menu[index].SetActive(true);
            partyScreen.Init();
            state = GameState.Party;
        }


        //========================================================================//


        if (state == GameState.FreeRoam) 
        {
            playerController.HandleUpdate();
            battleSystem.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
        }
        else if (state == GameState.Battle) 
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Bag)
        {
            int index = 0; // referring to specific element in the list
            Action onClose = () =>
            {
                menu[index].gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            InventoryUI.Instance.HandleUpdate(onClose);
        }
        else if (state == GameState.Map) 
        {
            int index = 1; // referring to specific element in the list
            Action onClose = () =>
            {
                menu[index].gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            CheckpointManager.Instance.HandleUpdate(onClose);
        }
        else if (state == GameState.Party) 
        {
            int index = 2; // referring to specific element in the list

            Action onSelect = () =>
            {
                Debug.Log("Selected");
            };

            Action onClose = () =>
            {
                menu[index].gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            partyScreen.HandleUpdate(onSelect, onClose);
        }

    }


}
