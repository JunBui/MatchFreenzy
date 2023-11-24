using System.Collections;
using System.Collections.Generic;
using Modules.DesignPatterns.Singleton;
using UnityEngine;

public class FrenzySaveManager : Singleton<FrenzySaveManager>
{
    public int GetCurrentLevelId()
    {
        return PlayerPrefs.GetInt("frenzy_current_level_id", 0);
    }
    public void IncreaseLevelId()
    {
        int current = GetCurrentLevelId();
        current += 1;
        PlayerPrefs.SetInt("frenzy_current_level_id", current);

        IncreaseLevelTextId();
    }
    
    public int GetCurrentLevelTextId()
    {
        return PlayerPrefs.GetInt("frenzy_current_level_text_id", 1);
    }

    private void IncreaseLevelTextId()
    {
        int current = GetCurrentLevelTextId();
        current++;
        PlayerPrefs.SetInt("frenzy_current_level_text_id", current);
    }

    public int GetRandomLevelId()
    {
        return PlayerPrefs.GetInt("frenzy_current_random_level_id", 0);
    }

    public void SetRandomLevelId(int level)
    {
        PlayerPrefs.SetInt("frenzy_current_random_level_id", level);
    }

    public bool CanGetRandomLevel()
    {
        if (PlayerPrefs.GetInt("frenzy_can_get_random_level_id", 1) > 0)
        {
            return true;
        }

        return false;
    }

    public void SetCanGetRandomLevel(bool can)
    {
        if (can)
        {
            PlayerPrefs.SetInt("frenzy_can_get_random_level_id", 1);
        }
        else
        {
            PlayerPrefs.SetInt("frenzy_can_get_random_level_id", 0);
        }
    }
}
