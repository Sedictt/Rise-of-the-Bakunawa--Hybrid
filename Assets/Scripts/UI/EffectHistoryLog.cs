using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

[System.Serializable]
public class EffectHistoryEntry
{
    public string effectName;
    public string effectType; // "Moon" or "Phenomenon"
    public string description;
    public string timestamp;
    public int roundTriggered;
    
    public EffectHistoryEntry(string name, string type, string desc, int round)
    {
        effectName = name;
        effectType = type;
        description = desc;
        roundTriggered = round;
        timestamp = DateTime.Now.ToString("HH:mm:ss");
    }
}

public class EffectHistoryLog : MonoBehaviour
{
    [Header("History UI")]
    public GameObject historyPanel;
    public Transform historyContentParent;
    public GameObject historyEntryPrefab;
    public Button toggleHistoryButton;
    public Button clearHistoryButton;
    public ScrollRect historyScrollRect;
    
    [Header("Entry Display")]
    public int maxHistoryEntries = 50;
    
    private List<EffectHistoryEntry> historyEntries;
    private List<GameObject> historyEntryObjects;
    private GameManager gameManager;
    private bool isHistoryVisible = false;
    
    private void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        historyEntries = new List<EffectHistoryEntry>();
        historyEntryObjects = new List<GameObject>();
        
        gameManager = FindObjectOfType<GameManager>();
        
        SetupButtonListeners();
        
        if (historyPanel != null)
            historyPanel.SetActive(false);
        
        // Subscribe to game events if possible
        SubscribeToEvents();
    }
    
    private void SetupButtonListeners()
    {
        if (toggleHistoryButton != null)
            toggleHistoryButton.onClick.AddListener(ToggleHistoryPanel);
        
        if (clearHistoryButton != null)
            clearHistoryButton.onClick.AddListener(ClearHistory);
    }
    
    private void SubscribeToEvents()
    {
        // This would ideally be connected to events from the systems
        // For now, we'll provide public methods to add entries
    }
    
    public void AddMoonEffectEntry(MoonEffect effect)
    {
        if (effect != null && gameManager != null)
        {
            var entry = new EffectHistoryEntry(
                effect.effectName,
                "Moon's Judgment",
                effect.description,
                gameManager.currentRound
            );
            
            AddHistoryEntry(entry);
        }
    }
    
    public void AddPhenomenonEntry(PhenomenonEffect phenomenon)
    {
        if (phenomenon != null && gameManager != null)
        {
            var entry = new EffectHistoryEntry(
                phenomenon.effectName,
                "Celestial Phenomenon",
                phenomenon.description,
                gameManager.currentRound
            );
            
            AddHistoryEntry(entry);
        }
    }
    
    private void AddHistoryEntry(EffectHistoryEntry entry)
    {
        historyEntries.Insert(0, entry); // Add to the beginning (most recent first)
        
        // Remove oldest entries if we exceed the limit
        if (historyEntries.Count > maxHistoryEntries)
        {
            historyEntries.RemoveAt(historyEntries.Count - 1);
        }
        
        RefreshHistoryDisplay();
    }
    
    private void RefreshHistoryDisplay()
    {
        // Clear existing display objects
        foreach (GameObject obj in historyEntryObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        historyEntryObjects.Clear();
        
        // Create new display objects
        foreach (EffectHistoryEntry entry in historyEntries)
        {
            CreateHistoryEntryDisplay(entry);
        }
        
        // Scroll to top to show most recent entry
        if (historyScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            historyScrollRect.verticalNormalizedPosition = 1f;
        }
    }
    
    private void CreateHistoryEntryDisplay(EffectHistoryEntry entry)
    {
        if (historyEntryPrefab != null && historyContentParent != null)
        {
            GameObject entryObject = Instantiate(historyEntryPrefab, historyContentParent);
            historyEntryObjects.Add(entryObject);
            
            // Set up the entry display
            HistoryEntryDisplay display = entryObject.GetComponent<HistoryEntryDisplay>();
            if (display != null)
            {
                display.SetupEntry(entry);
            }
            else
            {
                // Fallback setup if no HistoryEntryDisplay component
                SetupEntryFallback(entryObject, entry);
            }
        }
    }
    
    private void SetupEntryFallback(GameObject entryObject, EffectHistoryEntry entry)
    {
        // Find text components in the entry object and set them up
        TextMeshProUGUI[] textComponents = entryObject.GetComponentsInChildren<TextMeshProUGUI>();
        
        if (textComponents.Length >= 3)
        {
            textComponents[0].text = entry.effectName;
            textComponents[1].text = $"Round {entry.roundTriggered} - {entry.timestamp}";
            textComponents[2].text = entry.description;
        }
        
        // Set color based on effect type
        Image backgroundImage = entryObject.GetComponent<Image>();
        if (backgroundImage != null)
        {
            if (entry.effectType == "Moon's Judgment")
                backgroundImage.color = new Color(0.2f, 0.3f, 0.8f, 0.3f); // Blue tint
            else
                backgroundImage.color = new Color(0.8f, 0.2f, 0.8f, 0.3f); // Purple tint
        }
    }
    
    public void ToggleHistoryPanel()
    {
        isHistoryVisible = !isHistoryVisible;
        
        if (historyPanel != null)
            historyPanel.SetActive(isHistoryVisible);
        
        if (toggleHistoryButton != null)
        {
            TextMeshProUGUI buttonText = toggleHistoryButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = isHistoryVisible ? "Hide History" : "Show History";
        }
    }
    
    public void ClearHistory()
    {
        historyEntries.Clear();
        RefreshHistoryDisplay();
    }
    
    public void SaveHistoryToPlayerPrefs()
    {
        // Simple serialization for persistence
        string historyJson = JsonUtility.ToJson(new SerializableHistoryList(historyEntries));
        PlayerPrefs.SetString("EffectHistory", historyJson);
    }
    
    public void LoadHistoryFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("EffectHistory"))
        {
            string historyJson = PlayerPrefs.GetString("EffectHistory");
            SerializableHistoryList loadedHistory = JsonUtility.FromJson<SerializableHistoryList>(historyJson);
            
            if (loadedHistory != null && loadedHistory.entries != null)
            {
                historyEntries = loadedHistory.entries;
                RefreshHistoryDisplay();
            }
        }
    }
}

[System.Serializable]
public class SerializableHistoryList
{
    public List<EffectHistoryEntry> entries;
    
    public SerializableHistoryList(List<EffectHistoryEntry> historyEntries)
    {
        entries = historyEntries;
    }
}

public class HistoryEntryDisplay : MonoBehaviour
{
    [Header("Entry Components")]
    public TextMeshProUGUI effectNameText;
    public TextMeshProUGUI effectTypeText;
    public TextMeshProUGUI timestampText;
    public TextMeshProUGUI descriptionText;
    public Image backgroundImage;
    
    public void SetupEntry(EffectHistoryEntry entry)
    {
        if (effectNameText != null)
            effectNameText.text = entry.effectName;
        
        if (effectTypeText != null)
            effectTypeText.text = entry.effectType;
        
        if (timestampText != null)
            timestampText.text = $"Round {entry.roundTriggered} - {entry.timestamp}";
        
        if (descriptionText != null)
            descriptionText.text = entry.description;
        
        if (backgroundImage != null)
        {
            if (entry.effectType == "Moon's Judgment")
                backgroundImage.color = new Color(0.2f, 0.3f, 0.8f, 0.3f);
            else
                backgroundImage.color = new Color(0.8f, 0.2f, 0.8f, 0.3f);
        }
    }
}