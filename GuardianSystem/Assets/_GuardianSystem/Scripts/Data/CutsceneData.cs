using System;
using UnityEngine;

[Serializable]
public struct CutsceneDialog
{
    public PopularityLevel level;

    [TextArea(2, 5)]
    public string[] dialogues;
}

[CreateAssetMenu(fileName = "New Cutscene", menuName = "GS/Cutscene")]
public class CutsceneData : ScriptableObject
{
    public Sprite cutsceneImage;
    public CutsceneDialog[] dialogues;

    public string[] GetDialogues(PopularityLevel level)
    {
        foreach (CutsceneDialog dialog in dialogues)
        {
            if (dialog.level == level)
            {
                return dialog.dialogues;
            }
        }

        Debug.LogWarning($"No dialogues found for the {level} Popularity Level");
        return Array.Empty<string>();
    }
}
