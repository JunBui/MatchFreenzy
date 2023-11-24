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
    public Collider Collider;
    public FrenzyItemManager FrenzyItemManager;
    private bool canSelect;

    private Vector3 defaultLocalScale;
    // Start is called before the first frame update
    void Start()
    {
        defaultLocalScale = this.transform.localScale;
        canSelect = true;
        if(Outline == null)
            Outline = GetComponentInChildren<Outline>();
        if(Collider == null)
            Collider = GetComponentInChildren<Collider>();
        // FrenzyItemManager thisItemManager = GetComponentInParent<FrenzyItemManager>();
        if(rb == null)
            rb = GetComponentInChildren<Rigidbody>();
        // if (thisItemManager)
        // {
        //     thisItemManager.FrenzyItemController = this;
        //     FrenzyItemManager = thisItemManager;
        // }
    }

    public void OnSelected()
    {
        if(canSelect == false)
            return;
        SetOutLine(4.5f,Color.green);
        this.transform.DOScale(defaultLocalScale*1.2f, .35f);
    }

    public void GetItem(Transform MoveToPos, int holderIndex)
    {
        if(canSelect == false)
            return;
        if (rb)
            rb.isKinematic = true;
        if (Collider)
            Collider.enabled = false;
        if (FrenzyItemManager)
        {
            FrenzyItemManager.HolderIndex = holderIndex;
            FrenzyGameManager.Instance.CheckGameWin();
            EventManager.Instance.TriggerEvent(new FrenzyGameEvents.GetFrezyItem()
            {
                id = FrenzyItemManager.id
            });
            
        }
        canSelect = false;
        MoveTo(MoveToPos,(() =>
        {
            Debug.Log("Move To Holder complete");
            FrenzyGameManager.Instance.AddItemToDataHolder(FrenzyItemManager);
            FrenzyGameManager.Instance.CheckCanMoveAwayThreeItem();
        }));
        this.transform.DOScale(Vector3.one, .75f);
    }

    public void MoveTo(Transform MoveToPos,Action OnComplete)
    {
        Vector3 modifyMovePos = MoveToPos.position;
        modifyMovePos.y += .1f;
        transform.DOLocalRotate(new Vector3(45,45,45), .45f);
        transform.DOMove(modifyMovePos, .45f).OnComplete((() => { OnComplete?.Invoke();})).SetEase(Ease.InOutSine).SetId("MoveThisItem"+this.GetInstanceID());
    }

    public void DestroyItem(Transform MoveToPos,Action OnComplete)
    {
        DOTween.Kill("MoveThisItem" + this.GetInstanceID());
        transform.DOMove(MoveToPos.position, .25f).OnComplete((() =>
        {
            transform.DOScale(new Vector3(1.2f,1.2f,1.2f),.25f).OnComplete((() =>
            {
                transform.DOScale(Vector3.zero, .05f).OnComplete((() =>
                {
                    OnComplete?.Invoke();
                })).SetEase(Ease.InOutSine);
            })).SetEase(Ease.InOutSine);
        })).SetEase(Ease.InOutSine);
    }

    public void OnDeselect()
    {
        if(canSelect == false)
            return;
        ResetOutline();
        this.transform.DOScale(defaultLocalScale, .35f);
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
