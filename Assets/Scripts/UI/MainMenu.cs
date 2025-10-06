using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public Button startGameButton;
    public Button moonJudgmentButton;
    public Button celestialPhenomenaButton;
    public Button rulesButton;
    public Button optionsButton;
    public Button creditsButton;
    
    [Header("Continue Game")]
    public Button continueButton;
    public TextMeshProUGUI saveInfoText;
    
    [Header("Version Info")]
    public TextMeshProUGUI versionText;
    
    private PersistenceManager persistenceManager;
    
    private void Start()
    {
        SetupMainMenu();
    }
    
    private void SetupMainMenu()
    {
        persistenceManager = FindObjectOfType<PersistenceManager>();
        
        SetupButtonListeners();
        CheckForSaveData();
        SetVersionInfo();
    }
    
    private void SetupButtonListeners()
    {
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartNewGame);
        
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);
        
        if (moonJudgmentButton != null)
            moonJudgmentButton.onClick.AddListener(ShowMoonJudgmentInfo);
        
        if (celestialPhenomenaButton != null)
            celestialPhenomenaButton.onClick.AddListener(ShowCelestialPhenomenaInfo);
        
        if (rulesButton != null)
            rulesButton.onClick.AddListener(ShowRules);
        
        if (optionsButton != null)
            optionsButton.onClick.AddListener(ShowOptions);
        
        if (creditsButton != null)
            creditsButton.onClick.AddListener(ShowCredits);
    }
    
    private void CheckForSaveData()
    {
        bool hasSaveData = persistenceManager != null && persistenceManager.HasSaveData();
        
        if (continueButton != null)
            continueButton.gameObject.SetActive(hasSaveData);
        
        if (hasSaveData && saveInfoText != null && persistenceManager != null)
        {
            GameSaveData saveData = persistenceManager.GetCurrentSaveData();
            if (saveData != null)
            {
                saveInfoText.text = $"Continue: Round {saveData.currentRound}, Favor {saveData.favorTracker}";
            }
        }
    }
    
    private void SetVersionInfo()
    {
        if (versionText != null)
        {
            versionText.text = $"Rise of the Bakunawa v1.0\nCompanion App";
        }
    }
    
    public void StartNewGame()
    {
        // Clear any existing save data when starting new game
        if (persistenceManager != null)
        {
            persistenceManager.DeleteSaveData();
        }
        
        LoadGameScene();
    }
    
    public void ContinueGame()
    {
        LoadGameScene();
    }
    
    private void LoadGameScene()
    {
        // Load the main game scene (assuming it's at index 1)
        SceneManager.LoadScene("GameScene");
    }
    
    public void ShowMoonJudgmentInfo()
    {
        // Show info popup about Moon's Judgment system
        Debug.Log("Moon's Judgment: Triggered on rounds 3, 5, 7, and 9");
    }
    
    public void ShowCelestialPhenomenaInfo()
    {
        // Show info popup about Celestial Phenomena
        Debug.Log("Celestial Phenomena: Triggered when Favor reaches ±2 or ±4");
    }
    
    public void ShowRules()
    {
        // Load rules scene or show rules popup
        Debug.Log("Opening game rules");
    }
    
    public void ShowOptions()
    {
        SceneManager.LoadScene("OptionsScene");
    }
    
    public void ShowCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }
    
    public void QuitApplication()
    {
        Application.Quit();
    }
}
