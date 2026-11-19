using System;
using UnityEngine;

[Serializable]
public struct CutsceneDialog
{
    public PopularityLevel level;

    [TextArea(2, 5)]
    public string[] dialogues;

    public AudioClip[] audioClips;
}

[Serializable]
public struct CutsceneImage
{
    public PopularityLevel level;

    public Sprite image;
}

[CreateAssetMenu(fileName = "New Cutscene", menuName = "GS/Cutscene")]
public class CutsceneData : ScriptableObject
{
    public CutsceneImage[] images;
    public CutsceneDialog[] dialogues;

    public CutsceneDialog GetDialogues(PopularityLevel level)
    {
        foreach (CutsceneDialog dialog in dialogues)
        {
            if (dialog.level == level)
            {
                return dialog;
            }
        }

        Debug.LogWarning($"No dialogues found for the {level} Popularity Level");
        return default;
    }

    public Sprite GetImage(PopularityLevel level)
    {
        foreach (CutsceneImage image in images)
        {
            if (image.level == level)
            {
                return image.image;
            }
        }

        Debug.LogWarning($"No images found for the {level} Popularity Level");
        return null;
    }
}
