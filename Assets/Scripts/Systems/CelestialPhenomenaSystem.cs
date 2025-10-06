using UnityEngine;
using System.Collections.Generic;

public class CelestialPhenomenaSystem : MonoBehaviour
{
    [Header("Phenomena Database")]
    public List<PhenomenonEffect> phenomenaEffects;
    
    [Header("Settings")]
    public bool useResourceLoading = true;
    
    private GameManager gameManager;
    private PhenomenonEffect currentPhenomenon;
    private int remainingDuration;
    
    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        LoadPhenomenaEffects();
        
        gameManager.OnRoundChanged.AddListener(ProcessRoundTick);
    }
    
    private void LoadPhenomenaEffects()
    {
        if (useResourceLoading)
        {
            phenomenaEffects = new List<PhenomenonEffect>();
            PhenomenonEffect[] allEffects = Resources.LoadAll<PhenomenonEffect>("PhenomenaEffects");
            
            foreach (PhenomenonEffect effect in allEffects)
            {
                phenomenaEffects.Add(effect);
            }
        }
        
        // Create default effects if none are loaded
        if (phenomenaEffects.Count == 0)
        {
            CreateDefaultPhenomena();
        }
    }
    
    public void TriggerPhenomenon(int favorValue)
    {
        PhenomenonEffect selectedPhenomenon = SelectPhenomenonByFavor(favorValue);
        
        if (selectedPhenomenon != null)
        {
            ApplyPhenomenon(selectedPhenomenon);
            
            if (gameManager.uiManager != null)
                gameManager.uiManager.ShowPhenomenonPopup(selectedPhenomenon);
            
            if (gameManager.audioManager != null)
                gameManager.audioManager.PlayPhenomenonSFX();
        }
    }
    
    private PhenomenonEffect SelectPhenomenonByFavor(int favorValue)
    {
        List<PhenomenonEffect> validEffects = new List<PhenomenonEffect>();
        
        foreach (PhenomenonEffect effect in phenomenaEffects)
        {
            if (effect.triggerThreshold == favorValue || effect.triggerThreshold == Mathf.Abs(favorValue))
            {
                validEffects.Add(effect);
            }
        }
        
        if (validEffects.Count > 0)
        {
            int randomIndex = Random.Range(0, validEffects.Count);
            return validEffects[randomIndex];
        }
        
        return null;
    }
    
    private void ApplyPhenomenon(PhenomenonEffect phenomenon)
    {
        // End current phenomenon if one is active
        if (currentPhenomenon != null)
        {
            currentPhenomenon.isActive = false;
        }
        
        currentPhenomenon = phenomenon;
        currentPhenomenon.isActive = true;
        remainingDuration = phenomenon.duration;
        
        Debug.Log($"Celestial Phenomenon Triggered: {phenomenon.effectName} - Duration: {remainingDuration} rounds");
    }
    
    private void ProcessRoundTick(int round)
    {
        if (currentPhenomenon != null && remainingDuration > 0)
        {
            remainingDuration--;
            
            if (gameManager.uiManager != null)
                gameManager.uiManager.UpdatePhenomenonCountdown(remainingDuration);
            
            if (remainingDuration <= 0)
            {
                EndCurrentPhenomenon();
            }
        }
    }
    
    private void EndCurrentPhenomenon()
    {
        if (currentPhenomenon != null)
        {
            currentPhenomenon.isActive = false;
            
            if (gameManager.uiManager != null)
                gameManager.uiManager.ShowPhenomenonEndMessage();
            
            Debug.Log($"Celestial Phenomenon Ended: {currentPhenomenon.effectName}");
            currentPhenomenon = null;
        }
    }
    
    public void ClearActivePhenomenon()
    {
        if (currentPhenomenon != null)
        {
            currentPhenomenon.isActive = false;
            currentPhenomenon = null;
            remainingDuration = 0;
        }
    }
    
    private void CreateDefaultPhenomena()
    {
        phenomenaEffects = new List<PhenomenonEffect>();
        
        var crimsonEclipse = ScriptableObject.CreateInstance<PhenomenonEffect>();
        crimsonEclipse.effectName = "🔥 Crimson Eclipse";
        crimsonEclipse.description = "All Attack cards lose 2 Attack this round and next.";
        crimsonEclipse.duration = 2;
        crimsonEclipse.triggerThreshold = 2;
        crimsonEclipse.effectCategory = EffectCategory.Attack;
        phenomenaEffects.Add(crimsonEclipse);
        
        var torrentialMoonfall = ScriptableObject.CreateInstance<PhenomenonEffect>();
        torrentialMoonfall.effectName = "🌊 Torrential Moonfall";
        torrentialMoonfall.description = "Defense cards cost +1 Energy.";
        torrentialMoonfall.duration = 2;
        torrentialMoonfall.triggerThreshold = 2;
        torrentialMoonfall.effectCategory = EffectCategory.Defense;
        phenomenaEffects.Add(torrentialMoonfall);
        
        var shadowVeil = ScriptableObject.CreateInstance<PhenomenonEffect>();
        shadowVeil.effectName = "🌑 Shadow Veil";
        shadowVeil.description = "Support card effects are halved.";
        shadowVeil.duration = 2;
        shadowVeil.triggerThreshold = 4;
        shadowVeil.effectCategory = EffectCategory.Support;
        phenomenaEffects.Add(shadowVeil);
        
        var radiantBlessing = ScriptableObject.CreateInstance<PhenomenonEffect>();
        radiantBlessing.effectName = "🌕 Radiant Blessing";
        radiantBlessing.description = "Both teams recover 1 random cooldown card.";
        radiantBlessing.duration = 2;
        radiantBlessing.triggerThreshold = 4;
        radiantBlessing.effectCategory = EffectCategory.Global;
        phenomenaEffects.Add(radiantBlessing);
    }
    
    public PhenomenonEffect GetCurrentPhenomenon()
    {
        return currentPhenomenon;
    }
    
    public int GetRemainingDuration()
    {
        return remainingDuration;
    }
}