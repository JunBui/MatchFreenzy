using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Config/LevelData", order = 1)]
public class FrenzyLevelData : ScriptableObject
{
    public List<FrenzyItemData> Missions;
    public List<FrenzyItemData> Levels;
}

[Serializable]
public class FrenzyItemData
{
    public FrenzyItemManager Item;
    public int AmountOfItem;
}
