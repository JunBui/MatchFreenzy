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
    public FrenzyLevelData LevelData;
    public List<Transform> SpawnItemPoints = new List<Transform>();
    public List<FrenzyItemManager> FrenzyItemManagers = new List<FrenzyItemManager>();
    public List<Transform> FrenzyItemHolder = new List<Transform>();
    public List<FrenzyItemManager> FrenzyDataHolder = new List<FrenzyItemManager>();
    public Transform DestroyPos;
    Dictionary<string,int> FrenzyIdExists = new Dictionary<string, int>();
    Dictionary<string,int> FrenzyMissions = new Dictionary<string, int>();
    public FrenzyItemController CurrentSelectedItem;
    public FrenzyItemController LastSelectedItem;
    private int currentHolderIndex;
    private int currentSpawnIndex;
    private bool canCheckGameFail;
    private void Start()
    {
        canCheckGameFail = true;
        currentHolderIndex = 0;
        InitLevel();
        EventManager.Instance.AddListener<FrenzyGameEvents.GetFrezyItem>(GetFrenzyItemEventHandler);
    }

    public void GetFrenzyItemEventHandler(FrenzyGameEvents.GetFrezyItem param)
    {
        if (FrenzyMissions.ContainsKey(param.id))
        {
            FrenzyMissions[param.id]--;
            if (FrenzyMissions[param.id] <= 0)
            {
                FrenzyMissions.Remove(param.id);
                CheckGameWin();
            }
        }
    }

    public void CheckGameWin()
    {
        if (FrenzyMissions.Count == 0)
        {
            Debug.Log("Win game");
        }
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

    public void InitLevel()
    {
        currentSpawnIndex = 0;
        foreach (var item in LevelData.Levels)
        {
            int numberOfItem = item.AmountOfItem - item.AmountOfItem % 3;
            for (int i = 0; i < numberOfItem; i++)
            {
                if (currentSpawnIndex >= SpawnItemPoints.Count)
                    currentSpawnIndex = 0;
                Instantiate(item.Item.gameObject, SpawnItemPoints[currentSpawnIndex]);
                currentSpawnIndex++;
            }
        }
        foreach (var mission in LevelData.Missions)
        {
            FrenzyMissions.Add(mission.Item.id,mission.AmountOfItem);
        }
        FrenzyMenuMainGame.Instance.Init(LevelData);
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
        // Debug.Log("----");
        // foreach (var frenzyId in FrenzyIdExists)
        // {
        //     Debug.Log("Frenzy: " + frenzyId.Key + " --- value: " +frenzyId.Value );
        // }
        // Debug.Log("----");
        CheckCanMoveAwayThreeItem();
    }
    

    public void CheckCanMoveAwayThreeItem()
    {
        List<string> garbageList = new List<string>();
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
                garbageList.Add(frenzyItem.id);
                FrenzyIdExists.Remove(frenzyItem.id);
            }
        }

        List<FrenzyItemManager> tmpList = new List<FrenzyItemManager>();
        //Remove 3 item
        int deleteNumber = 0;
        for (int i = FrenzyDataHolder.Count - 1; i >= 0; i--)
        {
            if (garbageList.Contains(FrenzyDataHolder[i].id))
            {
                FrenzyItemManager tmp = FrenzyDataHolder[i];
                tmp.FrenzyItemController.DestroyItem(DestroyPos,(() =>
                {
                    tmp.gameObject.SetActive(false);
                    ReOrderNotDeleteItem();
                }));
                tmpList.Add(tmp);
                FrenzyDataHolder.RemoveAt(i);
                deleteNumber++;
                if(deleteNumber>=3)
                    break;
            }
        }
        if(deleteNumber == 0 && FrenzyDataHolder.Count>=FrenzyItemHolder.Count)
            CheckGameFail();
    }

    public void ReOrderNotDeleteItem()
    {
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
