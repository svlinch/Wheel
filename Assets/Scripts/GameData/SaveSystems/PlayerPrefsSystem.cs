using UnityEngine;
using Assets.Scripts.GameData;
using Assets.Scripts.Saves;

public class PlayerPrefsSystem : ISaveSystem
{
    private const string SAVE_KEY = "mainData";
    public PlayerTemplate Load()
    {
        try
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                var data = JsonUtility.FromJson<PlayerTemplate>(PlayerPrefs.GetString(SAVE_KEY));
                return data;
            }
            return new PlayerTemplate();
        }
        catch
        {
            Debug.LogWarning("Something went wrong with loading player data. New data generated.");
            return new PlayerTemplate();
        }
    }

    public void Save(PlayerTemplate data)
    {
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }
}
