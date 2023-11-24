using System.Collections;
using System.Collections.Generic;
using Modules.DesignPatterns.EventManager;
using Modules.DesignPatterns.Singleton;
using UnityEngine;

public class FrenzySpawnItemManager : SingletonMono<FrenzySpawnItemManager>
{
    public List<Transform> SpawnItemPoints = new List<Transform>();
    Dictionary<string,int> FrenzyMissions = new Dictionary<string, int>();
    public FrenzyLevelData LevelData;
    private int currentSpawnIndex;
    // Start is called before the first frame update
    void Start()
    {
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
    public void CheckGameWin()
    {
        if (FrenzyMissions.Count == 0)
        {
            Debug.Log("Win game");
        }
    }
}
