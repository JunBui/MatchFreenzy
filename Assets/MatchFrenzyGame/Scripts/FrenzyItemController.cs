using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FrenzyItemController : MonoBehaviour
{
    public Outline Outline;
    // Start is called before the first frame update
    void Start()
    {
        Outline = GetComponent<Outline>();
        FrenzyItemManager thisItemManager = GetComponentInParent<FrenzyItemManager>();
        if (thisItemManager)
            thisItemManager.FrenzyItemController = this;
    }

    public void OnSelected()
    {
        SetOutLine(2.3f,Color.green);
        this.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .75f);
    }

    public void GetItem()
    {
        FrenzyGameManager.Instance.CheckCanMoveAwayThreeItem();
    }

    public void OnDeselect()
    {
        ResetOutline();
        this.transform.DOScale(Vector3.one, .75f);
    }

    public void ResetOutline()
    {
        Outline.enabled = false;
    }

    public void SetOutLine(float Width, Color outlintColor)
    {
        if (Outline)
        {
            Outline.enabled = true;
            Outline.OutlineWidth = Width;
            Outline.OutlineColor = outlintColor;
        }
    }
}
