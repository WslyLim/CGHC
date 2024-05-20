using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] TextMeshProUGUI entityName;
    [SerializeField] int letterPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogManager Instance {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void HandleUpdate()
    {
        
    }

    Dialog dialog;
    Action onDialogFinished;

    int currentLine = 0;
    bool isTyping;
    public bool isShowing { get; private set; }
    public IEnumerator ShowDialog(Dialog dialog, string entityName = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        isShowing = true;
        this.entityName.text = entityName ?? string.Empty;

        dialogBox.SetActive(true);

        foreach (var line in dialog.Lines) 
        {
            yield return TypeDialog(line);
            yield return new WaitUntil (() => Input.GetKeyDown(KeyCode.Z));
        }

        dialogBox.SetActive(false);
        isShowing = false;
        OnCloseDialog?.Invoke();
    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
    }

    public IEnumerator ShowDialogText(string dialog, string entityName = null, bool waitForInput = true)
    {
        isShowing = true;
        OnShowDialog?.Invoke();
        this.entityName.text = entityName ?? string.Empty;

        dialogBox.SetActive(true);
        yield return TypeDialog(dialog);

        if (waitForInput) 
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        dialogBox.SetActive(false);
        isShowing = false;
        OnCloseDialog?.Invoke();
    }
}
