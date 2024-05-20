using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI monsterName;
    [SerializeField] TextMeshProUGUI monsterLevel;
    [SerializeField] HealthBar healthBar;
    [SerializeField] Image ExpBar;

    Monster _monster;

    public float transitionDuration;
    public void SetData(Monster monsterData)
    {
        if (_monster != null)
            _monster.OnHPChanged -= UpdateHP;

        _monster = monsterData;

        monsterName.text = monsterData.Base.MonsterName;
        SetLevel();
        healthBar.SetHP( (float) monsterData.HP / monsterData.MaxHealth);
        SetExp();
        _monster.OnHPChanged += UpdateHP;
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return healthBar.SetHPSmooth((float)_monster.HP / _monster.MaxHealth);
    }

    public void SetExp()
    {
        if (ExpBar == null) return;
        
        float normalizedExp = GetNormalizedExp();
        ExpBar.fillAmount = normalizedExp;
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
