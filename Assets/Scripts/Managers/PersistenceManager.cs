using UnityEngine;
using System;

[System.Serializable]
public class GameSaveData
{
    public int currentRound;
    public int favorTracker;
    public string activePhenomenonName;
    public int phenomenonDuration;
    public string lastMoonEffectName;
    public string saveTimestamp;
    
    public GameSaveData()
    {
        currentRound = 1;
        favorTracker = 0;
        activePhenomenonName = "";
        phenomenonDuration = 0;
        lastMoonEffectName = "";
        saveTimestamp = DateTime.Now.ToString();
    }
}

public class PersistenceManager : MonoBehaviour
{
    [Header("Save Settings")]
    public bool autoSave = true;
    public float autoSaveInterval = 30f;
    
    [Header("Keys")]
    private const string SAVE_DATA_KEY = "BakunawaGameData";
    private const string SETTINGS_KEY = "BakunawaSettings";
    
    private GameManager gameManager;
    private float lastSaveTime;
    
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        LoadGameData();
        
        if (autoSave)
        {
            InvokeRepeating(nameof(AutoSaveGame), autoSaveInterval, autoSaveInterval);
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameData();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGameData();
        }
    }
    
    public void SaveGameData()
    {
        if (gameManager == null) return;
        
        GameSaveData saveData = new GameSaveData();
        saveData.currentRound = gameManager.currentRound;
        saveData.favorTracker = gameManager.favorTracker;
        
        // Save active phenomenon
        if (gameManager.celestialPhenomena != null)
        {
            PhenomenonEffect activePhenomenon = gameManager.celestialPhenomena.GetCurrentPhenomenon();
            if (activePhenomenon != null)
            {
                saveData.activePhenomenonName = activePhenomenon.effectName;
                saveData.phenomenonDuration = gameManager.celestialPhenomena.GetRemainingDuration();
            }
        }
        
        // Save last moon effect
        if (gameManager.moonsJudgment != null)
        {
            MoonEffect lastEffect = gameManager.moonsJudgment.GetCurrentEffect();
            if (lastEffect != null)
            {
                saveData.lastMoonEffectName = lastEffect.effectName;
            }
        }
        
        string jsonData = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_DATA_KEY, jsonData);
        PlayerPrefs.Save();
        
        lastSaveTime = Time.time;
        Debug.Log("Game data saved successfully");
    }
    
    public void LoadGameData()
    {
        if (!PlayerPrefs.HasKey(SAVE_DATA_KEY)) return;
        
        string jsonData = PlayerPrefs.GetString(SAVE_DATA_KEY);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);
        
        if (saveData != null && gameManager != null)
        {
            gameManager.currentRound = saveData.currentRound;
            gameManager.favorTracker = saveData.favorTracker;
            
            // Update UI
            if (gameManager.uiManager != null)
            {
                gameManager.uiManager.UpdateRoundDisplay(saveData.currentRound);
                gameManager.uiManager.UpdateFavorDisplay(saveData.favorTracker);
            }
            
            Debug.Log($"Game data loaded: Round {saveData.currentRound}, Favor {saveData.favorTracker}");
        }
    }
    
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey(SAVE_DATA_KEY);
        PlayerPrefs.Save();
        Debug.Log("Save data deleted");
    }
    
    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_DATA_KEY);
    }
    
    public void SaveSettings(float sfxVolume, float musicVolume, bool enableAnimations)
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("EnableAnimations", enableAnimations ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void LoadSettings()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        bool enableAnimations = PlayerPrefs.GetInt("EnableAnimations", 1) == 1;
        
        // Apply settings to managers
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.SetSFXVolume(sfxVolume);
            audioManager.SetMusicVolume(musicVolume);
        }
        
        MysticalThemeManager themeManager = FindObjectOfType<MysticalThemeManager>();
        if (themeManager != null && !enableAnimations)
        {
            // Disable animations if needed
            themeManager.enabled = false;
        }
    }
    
    private void AutoSaveGame()
    {
        if (Time.time - lastSaveTime >= autoSaveInterval)
        {
            SaveGameData();
        }
    }
    
    public GameSaveData GetCurrentSaveData()
    {
        if (!PlayerPrefs.HasKey(SAVE_DATA_KEY)) return null;
        
        string jsonData = PlayerPrefs.GetString(SAVE_DATA_KEY);
        return JsonUtility.FromJson<GameSaveData>(jsonData);
    }
    
    public void ExportSaveData()
    {
        GameSaveData saveData = GetCurrentSaveData();
        if (saveData != null)
        {
            string exportJson = JsonUtility.ToJson(saveData, true);
            Debug.Log("Export Data: " + exportJson);
            
            // In a real implementation, you might save this to a file
            // or copy to clipboard for sharing between devices
        }
    }
}