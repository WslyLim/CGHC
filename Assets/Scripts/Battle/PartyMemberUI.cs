using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI monsterName;
    [SerializeField] TextMeshProUGUI monsterLevel;
    [SerializeField] HealthBar healthBar;
    [SerializeField] Image ExpBar;
    [SerializeField] Image background;

    

    public TextMeshProUGUI MonsterName => monsterName;
    public Image Background => background;

    public float transitionDuration;
    public void SetData(Monster monsterData)
    {
        _monster = monsterData;

        UpdateData();
        _monster.OnHPChanged += UpdateData;
    }


    Monster _monster;
    void UpdateData()
    {
        monsterName.text = _monster.Base.MonsterName;
        SetLevel();
        healthBar.SetHP((float)_monster.HP / _monster.MaxHealth);
    }

    public IEnumerator UpdateHP()
    {
        yield return healthBar.SetHPSmooth((float)_monster.HP / _monster.MaxHealth);
    }

    public void SetExp()
    {
        if (ExpBar == null) return;

        float normalizedExp = GetNormalizedExp();
        ExpBar.fillAmount = normalizedExp;

        Debug.Log(_monster.Exp.ToString());
        Debug.Log(normalizedExp.ToString());
        Debug.Log(_monster.Exp.ToString());
    }

    // Coroutine for smooth transition of fillAmount
    public IEnumerator SmoothTransition(bool reset)
    {
        if (ExpBar == null) yield break;

        if (reset)
            ExpBar.fillAmount = 0;

        float initialFill = ExpBar.fillAmount;
        float elapsedTime = 0f;
        float normalizedExp = GetNormalizedExp();

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            ExpBar.fillAmount = Mathf.Lerp(initialFill, normalizedExp, t);
            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set accurately
        ExpBar.fillAmount = normalizedExp;

        yield return new WaitForSeconds(1f);
    }

    float GetNormalizedExp()
    {
        int currLevelExp = _monster.Base.GetExpForLevel(_monster.Level);
        int nextLevelExp = _monster.Base.GetExpForLevel(_monster.Level + 1);

        float normalizedExp = (float)(_monster.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public void SetLevel()
    {
        monsterLevel.text = "Lvl " + _monster.Level;
    }
}
