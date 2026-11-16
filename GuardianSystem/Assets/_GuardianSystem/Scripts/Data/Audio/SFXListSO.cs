using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    UI_Button_Hover,
    UI_Button_Click,
    UI_Slot_Drop,
    UI_Panel_Open,
    UI_Panel_Close
}

[System.Serializable]
public struct SFXData
{
    public SFXType type;
    public AudioClip sfx;
}

[CreateAssetMenu(fileName = "New SFX List", menuName = "GS/Audio/SFX List")]
public class SFXListSO : ScriptableObject
{
    [SerializeField] private List<SFXData> sfxList;
    public List<SFXData> SfxList => sfxList;

    public AudioClip GetSFX(SFXType type)
    {
        if (sfxList.Count <= 0) return null;

        foreach (SFXData sfx in sfxList)
        {
            if (sfx.type == type)
            {
                return sfx.sfx;
            }
        }

        return null;
    }
}
