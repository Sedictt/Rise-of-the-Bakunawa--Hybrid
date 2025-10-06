using UnityEngine;

[CreateAssetMenu(fileName = "PhenomenonEffect", menuName = "Bakunawa/Phenomenon Effect")]
public class PhenomenonEffect : ScriptableObject
{
    [Header("Basic Info")]
    public string effectName;
    public Sprite icon;
    [TextArea(3, 5)]
    public string description;
    
    [Header("Phenomenon Properties")]
    public int duration = 2;
    public int triggerThreshold;
    public EffectCategory effectCategory;
    
    [Header("Runtime State")]
    public bool isActive;
}

public enum EffectCategory
{
    Attack,
    Defense,
    Support,
    Global
}