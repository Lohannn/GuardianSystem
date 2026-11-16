using System.Collections.Generic;
using UnityEngine;

public enum MusicType
{
    MainMenu,
    Stage,
    BadEnding,
    GoodEnding
}

[System.Serializable]
public struct MusicData
{
    public MusicType type;
    public AudioClip music;
}

[CreateAssetMenu(fileName = "New Music List", menuName = "GS/Audio/Music List")]
public class MusicListSO : ScriptableObject
{
    [SerializeField] private List<MusicData> musicList;

    public List<MusicData> MusicList => musicList;

    public AudioClip GetMusic(MusicType type)
    {
        if (musicList.Count <= 0) return null;

        foreach (MusicData music in musicList)
        {
            if (music.type == type)
            {
                return music.music;
            }
        }

        return null;
    }
}
