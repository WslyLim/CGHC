using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class AttackSystem : MonoBehaviour
{
    private SpriteRenderer imageEnemy;
    private Vector2 defaultPos;

    private int defaultSelection = 0;

    private bool isAttacking = false;
    private bool isSelecting = false;

    public Transform targetLoc;

    public GameObject[] targets;
    public GameObject currentTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        defaultPos = transform.localPosition;
        targets = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        MovingTowards();
        SelectingTarget();
    }

    public void AttackBTN()
    {
        isAttacking = true;
    }

    //public void MovingTowards()
    //{
    //    Vector3 destinationLoc = (targetLoc.position - transform.position).normalized;

    //    if (isAttacking)
    //    {
    //        transform.position += new Vector3(destinationLoc.x, destinationLoc.y, 0) * 5 * Time.deltaTime;
    //    }
    //    else if (Vector2.Distance(transform.position, targetLoc.position) < 0.1)
    //    {
    //        isAttacking = false;

    //        transform.position -= new Vector3(destinationLoc.x, destinationLoc.y, 0) * 5 * Time.deltaTime;
    //        if (Vector2.Distance(defaultPos, transform.position) < 0.1)
    //        {
    //            transform.position = defaultPos;
    //        }
    //    }




    //}

    public void MovingTowards()
    {
        if (isAttacking)
        {
            // Move towards the target location
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.transform.position, 50 * Time.deltaTime);

            // If the object reaches the target location, stop attacking
            if (Vector2.Distance(transform.position, currentTarget.transform.position) < 0.1f)
            {
                StartCoroutine(HitImpact());
                isAttacking = false;
            }
        }
        else
        {
            // Return to the default position
            transform.position = Vector2.MoveTowards(transform.position, defaultPos, 50 * Time.deltaTime);
        }
    }

    IEnumerator HitImpact()
    {
        imageEnemy = currentTarget.GetComponent<SpriteRenderer>();
        imageEnemy.color = new Color32(255, 0, 0, 170);
        yield return new WaitForSeconds(0.2f);
        imageEnemy.color = new Color32(255, 255, 255, 255);

    }

    public void SelectingTarget()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isSelecting = true;
            currentTarget = targets[0];
        }
        if (Input.GetMouseButtonDown(1))
        {
            isSelecting = false;
            currentTarget = null;
        }

        if (isSelecting && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            defaultSelection = Mathf.Clamp(defaultSelection + 1, 0, 2);
            currentTarget = targets[defaultSelection];
            Debug.Log("Selecting " + currentTarget.ToString());
        }

        if (isSelecting && Input.GetKeyDown(KeyCode.RightArrow))
        {
            defaultSelection = Mathf.Clamp(defaultSelection - 1, 0, 2);
            currentTarget = targets[defaultSelection];
            Debug.Log("Selecting " + currentTarget.ToString());
        }

        if (currentTarget != null && Input.GetKeyDown(KeyCode.Space))
        {
            isAttacking = true;
        }
    }
}
    
