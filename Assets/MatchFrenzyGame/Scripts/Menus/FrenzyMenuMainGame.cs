using System.Collections;
using System.Collections.Generic;
using Modules.Systems.MenuSystem;
using UnityEngine;

public class FrenzyMenuMainGame : SimpleMenu<FrenzyMenuMainGame>
{
    public Transform MisionParrent;

    public GameObject MisionItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(FrenzyLevelData LevelData)
    {
        foreach (var mission in LevelData.Missions)
        {
            GameObject missionUI = Instantiate(MisionItem,MisionParrent);
            FrenzyMissionUiItem missionUiItem = missionUI.GetComponent<FrenzyMissionUiItem>();
            if (missionUiItem)
                missionUiItem.Init(mission);
        }
        MisionItem.SetActive(false);
    }
}
