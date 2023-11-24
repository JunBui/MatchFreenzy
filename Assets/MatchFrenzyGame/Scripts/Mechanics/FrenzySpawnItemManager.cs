using System.Collections;
using System.Collections.Generic;
using Modules.DesignPatterns.EventManager;
using Modules.DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.Serialization;

public class FrenzySpawnItemManager : SingletonMono<FrenzySpawnItemManager>
{
    public List<Transform> SpawnItemPoints = new List<Transform>();
    Dictionary<string,int> FrenzyMissions = new Dictionary<string, int>();
    [FormerlySerializedAs("LevelData")] public FrenzyLevelList LevelListData;
    private int currentSpawnIndex;
    // Start is called before the first frame update
    void Start()
    {
        TrySpawnLevel(FrenzySaveManager.Instance.GetCurrentLevelId());
        EventManager.Instance.AddListener<FrenzyGameEvents.GetFrezyItem>(GetFrenzyItemEventHandler);
    }
    public void TrySpawnLevel(int index)
    {
        if (index >= 0 && index < LevelListData.LevelList.Count)
        {
            InitLevel(LevelListData.LevelList[index]);
        }
        else
        {
            bool canGetRandomLevel = FrenzySaveManager.Instance.CanGetRandomLevel();
            int randomLevelIndex = FrenzySaveManager.Instance.GetRandomLevelId();
            if (canGetRandomLevel)
            {
                randomLevelIndex = Random.Range(0, LevelListData.LevelList.Count);
                FrenzySaveManager.Instance.SetRandomLevelId(randomLevelIndex);
                FrenzySaveManager.Instance.SetCanGetRandomLevel(false);
            }
            InitLevel(LevelListData.LevelList[randomLevelIndex]);
        }
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
    public void InitLevel(FrenzyLevelData levelData)
    {
        currentSpawnIndex = 0;
        foreach (var item in levelData.Levels)
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
        foreach (var mission in levelData.Missions)
        {
            FrenzyMissions.Add(mission.Item.id,mission.AmountOfItem);
        }
        FrenzyMenuMainGame.Instance.Init(levelData);
    }
    public void CheckGameWin()
    {
        if (FrenzyMissions.Count == 0)
        {
            Debug.Log("Win game");
            FrenzyGameManager.Instance.WinGame();
        }
    }
}
