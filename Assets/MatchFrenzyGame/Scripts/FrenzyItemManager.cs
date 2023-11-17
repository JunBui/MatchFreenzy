using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrenzyItemManager : MonoBehaviour
{
    public FrenzyItemController FrenzyItemController;
    public string Id;
    

    private void Start()
    {
        FrenzyGameManager.Instance.FrenzyItemManagers.Add(this);
    }
}
