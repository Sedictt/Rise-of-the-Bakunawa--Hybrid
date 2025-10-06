using UnityEngine;
using System.Collections.Generic;

public class MoonsJudgmentSystem : MonoBehaviour
{
    [Header("Moon Effects Database")]
    public List<MoonEffect> normalMoonEffects;
    public List<MoonEffect> neutralMoonEffects;
    
    [Header("Settings")]
    public bool useResourceLoading = true;
    
    private GameManager gameManager;
    private MoonEffect currentEffect;
    private int lastRoundWinner = 0; // -1 = Bakunawa, 0 = Tie, 1 = Tribesmen
    
    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        LoadMoonEffects();
    }
    
    private void LoadMoonEffects()
    {
        if (useResourceLoading)
        {
            normalMoonEffects = new List<MoonEffect>();
            neutralMoonEffects = new List<MoonEffect>();
            
            MoonEffect[] allEffects = Resources.LoadAll<MoonEffect>("MoonEffects");
            
            foreach (MoonEffect effect in allEffects)
            {
                if (effect.moonType == MoonType.Normal)
                    normalMoonEffects.Add(effect);
                else
                    neutralMoonEffects.Add(effect);
            }
        }
        
        // Create default effects if none are loaded
        if (normalMoonEffects.Count == 0)
        {
            CreateDefaultNormalEffects();
        }
        
        if (neutralMoonEffects.Count == 0)
        {
            CreateDefaultNeutralEffects();
        }
    }
    
    public void TriggerMoonsJudgment()
    {
        MoonEffect selectedEffect = SelectRandomMoonEffect();
        
        if (selectedEffect != null)
        {
            ApplyMoonEffect(selectedEffect);
            
            if (gameManager.uiManager != null)
                gameManager.uiManager.ShowMoonEffectPopup(selectedEffect);
            
            if (gameManager.audioManager != null)
                gameManager.audioManager.PlayMoonJudgmentSFX();
        }
    }
    
    private MoonEffect SelectRandomMoonEffect()
    {
        List<MoonEffect> effectPool;
        
        // Determine which pool to use based on last round result
        if (lastRoundWinner == 0) // Tie = Neutral Moon
        {
            effectPool = neutralMoonEffects;
        }
        else // Normal Moon for winner/loser
        {
            effectPool = normalMoonEffects;
        }
        
        if (effectPool.Count > 0)
        {
            int randomIndex = Random.Range(0, effectPool.Count);
            return effectPool[randomIndex];
        }
        
        return null;
    }
    
    private void ApplyMoonEffect(MoonEffect effect)
    {
        currentEffect = effect;
        
        // Apply favor changes if any
        if (effect.favorModifier != 0)
        {
            gameManager.AdjustFavor(effect.favorModifier);
        }
        
        Debug.Log($"Moon's Judgment Applied: {effect.effectName} - {effect.description}");
    }
    
    public void SetLastRoundWinner(int winner)
    {
        lastRoundWinner = winner;
    }
    
    private void CreateDefaultNormalEffects()
    {
        // Create some default normal moon effects for testing
        normalMoonEffects = new List<MoonEffect>();
        
        var lunarBlessing = ScriptableObject.CreateInstance<MoonEffect>();
        lunarBlessing.effectName = "🌕 Lunar Blessing";
        lunarBlessing.description = "+2 Attack to the next card you play.";
        lunarBlessing.moonType = MoonType.Normal;
        lunarBlessing.effectType = EffectType.Buff;
        lunarBlessing.attackModifier = 2;
        normalMoonEffects.Add(lunarBlessing);
        
        var tidalDrain = ScriptableObject.CreateInstance<MoonEffect>();
        tidalDrain.effectName = "🌊 Tidal Drain";
        tidalDrain.description = "Lose 1 Energy this round.";
        tidalDrain.moonType = MoonType.Normal;
        tidalDrain.effectType = EffectType.Debuff;
        tidalDrain.energyChange = -1;
        normalMoonEffects.Add(tidalDrain);
        
        var ancestralSurge = ScriptableObject.CreateInstance<MoonEffect>();
        ancestralSurge.effectName = "⚡ Ancestral Surge";
        ancestralSurge.description = "Gain +1 Energy this round.";
        ancestralSurge.moonType = MoonType.Normal;
        ancestralSurge.effectType = EffectType.Buff;
        ancestralSurge.energyChange = 1;
        normalMoonEffects.Add(ancestralSurge);
        
        var blankFate = ScriptableObject.CreateInstance<MoonEffect>();
        blankFate.effectName = "🌟 Blank Fate";
        blankFate.description = "Nothing happens.";
        blankFate.moonType = MoonType.Normal;
        blankFate.effectType = EffectType.Neutral;
        normalMoonEffects.Add(blankFate);
    }
    
    private void CreateDefaultNeutralEffects()
    {
        neutralMoonEffects = new List<MoonEffect>();
        
        var equalDrain = ScriptableObject.CreateInstance<MoonEffect>();
        equalDrain.effectName = "🌊 Equal Drain";
        equalDrain.description = "Both teams lose 1 Energy.";
        equalDrain.moonType = MoonType.Neutral;
        equalDrain.effectType = EffectType.Global;
        equalDrain.energyChange = -1;
        neutralMoonEffects.Add(equalDrain);
        
        var equalSurge = ScriptableObject.CreateInstance<MoonEffect>();
        equalSurge.effectName = "⚡ Equal Surge";
        equalSurge.description = "Both teams gain +1 Energy.";
        equalSurge.moonType = MoonType.Neutral;
        equalSurge.effectType = EffectType.Global;
        equalSurge.energyChange = 1;
        neutralMoonEffects.Add(equalSurge);
        
        var neutralBlank = ScriptableObject.CreateInstance<MoonEffect>();
        neutralBlank.effectName = "🌟 Blank Fate";
        neutralBlank.description = "Nothing happens.";
        neutralBlank.moonType = MoonType.Neutral;
        neutralBlank.effectType = EffectType.Neutral;
        neutralMoonEffects.Add(neutralBlank);
    }
    
    public MoonEffect GetCurrentEffect()
    {
        return currentEffect;
    }
}