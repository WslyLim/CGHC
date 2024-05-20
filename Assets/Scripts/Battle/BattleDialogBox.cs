using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Color highlightedColour;

    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject MoveDetails;
    [SerializeField] GameObject SPDetail;

    [Header("Player HUD (Move and Action Selector)")]
    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] List<TextMeshProUGUI> moveTexts;

    [Header("Enemy HUD (Text and Background)")]
    public List<TextMeshProUGUI> enemyTexts;
    public List<Image> enemyBackgroundImages;

    [Header("Party HUD (Text and Background)")]
    public List<TextMeshProUGUI> partyTexts;
    public List<Image> partyBackgroundImages;

    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI SpText;

    public void SetDialog(string text)
    {
        dialogText.text = text;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray()) 
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/60);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled) 
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled) 
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        MoveDetails.SetActive(enabled);
        SPDetail.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColour;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightedColour;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }

        typeText.text = $"Move Type: {move.MoveBase.Type} \n" +
            $"Move Power: {move.MoveBase.Power} \n" +
            $"Move Accuracy: {move.MoveBase.Accuracy}";

        SpText.text = $"Move SP: {move.SP} / {move.MoveBase.SP}";

        if (move.SP == 0)
            SpText.color = Color.red;
        else
            SpText.color = Color.black;
    }


    public void UpdateEnemyTargetSelection(int selectedTarget)
    {
        for (int i = 0; i < enemyTexts.Count; i++) 
        {
            if (i == selectedTarget)
            {
                enemyTexts[i].color = highlightedColour;
                enemyBackgroundImages[i].color = Color.yellow;
            }
            else
            {
                enemyTexts[i].color = Color.black;
                enemyBackgroundImages[i].color = Color.white;
            }
        }
    }

    public void UpdateAllyTargetSelection(int selectedTarget)
    {
        for (int i = 0; i < partyTexts.Count; i++)
        {
            if (i == selectedTarget)
            {
                partyTexts[i].color = highlightedColour;
                partyBackgroundImages[i].color = Color.yellow;
            }
            else
            {
                partyTexts[i].color = Color.black;
                partyBackgroundImages[i].color = Color.white;
            }
        }
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].MoveBase.MoveName;
            else
                moveTexts[i].text = "-";
        }
    }
}
