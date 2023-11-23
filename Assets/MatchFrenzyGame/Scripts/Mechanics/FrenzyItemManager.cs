using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrenzyItemManager : MonoBehaviour
{
    public FrenzyItemController FrenzyItemController;
    public string id;
    public Sprite icon;
    public int HolderIndex = -1;
    private void Start()
    {
        HolderIndex = -1;
        FrenzyGameManager.Instance.FrenzyItemManagers.Add(this);
    }
}
