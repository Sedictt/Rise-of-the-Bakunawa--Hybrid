using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UIReferences
{
    [Header("Phase UI")]
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI timerText;
    public Image phaseProgressBar;
    
    [Header("Game State")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI favorText;
    public Slider favorSlider;
    
    [Header("Dice")]
    public GameObject dicePanel;
    public TextMeshProUGUI bakunawaDiceText;
    public TextMeshProUGUI tribesmenDiceText;
    public TextMeshProUGUI initiativeWinnerText;
    
    [Header("Controls")]
    public Button startGameButton;
    public Button nextPhaseButton;
    public Button resetGameButton;
    public Button favorUpButton;
    public Button favorDownButton;
    
    [Header("Round Winner")]
    public GameObject roundWinnerPanel;
    public Button bakunawaWinButton;
    public Button tribesmenWinButton;
}

public class GameSetupHelper : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool autoSetupGameManager = true;
    public UIReferences uiReferences;
    
    [Header("Prefab References")]
    public GameObject gameManagerPrefab;
    
    private void Start()
    {
        if (autoSetupGameManager)
        {
            SetupGameManager();
        }
    }
    
    public void SetupGameManager()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        
        if (gameManager == null)
        {
            // Create GameManager if it doesn't exist
            GameObject managerObj = new GameObject("GameManager");
            gameManager = managerObj.AddComponent<GameManager>();
            
            // Add required components
            managerObj.AddComponent<PhaseManager>();
            managerObj.AddComponent<DiceSystem>();
            managerObj.AddComponent<MoonsJudgmentSystem>();
            managerObj.AddComponent<CelestialPhenomenaSystem>();
            managerObj.AddComponent<UIManager>();
            managerObj.AddComponent<AudioManager>();
        }
        
        // Auto-assign components
        AssignComponents(gameManager);
        AssignUIReferences(gameManager);
    }
    
    private void AssignComponents(GameManager gameManager)
    {
        if (gameManager.phaseManager == null)
            gameManager.phaseManager = gameManager.GetComponent<PhaseManager>();
        
        if (gameManager.diceSystem == null)
            gameManager.diceSystem = gameManager.GetComponent<DiceSystem>();
        
        if (gameManager.moonsJudgment == null)
            gameManager.moonsJudgment = gameManager.GetComponent<MoonsJudgmentSystem>();
        
        if (gameManager.celestialPhenomena == null)
            gameManager.celestialPhenomena = gameManager.GetComponent<CelestialPhenomenaSystem>();
        
        if (gameManager.uiManager == null)
            gameManager.uiManager = gameManager.GetComponent<UIManager>();
        
        if (gameManager.audioManager == null)
            gameManager.audioManager = gameManager.GetComponent<AudioManager>();
    }
    
    private void AssignUIReferences(GameManager gameManager)
    {
        UIManager uiManager = gameManager.uiManager;
        if (uiManager == null) return;
        
        // Assign UI references
        uiManager.phaseText = uiReferences.phaseText;
        uiManager.timerText = uiReferences.timerText;
        uiManager.phaseProgressBar = uiReferences.phaseProgressBar;
        
        uiManager.roundText = uiReferences.roundText;
        uiManager.favorText = uiReferences.favorText;
        uiManager.favorSlider = uiReferences.favorSlider;
        
        uiManager.dicePanel = uiReferences.dicePanel;
        uiManager.bakunawaDiceText = uiReferences.bakunawaDiceText;
        uiManager.tribesmenDiceText = uiReferences.tribesmenDiceText;
        uiManager.initiativeWinnerText = uiReferences.initiativeWinnerText;
        
        uiManager.startGameButton = uiReferences.startGameButton;
        uiManager.nextPhaseButton = uiReferences.nextPhaseButton;
        uiManager.resetGameButton = uiReferences.resetGameButton;
        uiManager.favorUpButton = uiReferences.favorUpButton;
        uiManager.favorDownButton = uiReferences.favorDownButton;
        
        uiManager.roundWinnerPanel = uiReferences.roundWinnerPanel;
        uiManager.bakunawaWinButton = uiReferences.bakunawaWinButton;
        uiManager.tribesmenWinButton = uiReferences.tribesmenWinButton;
    }
    
    [ContextMenu("Create Basic UI Layout")]
    public void CreateBasicUILayout()
    {
        GameObject canvas = FindObjectOfType<Canvas>()?.gameObject;
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
            return;
        }
        
        // Create main UI panel
        GameObject mainPanel = CreateUIPanel(canvas, "MainGameUI");
        
        // Create phase display
        CreatePhaseUI(mainPanel);
        
        // Create game state display
        CreateGameStateUI(mainPanel);
        
        // Create dice panel
        CreateDiceUI(mainPanel);
        
        // Create control buttons
        CreateControlButtons(mainPanel);
        
        // Create round winner panel
        CreateRoundWinnerUI(mainPanel);
    }
    
    private GameObject CreateUIPanel(GameObject parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent.transform, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        
        return panel;
    }
    
    private void CreatePhaseUI(GameObject parent)
    {
        // Phase text
        GameObject phaseTextObj = new GameObject("PhaseText");
        phaseTextObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI phaseText = phaseTextObj.AddComponent<TextMeshProUGUI>();
        phaseText.text = "ROUND START";
        phaseText.fontSize = 24;
        phaseText.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = phaseTextObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.9f);
        rect.anchorMax = new Vector2(1, 1);
        rect.sizeDelta = Vector2.zero;
        
        uiReferences.phaseText = phaseText;
        
        // Timer text
        GameObject timerTextObj = new GameObject("TimerText");
        timerTextObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI timerText = timerTextObj.AddComponent<TextMeshProUGUI>();
        timerText.text = "01:30";
        timerText.fontSize = 18;
        timerText.alignment = TextAlignmentOptions.Center;
        
        RectTransform timerRect = timerTextObj.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(0, 0.8f);
        timerRect.anchorMax = new Vector2(1, 0.9f);
        timerRect.sizeDelta = Vector2.zero;
        
        uiReferences.timerText = timerText;
    }
    
    private void CreateGameStateUI(GameObject parent)
    {
        // Round display
        GameObject roundObj = new GameObject("RoundText");
        roundObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI roundText = roundObj.AddComponent<TextMeshProUGUI>();
        roundText.text = "Round 1";
        roundText.fontSize = 20;
        
        RectTransform roundRect = roundObj.GetComponent<RectTransform>();
        roundRect.anchorMin = new Vector2(0, 0.7f);
        roundRect.anchorMax = new Vector2(0.5f, 0.8f);
        roundRect.sizeDelta = Vector2.zero;
        
        uiReferences.roundText = roundText;
        
        // Favor display
        GameObject favorObj = new GameObject("FavorText");
        favorObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI favorText = favorObj.AddComponent<TextMeshProUGUI>();
        favorText.text = "Favor: 0";
        favorText.fontSize = 20;
        
        RectTransform favorRect = favorObj.GetComponent<RectTransform>();
        favorRect.anchorMin = new Vector2(0.5f, 0.7f);
        favorRect.anchorMax = new Vector2(1, 0.8f);
        favorRect.sizeDelta = Vector2.zero;
        
        uiReferences.favorText = favorText;
    }
    
    private void CreateDiceUI(GameObject parent)
    {
        // Create dice panel
        GameObject dicePanel = CreateUIPanel(parent, "DicePanel");
        dicePanel.SetActive(false);
        
        RectTransform dicePanelRect = dicePanel.GetComponent<RectTransform>();
        dicePanelRect.anchorMin = new Vector2(0.2f, 0.4f);
        dicePanelRect.anchorMax = new Vector2(0.8f, 0.6f);
        dicePanelRect.sizeDelta = Vector2.zero;
        
        // Add background
        Image background = dicePanel.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.7f);
        
        uiReferences.dicePanel = dicePanel;
        
        // Bakunawa dice
        GameObject bakunawaDice = new GameObject("BakunawaDice");
        bakunawaDice.transform.SetParent(dicePanel.transform, false);
        
        TextMeshProUGUI bakunawaText = bakunawaDice.AddComponent<TextMeshProUGUI>();
        bakunawaText.text = "6";
        bakunawaText.fontSize = 36;
        bakunawaText.alignment = TextAlignmentOptions.Center;
        
        RectTransform bakunawaRect = bakunawaDice.GetComponent<RectTransform>();
        bakunawaRect.anchorMin = new Vector2(0, 0);
        bakunawaRect.anchorMax = new Vector2(0.5f, 1);
        bakunawaRect.sizeDelta = Vector2.zero;
        
        uiReferences.bakunawaDiceText = bakunawaText;
        
        // Tribesmen dice
        GameObject tribesmenDice = new GameObject("TribesmenDice");
        tribesmenDice.transform.SetParent(dicePanel.transform, false);
        
        TextMeshProUGUI tribesmenText = tribesmenDice.AddComponent<TextMeshProUGUI>();
        tribesmenText.text = "4";
        tribesmenText.fontSize = 36;
        tribesmenText.alignment = TextAlignmentOptions.Center;
        
        RectTransform tribesmenRect = tribesmenDice.GetComponent<RectTransform>();
        tribesmenRect.anchorMin = new Vector2(0.5f, 0);
        tribesmenRect.anchorMax = new Vector2(1, 1);
        tribesmenRect.sizeDelta = Vector2.zero;
        
        uiReferences.tribesmenDiceText = tribesmenText;
    }
    
    private void CreateControlButtons(GameObject parent)
    {
        // Start Game button
        CreateButton(parent, "StartGameButton", "START GAME", new Vector2(0, 0.1f), new Vector2(0.3f, 0.2f));
        
        // Next Phase button
        CreateButton(parent, "NextPhaseButton", "NEXT PHASE", new Vector2(0.35f, 0.1f), new Vector2(0.65f, 0.2f));
        
        // Reset Game button
        CreateButton(parent, "ResetGameButton", "RESET", new Vector2(0.7f, 0.1f), new Vector2(1, 0.2f));
    }
    
    private Button CreateButton(GameObject parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = Vector2.zero;
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.3f, 0.8f, 0.8f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 14;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        return button;
    }
    
    private void CreateRoundWinnerUI(GameObject parent)
    {
        // Create round winner panel
        GameObject winnerPanel = CreateUIPanel(parent, "RoundWinnerPanel");
        winnerPanel.SetActive(false);
        
        RectTransform winnerRect = winnerPanel.GetComponent<RectTransform>();
        winnerRect.anchorMin = new Vector2(0.3f, 0.3f);
        winnerRect.anchorMax = new Vector2(0.7f, 0.7f);
        winnerRect.sizeDelta = Vector2.zero;
        
        // Add background
        Image background = winnerPanel.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.8f);
        
        uiReferences.roundWinnerPanel = winnerPanel;
        
        // Bakunawa win button
        uiReferences.bakunawaWinButton = CreateButton(winnerPanel, "BakunawaWinButton", "BAKUNAWA WINS", 
            new Vector2(0, 0.6f), new Vector2(1, 0.8f));
        
        // Tribesmen win button
        uiReferences.tribesmenWinButton = CreateButton(winnerPanel, "TribesmenWinButton", "TRIBESMEN WIN", 
            new Vector2(0, 0.2f), new Vector2(1, 0.4f));
    }
}