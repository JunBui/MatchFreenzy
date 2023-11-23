using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Modules.DesignPatterns.EventManager;
using UnityEngine;

public class FrenzyItemController : MonoBehaviour
{
    public Outline Outline;
    public Rigidbody rb;
    public FrenzyItemManager FrenzyItemManager;
    private bool canSelect;

    private Vector3 defaultLocalScale;
    // Start is called before the first frame update
    void Start()
    {
        defaultLocalScale = this.transform.localScale;
        canSelect = true;
        Outline = GetComponent<Outline>();
        FrenzyItemManager thisItemManager = GetComponentInParent<FrenzyItemManager>();
        rb = GetComponent<Rigidbody>();
        if (thisItemManager)
        {
            thisItemManager.FrenzyItemController = this;
            FrenzyItemManager = thisItemManager;
        }
    }

    public void OnSelected()
    {
        if(canSelect == false)
            return;
        SetOutLine(2.3f,Color.green);
        this.transform.DOScale(defaultLocalScale*1.1f, .75f);
    }

    public void GetItem(Transform MoveToPos, int holderIndex)
    {
        if(canSelect == false)
            return;
        if (rb)
            rb.isKinematic = true;
        if (FrenzyItemManager)
        {
            FrenzyItemManager.HolderIndex = holderIndex;
            EventManager.Instance.TriggerEvent(new FrenzyGameEvents.GetFrezyItem()
            {
                id = FrenzyItemManager.id
            });
            
        }
        canSelect = false;
        MoveTo(MoveToPos,(() =>
        {
            FrenzyGameManager.Instance.AddItemToDataHolder(FrenzyItemManager);
            FrenzyGameManager.Instance.CheckGameFail();
        }));
        this.transform.DOScale(Vector3.one, .75f);
    }

    public void MoveTo(Transform MoveToPos,Action OnComplete)
    {
        Vector3 modifyMovePos = MoveToPos.position;
        modifyMovePos.y += .1f;

        transform.DOMove(modifyMovePos, 1).OnComplete((() => { OnComplete?.Invoke();}));
    }

    public void DestroyItem(Transform MoveToPos,Action OnComplete)
    {
        transform.DOMove(MoveToPos.position, .5f).OnComplete((() =>
        {
            transform.DOScale(new Vector3(1.2f,1.2f,1.2f),.5f).OnComplete((() =>
            {
                transform.DOScale(Vector3.zero, .2f).OnComplete((() =>
                {
                    OnComplete?.Invoke();
                }));
            }));
        }));
    }

    public void OnDeselect()
    {
        if(canSelect == false)
            return;
        ResetOutline();
        this.transform.DOScale(defaultLocalScale, .75f);
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
