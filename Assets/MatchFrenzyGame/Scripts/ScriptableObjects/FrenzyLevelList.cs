using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "Config/LevelList", order = 1)]
public class FrenzyLevelList : ScriptableObject
{
    public List<FrenzyLevelData> LevelList;
}
