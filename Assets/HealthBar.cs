using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    // Start is called before the first frame update
    public void SetHP(float value)
    {
        health.transform.localScale = new Vector3(value/5f, .2f);
    }

    public IEnumerator SetHPSmooth(float HP)
    {
        float newHP = HP / 5f;
        float curHP = health.transform.localScale.x;
        float changeAmount = curHP - newHP;

        while (curHP - newHP > Mathf.Epsilon)
        {
            curHP -= changeAmount * Time.deltaTime;
            health.transform.localScale = new Vector3(curHP, .2f);

            yield return null;
        }
        health.transform.localScale = new Vector3(newHP, .2f);
    }

    
}
