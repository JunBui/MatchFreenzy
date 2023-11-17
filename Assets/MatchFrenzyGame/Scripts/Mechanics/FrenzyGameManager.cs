using System;
using System.Collections;
using System.Collections.Generic;
using Modules.DesignPatterns.Singleton;
using UnityEngine;

public class FrenzyGameManager : SingletonMono<FrenzyGameManager>
{
    public List<FrenzyItemManager> FrenzyItemManagers = new List<FrenzyItemManager>();
    public List<Transform> FrenzyItemHolder;
    public FrenzyItemController CurrentSelectedItem;
    public FrenzyItemController LastSelectedItem;
    private int currentHolderIndex;

    private void Start()
    {
        currentHolderIndex = 0;
    }

    public void CheckCanMoveAwayThreeItem()
    {
        
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
                CurrentSelectedItem.GetItem(FrenzyItemHolder[currentHolderIndex]);
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
