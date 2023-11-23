using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrenzyItemManager : MonoBehaviour
{
    public FrenzyItemController FrenzyItemController;
    [SerializeField] private string id;
    private void Start()
    {
        FrenzyGameManager.Instance.FrenzyItemManagers.Add(this);
    }
}
