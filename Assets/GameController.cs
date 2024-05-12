using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {FreeRoam, Map, Battle, Dialog, Cutscene, Paused}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    TrainerController trainer;

    [SerializeField] GameState state;
    GameState stateBeforePaused;

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
        yield return StartCoroutine(EffectTransition.Instance.PlayTransition());
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<MonsterParty>();
        MapArea wildMonster = mapArea;

        battleSystem.StartBattle(playerParty, wildMonster);
    }

    public void ForceStartBattle(TrainerController opponent)
    {
        state = GameState.Battle;
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
            trainer.OnBattleLoss();
            trainer = null;
        }

        if (won)
        {
            state = GameState.FreeRoam;
            battleSystem.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
        }
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(StartBattle(FindAnyObjectByType<MapArea>()));
            state = GameState.Battle;
        }



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
        else if (state == GameState.Map) 
        {
            CheckpointManager.Instance.HandleUpdate();
        }
    }


}
