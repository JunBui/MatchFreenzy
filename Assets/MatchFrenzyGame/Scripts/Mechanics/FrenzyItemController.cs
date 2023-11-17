using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FrenzyItemController : MonoBehaviour
{
    public Outline Outline;
    public Rigidbody rb;
    private bool canSelect;
    // Start is called before the first frame update
    void Start()
    {
        canSelect = true;
        Outline = GetComponent<Outline>();
        FrenzyItemManager thisItemManager = GetComponentInParent<FrenzyItemManager>();
        rb = GetComponent<Rigidbody>();
        if (thisItemManager)
            thisItemManager.FrenzyItemController = this;
    }

    public void OnSelected()
    {
        if(canSelect == false)
            return;
        SetOutLine(2.3f,Color.green);
        this.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .75f);
    }

    public void GetItem(Transform MoveToPos)
    {
        if(canSelect == false)
            return;
        if (rb)
            rb.isKinematic = true;
        canSelect = false;
        Vector3 modifyMovePos = MoveToPos.position;
        modifyMovePos.y += .1f;
        transform.DOMove(modifyMovePos, 1);
        FrenzyGameManager.Instance.CheckCanMoveAwayThreeItem();
    }

    public void OnDeselect()
    {
        if(canSelect == false)
            return;
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
