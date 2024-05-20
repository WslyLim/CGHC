using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTransition : MonoBehaviour
{
    [SerializeField] Animator regularAnimator;
    [SerializeField] Animator cutsceneAnimator;
    public static EffectTransition Instance { get; private set; }

    public Animator Animator => regularAnimator;
    public void Awake()
    {
        Instance = this;
    }

    public IEnumerator BattleTransition()
    {
        regularAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1.2f);
        regularAnimator.ResetTrigger("Start");
    }

    public IEnumerator SceneTransition() 
    {
        regularAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.8f);
        regularAnimator.ResetTrigger("FadeOut");
    }

    public IEnumerator SimpleTransition() 
    {
        regularAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.3f);
        regularAnimator.ResetTrigger("FadeOut");
    }
}
