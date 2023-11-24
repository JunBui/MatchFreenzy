using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Modules.DesignPatterns.EventManager;
using Modules.DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FrenzyGameManager : SingletonMono<FrenzyGameManager>
{
    public List<FrenzyItemManager> FrenzyItemManagers = new List<FrenzyItemManager>();
    public List<Transform> FrenzyItemHolder = new List<Transform>();
    public List<FrenzyItemManager> FrenzyDataHolder = new List<FrenzyItemManager>();
    public Transform DestroyPos;
    Dictionary<string,int> FrenzyIdExists = new Dictionary<string, int>();
    public FrenzyItemController CurrentSelectedItem;
    public FrenzyItemController LastSelectedItem;
    private int currentHolderIndex;
    private bool canCheckGameFail;
    List<string> garbageList = new List<string>();
    List<FrenzyItemManager> removeVisualList = new List<FrenzyItemManager>();
    private void Start()
    {
        canCheckGameFail = true;
        currentHolderIndex = 0;
    }
    public void CheckGameWin()
    {
        FrenzySpawnItemManager.Instance.CheckGameWin();
    }
    public void CheckGameFail()
    {
        if (canCheckGameFail)
        {
            Debug.Log("Fail game");
            canCheckGameFail = false;
            DOVirtual.DelayedCall(1, (() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }));
        }
    }
    public void AddItemToDataHolder(FrenzyItemManager item)
    {
        if (item != null)
        {
            FrenzyDataHolder.Add(item);
            if (FrenzyIdExists.ContainsKey(item.id))
            {
                FrenzyIdExists[item.id]++;
            }
            else
            { 
                FrenzyIdExists.Add(item.id,1);
            }
        }
    }
    public void CheckCanMoveAwayThreeItem()
    {
        garbageList = new List<string>();
        //AddToGarbageList
        foreach (var frenzyItem in FrenzyDataHolder)
        {
            int value;
            if(!FrenzyIdExists.TryGetValue(frenzyItem.id,out value))
            {
                value = 0;
            }
            if ( value >= 3 && garbageList.Contains(frenzyItem.id) == false)
            {
                Debug.Log("item need to remove: " + frenzyItem.id + " ---- Had in holder: " + value);
                garbageList.Add(frenzyItem.id);
                FrenzyIdExists.Remove(frenzyItem.id);
            }
        }

        if (garbageList.Count <= 0)
        {
            if(FrenzyDataHolder.Count == FrenzyItemHolder.Count)
                CheckGameFail();
            return;
        }

        Debug.Log("----Start remove item data----");
        removeVisualList = new List<FrenzyItemManager>();
        //Remove 3 item
        int deleteNumber = 0;
        for (int i = FrenzyDataHolder.Count - 1; i >= 0; i--)
        {
            if (garbageList.Contains(FrenzyDataHolder[i].id))
            {
                FrenzyItemManager tmp = FrenzyDataHolder[i];
                removeVisualList.Add(tmp);
                FrenzyDataHolder.RemoveAt(i);
                deleteNumber++;
                if(deleteNumber>=3)
                    break;
            }
        }
        CheckCanRemoveThreeItemVisual();
        Debug.Log("delete number: " + deleteNumber);
        Debug.Log("frenzy data holder: " + FrenzyDataHolder.Count);
    }
    public void CheckCanRemoveThreeItemVisual()
    {
            Debug.Log("----Start remove item visual----");
            foreach (var item in removeVisualList)
            {
                item.FrenzyItemController.DestroyItem(DestroyPos,(() =>
                {
                    if(item == removeVisualList[removeVisualList.Count-1])
                        ReOrderNotDeleteItem();
                    item.gameObject.SetActive(false);
                }));
            }
    }
    public void ReOrderNotDeleteItem()
    {
        Debug.Log("---- Reorder item ----");
        int index = 0;
        foreach (var frenzyItem in FrenzyDataHolder)
        {
            if(index>=FrenzyItemHolder.Count)
                break;
            frenzyItem.FrenzyItemController.MoveTo(FrenzyItemHolder[index], (() => { }));
            index++;
        }
        currentHolderIndex=index;
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
                RaycastHit hit = CastRay();
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("FrenzyItem"))
                    {
                        FrenzyItemController tmpController = hit.collider.GetComponent<FrenzyItemController>();
                        if (tmpController)
                        {
                            if(LastSelectedItem != null)
                                LastSelectedItem.OnDeselect();
                            CurrentSelectedItem = tmpController;
                            LastSelectedItem = tmpController;
                            CurrentSelectedItem.OnSelected();
                        }
                    }
                }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (LastSelectedItem)
            {
                LastSelectedItem.OnDeselect();
                LastSelectedItem = null;
            }

            if (CurrentSelectedItem)
            {
                if(currentHolderIndex>=FrenzyItemHolder.Count)
                    return;
                CurrentSelectedItem.GetItem(FrenzyItemHolder[currentHolderIndex],currentHolderIndex);
                currentHolderIndex++;
                CurrentSelectedItem.OnDeselect();
                CurrentSelectedItem = null;
            }
        }
        
    }
    private RaycastHit CastRay()
    {
        Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);
        return hit;
    }
}
