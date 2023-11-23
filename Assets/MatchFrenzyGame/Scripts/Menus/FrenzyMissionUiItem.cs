using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Modules.DesignPatterns.EventManager;
using Modules.GameplayHelpers.Commons;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FrenzyMissionUiItem : BaseMonoBehaviour
{
    public Image Icon;
    public Text NumberOfMissionText;
    public int NumberOfMission;
    private FrenzyItemData thisItemUi;
    private string thisItemId;

    private void Start()
    {
        EventManager.Instance.AddListener<FrenzyGameEvents.GetFrezyItem>(GetFrenzyItemEventHandler,ref OnGameObjectDestroy);
    }
    public void GetFrenzyItemEventHandler(FrenzyGameEvents.GetFrezyItem param)
    {
        if (param.id == thisItemId)
        {
            NumberOfMission--;
            HandleUI();
        }
    }

    public void HandleUI()
    {
        NumberOfMissionText.text = NumberOfMission.ToString();
        if (NumberOfMission <= 0)
        {
            this.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f).OnComplete((() =>
            {
                this.transform.DOScale(Vector3.zero, .15f).OnComplete((() =>
                {
                    Destroy(this.gameObject);
                }));
            }));
        }
    }
    public void Init(FrenzyItemData itemData)
    {
        thisItemUi = itemData;
        thisItemId = thisItemUi.Item.id;
        Icon.sprite = itemData.Item.icon;
        NumberOfMission = itemData.AmountOfItem;
        NumberOfMissionText.text = NumberOfMission.ToString();
    }
}
