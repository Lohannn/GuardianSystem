using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int lastDayNumber = 0;
    public float lastPopularityValue = 50f;
}

public static class SaveData
{
    private const string PLAYER_SAVE_KEY = "GuardianSystem_Player_SaveFile";
    public static PlayerData currentPlayerData;

    public static PlayerData LoadGame()
    {
        if (PlayerPrefs.HasKey(PLAYER_SAVE_KEY))
        {
            string playerJson = PlayerPrefs.GetString(PLAYER_SAVE_KEY);
            currentPlayerData = JsonUtility.FromJson<PlayerData>(playerJson);
        }
        else
        {
            currentPlayerData = new();
        }

        Debug.Log(currentPlayerData);

        return currentPlayerData;
    }

    public static void SaveGame()
    {
        string playerJson = JsonUtility.ToJson(currentPlayerData);
        PlayerPrefs.SetString(PLAYER_SAVE_KEY, playerJson);

        PlayerPrefs.Save();
    }
}
