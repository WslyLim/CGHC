using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] List<GameObject> Menu;

    public event Action onClickOpen;
    public event Action onClickClose;

    public static MenuController Instance;
    void Awake()
    {
        Instance = this;
    }

}
