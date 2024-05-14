using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using static UnityEngine.GraphicsBuffer;

public enum BattleState { Start, ActionSelection, MoveSelection, SelectingAlly, SelectingTarget, PerformMove, Busy, BattleOver}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] List<BattleUnit> allBattleUnits;
    public List<BattleUnit> playerUnits;
	public List<BattleUnit> enemyUnits;
    public BattleUnit CurrentUnitTurn;

	public TextMeshProUGUI dialogueText;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

	public BattleState state;
    int currentAction;
    int currentMove;
    int currentTurn = 0;
    int currentTarget;

    MonsterParty playerParty;
    MonsterParty enemyParty;
    MapArea wildMonster;
    int monsterAmount;

    bool isForcedBattle; //isTrainerBattle
    PlayerController player;
    TrainerController trainer;


    public event Action<bool> OnBattleOver;

    // Random Encounter Battle
    public void StartBattle(MonsterParty playerParty, MapArea wildMonster)
    {
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;

        state = BattleState.Start;

        StartCoroutine(SetupBattle());
        
	}

    // Force Battle from Trainer/Story
    public void ForceBattle(MonsterParty playerParty, MonsterParty enemyParty)
    {
        this.playerParty = playerParty;
        this.enemyParty = enemyParty;

        isForcedBattle = true;

        player = playerParty.GetComponent<PlayerController>();
        trainer = enemyParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle() // Setup the battle system
    {
        if (isForcedBattle)
        {
            // Setup Player Monsters with HUD
            for (int i = 0; i < playerParty.Monsters.Count; i++)
            {
                playerUnits[i].Setup(playerParty.Monsters[i]);
            }

            // Setup Enemy Monsters with HUD
            for (int i = 0; i < enemyParty.Monsters.Count; i++)
            {
                enemyUnits[i].Setup(enemyParty.Monsters[i]);
            }
        }
        else
        {
            // Random number of monsters for random encounter
            
            monsterAmount = UnityEngine.Random.Range(0, wildMonster.MonsterAmount);
            if (monsterAmount == 0)
                monsterAmount = 1;

            // Setup Player Monsters with HUD
            for (int i = 0; i < playerParty.Monsters.Count; i++)
            {
                playerUnits[i].Setup(playerParty.Monsters[i]);
            }

            // Setup Enemy Monsters with HUD
            for (int i = 0; i < monsterAmount; i++)
            {
                enemyUnits[i].Setup(wildMonster.GetRandomWildMonsters());
            }
        }

        partyScreen.Init();

        yield return dialogBox.TypeDialog("Battle Started");

        GatherAllBattleUnit();
        SortBySpeed();
        ExecuteTurn();
    }

    #region Battle Unit's Turn Management
    public void GatherAllBattleUnit() // Gather all monsters in the battle field and store into the list
    {
        for (int i = 0; i < playerParty.Monsters.Count; i++)
            if (playerUnits[i].gameObject.activeSelf)
                allBattleUnits.Add(playerUnits[i]);

        for (int i = 0; i <= monsterAmount; i++)
            if (enemyUnits[i].gameObject.activeSelf)
                allBattleUnits.Add(enemyUnits[i]);
    }

    public void TurnChecking() // Check the monster's turn based by speed attribute, either enemy or player will attack first
    {
        //foreach (BattleUnit unit in allBattleUnits)
        //{
        //    if (unit.HasFallen)
        //    {
        //        allBattleUnits.Remove(unit);
        //    }
        //}

        if (currentTurn >= allBattleUnits.Count - 1)
        {
            currentTurn = 0;
            foreach (BattleUnit unit in allBattleUnits)
            {
                unit.HasTakenTurn = false;
            }
        }
        else if (currentTurn < allBattleUnits.Count)
        {
            currentTurn++;
        }
    }

    public void ExecuteTurn()
    {
        CurrentUnitTurn = GetNextTurnUnit();

        if (CurrentUnitTurn.isPlayerUnit)
            ActionSelection();
        else if (!CurrentUnitTurn.isPlayerUnit)
            StartCoroutine(EnemyMove(CurrentUnitTurn));
    }

    public BattleUnit GetNextTurnUnit()
    {
        foreach (BattleUnit unit in allBattleUnits)
        {
            if (!unit.HasTakenTurn)
            {
                unit.HasTakenTurn = true;
                return unit;
            }
        }
        return null;
    }

    public void SortBySpeed() // Sort the monster's turn by speed, the highest speed will be the first to perform a move
    {
        allBattleUnits.Sort((unitA, unitB) =>
        {
            // Get the speed of MonsterBase associated with BattleUnit A
            int speedA = unitA.Monster.Speed;
            // Get the speed of MonsterBase associated with BattleUnit B
            int speedB = unitB.Monster.Speed;

            // Compare the speeds in descending order (higher speed first)
            return speedB.CompareTo(speedA);
        });
    }

    #endregion

   

    #region Player's Turn Management
    private void ActionSelection() // Player's turn to select action (Attack / Item / Run)
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    private void MoveSelection(BattleUnit playerUnitMoves) // Player's turn to select their monster's move 
    {
        state = BattleState.MoveSelection;
        dialogBox.SetMoveNames(playerUnitMoves.Monster.Moves); // Need to be updated later

        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    #endregion

    #region Turn Management
    private IEnumerator PlayerMove(BattleUnit playerUnit, BattleUnit target) // To deal damage to enemy and update enemy hp bar
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Monster.Moves[currentMove]; // Need to be updated later

        yield return RunMove(playerUnit, target, move);
    }
    private IEnumerator EnemyMove(BattleUnit enemy) // Enemy will Perform attack to player's monster. At the end, it will check the turn either the player gets to attack first or enemy.
    {
        state = BattleState.PerformMove;

        var move = enemy.Monster.GetRandomMove();

        yield return RunMove(enemy, playerUnits[0], move); // Need to be updated later
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        state = BattleState.Busy;

        yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.MonsterName} (Team) used {move.MoveBase.MoveName}");

        if (move.MoveBase.MoveCategory == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit.Monster, targetUnit.Monster);
        }
        else if (move.MoveBase.MoveCategory == MoveCategory.Heal)
        {
            var heal = move.MoveBase;
            if (move.MoveBase.Target == MoveTarget.Self) 
            {
                int healedHP = (int)(sourceUnit.Monster.MaxHealth * heal.HealingPercentage / 100);

                sourceUnit.Monster.HP += healedHP;

                if (sourceUnit.Monster.HP >= sourceUnit.Monster.MaxHealth)
                    sourceUnit.Monster.HP = sourceUnit.Monster.MaxHealth;

                yield return sourceUnit.HUD.UpdateHP();
            }
            else if (move.MoveBase.Target == MoveTarget.Ally)
            {
                int healedHP = (int)(targetUnit.Monster.MaxHealth * heal.HealingPercentage / 100);
                targetUnit.Monster.HP += healedHP;

                if (targetUnit.Monster.HP >= targetUnit.Monster.MaxHealth)
                    targetUnit.Monster.HP = targetUnit.Monster.MaxHealth;

                yield return targetUnit.HUD.UpdateHP();
            }
        }
        else
        {
            var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster); // Need to be updated later
            yield return targetUnit.HUD.UpdateHP(); // Need to be updated later
            yield return ShowDamageDetails(damageDetails);
        }


        if (targetUnit.Monster.HP <= 0)
        {
            yield return HandleFaintedMonster(targetUnit);

            bool win = LoseOrWin();
            // If there is still an enemy alive in the battlefield, then the fight continues
            if (win)
            {
                OnBattleOver(win);
                CleanUp();
            }
            else
            {
                SortBySpeed();
                TurnChecking();
                ExecuteTurn();
            }
        }
        else
        {
            SortBySpeed();
            TurnChecking();
            ExecuteTurn();
        }  
    }
    #endregion


    public void HandleUpdate() // Check Battle State and Handle All Kind of Selection (Action / Move / Enemy Select / Team Select)
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection) 
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.SelectingTarget)
        {
            if (CurrentUnitTurn.Monster.Moves[currentMove].MoveBase.Target == MoveTarget.Ally) 
                HandlePartySelection();
            else if (CurrentUnitTurn.Monster.Moves[currentMove].MoveBase.Target == MoveTarget.Enemy)
                HandleEnemySelection();
        }
    }


    #region Player Selector Management
    private void HandleActionSelection() // Select Action and Highlight Colour
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) 
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            if (currentAction == 0) 
            {
                // Attack
                MoveSelection(CurrentUnitTurn);
            }
            else if (currentAction == 1)
            {
                // Item
            }
        }
    }


    private void HandleMoveSelection() // Select Move and Highlight Colour
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, CurrentUnitTurn.Monster.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, CurrentUnitTurn.Monster.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (CurrentUnitTurn.Monster.Moves[currentMove].MoveBase.Target == MoveTarget.Self)
            {
                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(PlayerMove(CurrentUnitTurn, CurrentUnitTurn));
            }
            else
                state = BattleState.SelectingTarget;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }


    public void HandleEnemySelection() // Select Enemy
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentTarget < dialogBox.enemyTexts.Count - 1)
                ++currentTarget;
        }

        if (Input.GetKeyDown (KeyCode.LeftArrow))
        {
            if (currentTarget > 0)
                --currentTarget;
        }

        dialogBox.UpdateEnemyTargetSelection(currentTarget);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove(CurrentUnitTurn, enemyUnits[currentTarget]));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            state = BattleState.MoveSelection;
            dialogBox.EnableMoveSelector(true);
            dialogBox.EnableDialogText(false);
            dialogBox.enemyBackgroundImages[currentTarget].color = Color.white;
            dialogBox.enemyTexts[currentTarget].color = Color.black;
        }
    }


    public void HandlePartySelection() // Select Ally
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentTarget < dialogBox.partyTexts.Count - 1)
                ++currentTarget;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentTarget > 0)
                --currentTarget;
        }

        dialogBox.UpdateAllyTargetSelection(currentTarget);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove(CurrentUnitTurn, playerUnits[currentTarget])); // Need to be Updated
            currentTarget = 0;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            state = BattleState.MoveSelection;
            dialogBox.EnableMoveSelector(true);
            dialogBox.EnableDialogText(false);
            dialogBox.enemyBackgroundImages[currentTarget].color = Color.white;
            dialogBox.enemyTexts[currentTarget].color = Color.black;
        }
    }
    #endregion

    IEnumerator ShowStatusChanges(Monster monster) 
    {
        while (monster.StatusChanges.Count > 0) 
        {
            var message = monster.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails dmgDetail)
    {
        if (dmgDetail.Critical > 1f)
            yield return dialogBox.TypeDialog("A Critical Hit!");

        if (dmgDetail.TypeEffective > 1f)
            yield return dialogBox.TypeDialog("It's Super Effective!");
        else if (dmgDetail.TypeEffective < 1f)
            yield return dialogBox.TypeDialog("It's Not Very Effective!");
    }

    IEnumerator RunMoveEffects(Move move, Monster source,  Monster target)
    {
        var effects = move.MoveBase.Effects;
        if (effects.Boosts != null)
        {
            if (move.MoveBase.Target == MoveTarget.Self)
                source.ApplyBoosts(move.MoveBase.Effects.Boosts);
            else
                target.ApplyBoosts(move.MoveBase.Effects.Boosts);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    public IEnumerator HandleFaintedMonster(BattleUnit faintedUnit)
    {
        string allyOrFoe;
        if (faintedUnit.isPlayerUnit)
            allyOrFoe = "(Team)";
        else
            allyOrFoe = "(Enemy)";

        yield return dialogBox.TypeDialog($"{faintedUnit.Monster.Base.MonsterName} {allyOrFoe} has Fallen"); // Need to be updated later

        if (!faintedUnit.isPlayerUnit)
        {
            // Exp Gain
            int expYield = faintedUnit.Monster.Base.ExpYield;
            int enemyLevel = faintedUnit.Monster.Level;
            float trainerBonus = (isForcedBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            CurrentUnitTurn.Monster.Exp += expGain;
            yield return dialogBox.TypeDialog($"{CurrentUnitTurn.Monster.Base.name} gained {expGain} EXP");
            yield return StartCoroutine(CurrentUnitTurn.HUD.SmoothTransition(false));

            //Check if Level up
            while (CurrentUnitTurn.Monster.CheckForLevelUp())
            {
                CurrentUnitTurn.HUD.SetLevel();
                yield return dialogBox.TypeDialog($"{CurrentUnitTurn.Monster.Base.name} Leveled up to {CurrentUnitTurn.Monster.Level}");
                yield return StartCoroutine(CurrentUnitTurn.HUD.SmoothTransition(true));

            }
        }

        allBattleUnits.Remove(faintedUnit);

    }

    void OpenPartyScreen()
    {
        
        partyScreen.gameObject.SetActive(true);
    }

    private bool LoseOrWin()
    {
        // If no more enemy unit alive, end battle
        if (!allBattleUnits.Exists(unit => playerUnits.Contains(unit)))
            return true;
        else if (!allBattleUnits.Exists(unit => enemyUnits.Contains(unit)))
            return true;
        else
            return false;
    }

    private void CleanUp()
    {
        // Reset all battle units' turn
        foreach (var unit in allBattleUnits) 
        {
            unit.CleanUp();
        }

        // Set the current target to empty
        CurrentUnitTurn = null;

        // Reset the turn
        currentTurn = 0;
        
        // Reset the highlighted colour
        List<Image> colorReset = dialogBox.enemyBackgroundImages;

        foreach (var image in colorReset) 
        {
            image.color = Color.white;
        }

        playerParty.Monsters.ForEach(monster => monster.OnBattleOver());

        // Clear up the list of battle units in the previous battle
        allBattleUnits.Clear();
        isForcedBattle = false;
    }


}