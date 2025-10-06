using UnityEngine;

[CreateAssetMenu(fileName = "MoonEffect", menuName = "Bakunawa/Moon Effect")]
public class MoonEffect : ScriptableObject
{
    [Header("Basic Info")]
    public string effectName;
    public Sprite icon;
    [TextArea(3, 5)]
    public string description;
    
    [Header("Moon Properties")]
    public MoonType moonType;
    public EffectType effectType;
    public int duration = 1;
    
    [Header("Effect Modifiers")]
    public int energyChange;
    public int attackModifier;
    public int defenseModifier;
    public int favorModifier;
    
    [Header("Special Conditions")]
    [TextArea(2, 3)]
    public string specialCondition;
}

public enum MoonType
{
    Normal,
    Neutral
}

public enum EffectType
{
    Buff,
    Debuff,
    Global,
    Neutral
}