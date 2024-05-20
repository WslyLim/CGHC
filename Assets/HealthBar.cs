using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        float changeAmount = Mathf.Abs(curHP - newHP); // Absolute difference

        while (Mathf.Abs(curHP - newHP) > Mathf.Epsilon)
        {
            // Move current HP towards new HP
            curHP = Mathf.MoveTowards(curHP, newHP, changeAmount * Time.deltaTime);
            health.transform.localScale = new Vector3(curHP, .2f, 1f); // Assuming 1f is the Z scale

            yield return null;
        }
        health.transform.localScale = new Vector3(newHP, .2f, 1f); // Final adjustment
    }

    
}
