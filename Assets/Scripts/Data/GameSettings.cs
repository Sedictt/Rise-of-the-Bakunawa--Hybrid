using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Bakunawa/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Round Settings")]
    public int maxRounds = 10;
    public int[] moonJudgmentRounds = { 3, 5, 7, 9 };
    
    [Header("Favor Settings")]
    public int minFavor = -5;
    public int maxFavor = 5;
    public int[] phenomenonThresholds = { -4, -2, 2, 4 };
    
    [Header("Duration Settings")]
    public int moonEffectDuration = 1;
    public int phenomenonDuration = 2;
    public float popupDisplayTime = 3f;
    
    [Header("Audio Settings")]
    public float sfxVolume = 0.8f;
    public float musicVolume = 0.5f;
    
    [Header("UI Settings")]
    public bool autoTriggerEffects = true;
    public bool showHistoryLog = true;
    public bool enableAnimations = true;
    
    [Header("Visual Theme")]
    public Color primaryColor = Color.blue;
    public Color secondaryColor = Color.purple;
    public Color favorPositiveColor = Color.cyan;
    public Color favorNegativeColor = Color.red;
    
    public bool IsMoonJudgmentRound(int round)
    {
        foreach (int moonRound in moonJudgmentRounds)
        {
            if (round == moonRound)
                return true;
        }
        return false;
    }
    
    public bool IsPhenomenonTrigger(int favorValue)
    {
        foreach (int threshold in phenomenonThresholds)
        {
            if (favorValue == threshold)
                return true;
        }
        return false;
    }
}