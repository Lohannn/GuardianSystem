using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "GS/Unit")]
public class UnitSO : ScriptableObject
{
    public string id;
    public Sprite icon;
    public UnitType type;
    public int speedBySecond;
}
