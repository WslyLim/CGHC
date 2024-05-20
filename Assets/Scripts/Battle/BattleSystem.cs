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

public enum BattleState { Start, ActionSelection, MoveSelection, Bag, SelectingAlly, SelectingTarget, PerformMove, Busy, BattleOver}

public enum BattleOutcome { None, Ongoing, PlayerWon, PlayerLost}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] List<BattleUnit> allBattleUnits;
    public List<BattleUnit> playerUnits;
	public List<BattleUnit> enemyUnits;
    public BattleUnit CurrentUnitTurn;

	public TextMeshProUGUI dialogueText;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

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
    public List<BattleUnit> AllBattleUnits => allBattleUnits;



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
        TurnChecking();
    }

    #region Battle Unit's Turn Management
    public void GatherAllBattleUnit() // Gather all monsters in the battle field and store into the list
    {
        for (int i = 0; i < playerParty.Monsters.Count; i++)
            if (playerUnits[i].gameObject.activeSelf)
                allBattleUnits.Add(playerUnits[i]);

        for (int i = 0; i < enemyUnits.Count; i++)
            if (enemyUnits[i].gameObject.activeSelf)
                allBattleUnits.Add(enemyUnits[i]);
    }

    public void TurnChecking() // Check the monster's turn based on the speed attribute
    {
        SortBySpeed();
        ExecuteTurn();
    }

    public void ExecuteTurn()
    {
        CurrentUnitTurn = GetNextTurnUnit();

        if (CurrentUnitTurn != null)
        {
            if (CurrentUnitTurn.isPlayerUnit)
            {
                ActionSelection();
            }
            else
            {
                StartCoroutine(EnemyMove(CurrentUnitTurn));
            }
        }
    }

    private BattleUnit GetNextTurnUnit()
    {
        for (int i = currentTurn; i < allBattleUnits.Count; i++)
        {
            BattleUnit unit = allBattleUnits[i];
            currentTurn++;

            if (unit.Monster.HP > 0)
            {
                return unit;
            }
        }

        // Reset the turn and check again
        currentTurn = 0;
        ResetTurns();

        // Re-check from the beginning after resetting
        for (int i = currentTurn; i < allBattleUnits.Count; i++)
        {
            BattleUnit unit = allBattleUnits[i];
            currentTurn++;

            if (unit.Monster.HP > 0)
            {
                return unit;
            }
        }

        Debug.Log("Executed");
        return null;
    }

    private void ResetTurns()
    {
        foreach (BattleUnit unit in allBattleUnits)
        {
            unit.HasTakenTurn = false;
        }
    }

    public void SortBySpeed() // Sort the monster's turn by speed, the highest speed will be the first to perform a move
    {
        allBattleUnits.Sort((unit1, unit2) =>
        {
            int speedComparison = unit2.Monster.Speed.CompareTo(unit1.Monster.Speed);
            if (speedComparison == 0)
            {
                // If speeds are equal, compare by level
                return unit2.Monster.Level.CompareTo(unit1.Monster.Level);
            }
            return speedComparison;
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

    void OpenInventory()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
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

        var target = GetRandomTarget();

        yield return RunMove(enemy, target, move); // Need to be updated later
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        state = BattleState.Busy;

        string allyOrFoe;
        if (sourceUnit.isPlayerUnit)
            allyOrFoe = "(Team)";
        else
            allyOrFoe = "(Enemy)";

        yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.MonsterName} {allyOrFoe} used {move.MoveBase.MoveName}");

        move.SP = Mathf.Clamp(move.SP--, 0, move.MoveBase.SP);

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
                sourceUnit.Monster.IncreaseHP(healedHP);
            }
            else if (move.MoveBase.Target == MoveTarget.Ally)
            {
                int healedHP = (int)(targetUnit.Monster.MaxHealth * heal.HealingPercentage / 100);
                targetUnit.Monster.IncreaseHP(healedHP);
            }
        }
        else
        {
            yield return HandleSimpleAnimation(sourceUnit);
            yield return Shake(targetUnit.gameObject, 5f, 0.3f);
            var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
            yield return ShowDamageDetails(damageDetails);
        }


        if (targetUnit.Monster.HP <= 0)
        {
            yield return HandleFaintedMonster(targetUnit);

            BattleOutcome outcome = LoseOrWin();

            // If there is still an enemy alive in the battlefield, then the fight continues
            if (outcome == BattleOutcome.PlayerWon)
            {
                OnBattleOver(true);
                CleanUp();
            }
            else if (outcome == BattleOutcome.PlayerLost)
            {
                OnBattleOver(false);
                CleanUp();
            }
            //else if (outcome == BattleOutcome.Ongoing)
            //{
            //    TurnChecking();
            //}
        }
        
        TurnChecking();
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
        else if (state == BattleState.Bag)
        {
            Action onBack = () => 
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };

            Action onItemUsed = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                TurnChecking();
            };

            inventoryUI.HandleUpdate(onBack, onItemUsed);
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

        currentAction = Mathf.Clamp(currentAction, 0, 2);

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
                OpenInventory();
            }
            else if (currentAction == 2)
            {
                OnBattleOver(false);
                CleanUp();
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
            var move = CurrentUnitTurn.Monster.Moves[currentMove];
            if (move.SP == 0 && !move.InfiniteSP) return;

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
            
            dialogBox.enemyBackgroundImages[currentTarget].color = Color.white;
            dialogBox.enemyTexts[currentTarget].color = Color.black;
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

            StartCoroutine(PlayerMove(CurrentUnitTurn, playerUnits[currentTarget]));

            dialogBox.partyBackgroundImages[currentTarget].color = Color.white;
            dialogBox.partyTexts[currentTarget].color = Color.black;

            currentTarget = 0;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            state = BattleState.MoveSelection;
            
            dialogBox.EnableMoveSelector(true);
            dialogBox.EnableDialogText(false);
            
            dialogBox.partyBackgroundImages[currentTarget].color = Color.white;
            dialogBox.partyTexts[currentTarget].color = Color.black;
        }
    }
    #endregion

    BattleUnit GetRandomTarget()
    {
        var unitList = playerUnits.Where(unit => unit.Monster != null && unit.Monster.HP > 0 && unit.gameObject.activeSelf).ToList();
        BattleUnit unit = unitList[UnityEngine.Random.Range(0, unitList.Count)];
        return unit;
    }

    public IEnumerator Shake(GameObject obj, float magnitude, float duration)
    {
        Vector3 originalPos = obj.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            obj.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        obj.transform.localPosition = originalPos;
    }

    IEnumerator HandleSimpleAnimation(BattleUnit unit, float duration = 0.1f)
    {
        RectTransform unitPos = unit.GetComponent<RectTransform>();
        bool thisUnit = unit.isPlayerUnit;
        float startPivotY = thisUnit ? 0.5f : 0.5f;
        float endPivotY = thisUnit ? 0 : 1;

        // Animate pivot in the first direction
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            unitPos.pivot = new Vector2(unitPos.pivot.x, Mathf.Lerp(startPivotY, endPivotY, t / duration));
            yield return null;
        }
        unitPos.pivot = new Vector2(unitPos.pivot.x, endPivotY);

        // Animate pivot back to the original position
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            unitPos.pivot = new Vector2(unitPos.pivot.x, Mathf.Lerp(endPivotY, startPivotY, t / duration));
            yield return null;
        }
        unitPos.pivot = new Vector2(unitPos.pivot.x, startPivotY);
    }

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
        {
            yield return Shake(mainCamera.gameObject, .3f, 0.15f);
            yield return dialogBox.TypeDialog("A Critical Hit!");
        }

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

        //allBattleUnits.Remove(faintedUnit);
        
    }

    private BattleOutcome LoseOrWin()
    {
        // Check if all active player units are defeated
        bool allActivePlayersDefeated = playerUnits
            .Where(unit => unit.gameObject.activeSelf) // Filter active player units
            .All(unit => unit.Monster.HP <= 0);

        // Check if all active enemy units are defeated
        bool allActiveEnemiesDefeated = enemyUnits
            .Where(unit => unit.gameObject.activeSelf) // Filter active enemy units
            .All(unit => unit.Monster.HP <= 0);

        if (allActiveEnemiesDefeated)
        {
            // All active enemies are defeated, player wins
            return BattleOutcome.PlayerWon;
        }
        else if (allActivePlayersDefeated)
        {
            // All active player units are defeated, player loses
            return BattleOutcome.PlayerLost;
        }
        else if (!allActiveEnemiesDefeated && !allActivePlayersDefeated)
        {
            // The battle continues
            return BattleOutcome.Ongoing;
        }
        else
            return BattleOutcome.None;
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
        currentTarget = 0;
        
        // Reset the highlighted colour
        List<Image> colorToReset = dialogBox.enemyBackgroundImages;

        foreach (var image in colorToReset) 
        {
            image.color = Color.white;
        }

        playerParty.Monsters.ForEach(monster => monster.OnBattleOver());

        // Clear up the list of battle units in the previous battle
        allBattleUnits.Clear();
        isForcedBattle = false;
    }


}