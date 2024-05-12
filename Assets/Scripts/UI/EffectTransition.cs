using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTransition : MonoBehaviour
{
    [SerializeField] Animator regularAnimator;
    [SerializeField] Animator cutsceneAnimator;
    public static EffectTransition Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public IEnumerator PlayTransition()
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
}
